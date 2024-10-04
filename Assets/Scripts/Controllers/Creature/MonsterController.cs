using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    private CancellationTokenSource _dotDamageCancelToken = new CancellationTokenSource();

    private bool _isDotDamageRunning = false;

    Vector3 _moveDir = Vector3.zero;

    public event Action<MonsterController> onMonsterInfoUpdate;
    public override bool Init()
    {
        if (!base.Init())
            return false;

        IsMonster = true;
        _animator = GetComponent<Animator>();
        ObjectType = ObjectType.Monster;
        CreatureState = CreatureState.Moving;

        return true;
    }

    public override void InitCreatureStat()
    {
        base.InitCreatureStat();

        MaxHp = creatureData.maxHp;
        Hp = MaxHp;
        Atk = creatureData.atk;
        MoveSpeed = creatureData.moveSpeed;
    }

    private void FixedUpdate()
    {
        var playerController = Managers.Object.Player;

        if (playerController.IsValid() == false)
            return;

        if (CreatureState != CreatureState.Moving)
            return;

        _moveDir = playerController.transform.position - transform.position;

        Vector3 newPos = transform.position + _moveDir.normalized * Time.fixedDeltaTime * MoveSpeed;
        GetComponent<Rigidbody2D>().MovePosition(newPos);
        GetComponent<SpriteRenderer>().flipX = _moveDir.x < 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (!player.IsValid())
            return;

        if (!this.IsValid())
            return;

        UniTaskUtils.CancelTokenSource(ref _dotDamageCancelToken);
        _dotDamageCancelToken = new CancellationTokenSource();

        TaskStartDotDamage(player).Forget();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (!player.IsValid())
            return;

        if (!this.IsValid())
            return;

        _dotDamageCancelToken.Cancel();
    }
    
    public override void OnDamaged(BaseController attacker, SkillBase skill = null, float damage = 0)
    {
        if (skill != null)
        {
            // 사운드 예정
        }

        float totalDmg = Managers.Game.Player.Atk * skill.SkillData.DamageMultiplier;
        base.OnDamaged(attacker, skill, totalDmg);
        InvokeMonsterData();

        KnockBackTask().Forget();
    }

    protected override void OnDead()
    {
        base.OnDead();
        InvokeMonsterData();


        //var gemController = Managers.Object.Spawn<GemController>(transform.position, 0);

        Managers.Object.Despawn(this);

        Managers.Game.KillCount++;
    }

    private void OnEnable()
    {
        UniTaskUtils.CancelTokenSource(ref _disableCancelToken);
        _disableCancelToken = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _disableCancelToken.Cancel();
    }

    private void OnDestroy()
    {
        _dotDamageCancelToken.Cancel();
    }


    private UniTaskCompletionSource _knockbackCompleteSource = null;
    private CancellationTokenSource _disableCancelToken = null;

    private async UniTask KnockBackTask()
    {
        // 이전 넉백 태스크가 실행 중이라면 대기
        if (_knockbackCompleteSource != null)
        {
            return;
        }

        CreatureState = CreatureState.OnDamaged;

        // 새로운 UniTaskCompletionSource 생성
        _knockbackCompleteSource = new UniTaskCompletionSource();

        float elapsed = 0;
        CancellationToken token;
        
        try
        {
            token = CancellationTokenSource.CreateLinkedTokenSource(_disableCancelToken.Token, destroyCancellationToken).Token;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        while (true)
        {
            elapsed += Time.deltaTime;
            if (elapsed > KNOCKBACK_TIME)
                break;

            // 현재 움직이던 방향의 반대로 밀어내기
            Vector3 dir = _moveDir * -1f;

            Vector2 nextVec = dir.normalized * KNOCKBACK_SPEED * Time.fixedDeltaTime;
            _rigidBody.MovePosition(_rigidBody.position + nextVec);
            await UniTask.NextFrame(cancellationToken: token);
        }

        CreatureState = CreatureState.Moving;

        // 연속적으로 밀려나지 않도록 쿨다운
        int knockbackCoolTimeMilliSec = (int)(KNOCKBACK_COOLTIME * 1000);
        await UniTask.Delay(knockbackCoolTimeMilliSec, cancellationToken: token);
        
        // 태스크 끝
        _knockbackCompleteSource.TrySetResult();
        _knockbackCompleteSource = null;
    }

    public async UniTaskVoid TaskStartDotDamage(PlayerController target)
    {
        var token = CancellationTokenSource.CreateLinkedTokenSource(_dotDamageCancelToken.Token, destroyCancellationToken).Token;

        while (true)
        {
            target.OnDamaged(this, damage: Atk);
            await UniTask.Delay(1000, cancellationToken: token);
        }
    }

    public void InvokeMonsterData()
    {
        if (this.IsValid() && gameObject.IsValid() && ObjectType != ObjectType.Monster)
        {
            onMonsterInfoUpdate?.Invoke(this);
        }
    }
}
