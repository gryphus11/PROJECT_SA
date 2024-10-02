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
    private CancellationTokenSource _dotDamageCancelToken;

    private bool _isDotDamageRunning = false;

    Vector3 _moveDir = Vector3.zero;

    public override bool Init()
    {
        if (!base.Init())
            return false;

        _animator = GetComponent<Animator>();
        ObjectType = ObjectType.Monster;
        CreatureState = CreatureState.Moving;

        return true;
    }

    public override void InitCreatureStat()
    {
        base.InitCreatureStat();

        MaxHp = creatureData.maxHp;
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

    private void OnTriggerEnter2D(Collider2D collision)
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (!player.IsValid())
            return;

        if (!this.IsValid())
            return;

        UniTaskUtils.CancelTokenSource(ref _dotDamageCancelToken);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
       
    }

    protected override void OnDead()
    {
        base.OnDead();

        UniTaskUtils.CancelTokenSource(ref _dotDamageCancelToken);

        var gemController = Managers.Object.Spawn<GemController>(transform.position, 0);

        Managers.Object.Despawn(this);

        Managers.Game.KillCount++;
    }

    private void OnEnable()
    {
        _disableCancelToken = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        UniTaskUtils.CancelTokenSource(ref _dotDamageCancelToken);
        UniTaskUtils.CancelTokenSource(ref _disableCancelToken);
    }

    private void OnDestroy()
    {
        UniTaskUtils.CancelTokenSource(ref _dotDamageCancelToken);
    }

    public override void OnDamaged(BaseController attacker, float damage)
    {
        base.OnDamaged(attacker, damage);

        KnockBackTask().Forget();
    }

    private UniTaskCompletionSource _knockbackCompleteSource = null;
    private CancellationTokenSource _disableCancelToken;

    private async UniTask KnockBackTask()
    {
        // ���� �˹� �½�ũ�� ���� ���̶�� ���
        if (_knockbackCompleteSource != null)
        {
            return;
        }

        CreatureState = CreatureState.OnDamaged;

        // ���ο� UniTaskCompletionSource ����
        _knockbackCompleteSource = new UniTaskCompletionSource();

        float elapsed = 0;

        var token = CancellationTokenSource.CreateLinkedTokenSource(_disableCancelToken.Token, destroyCancellationToken).Token;
        Debug.Log("KnockBackTask Start");

        while (true)
        {
            elapsed += Time.deltaTime;
            if (elapsed > KNOCKBACK_TIME)
                break;

            // ���� �����̴� ������ �ݴ�� �о��
            Vector3 dir = _moveDir * -1f;

            Vector2 nextVec = dir.normalized * KNOCKBACK_SPEED * Time.fixedDeltaTime;
            _rigidBody.MovePosition(_rigidBody.position + nextVec);
            Debug.Log($"KnockBackTask MovePosition : {_rigidBody.position}");
            await UniTask.NextFrame(cancellationToken: token);
        }

        CreatureState = CreatureState.Moving;

        // ���������� �з����� �ʵ��� ��ٿ�
        int knockbackCoolTimeMilliSec = (int)(KNOCKBACK_COOLTIME * 1000);
        await UniTask.Delay(knockbackCoolTimeMilliSec, cancellationToken: token);
        
        // �½�ũ ��
        _knockbackCompleteSource.TrySetResult();
        _knockbackCompleteSource = null;
    }

    public async UniTaskVoid TaskStartDotDamage(PlayerController target)
    {
        var token = CancellationTokenSource.CreateLinkedTokenSource(_dotDamageCancelToken.Token, destroyCancellationToken).Token;

        while (true)
        {
            target.OnDamaged(this, Atk);
            await UniTask.Delay(1000, cancellationToken: token);
        }
    }


}
