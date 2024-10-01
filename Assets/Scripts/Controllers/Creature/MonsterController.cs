using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    #region State Pattern
    Define.CreatureState _creatureState = Define.CreatureState.Moving;
    public virtual Define.CreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            _creatureState = value;
            UpdateAnimation();
        }
    }

    protected Animator _animator;

    public virtual void UpdateAnimation()
    {

    }

    protected override void UpdateController()
    {
        base.UpdateController();

        switch (CreatureState)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {

    }

    protected virtual void UpdateMoving()
    {

    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }
    #endregion

    private CancellationTokenSource _cancellationTokenSource;

    private bool _isDotDamageRunning = false;

    public override bool Init()
    {
        if (!base.Init())
            return false;

        _animator = GetComponent<Animator>();
        ObjectType = ObjectType.Monster;
        CreatureState = CreatureState.Moving;

        return true;
    }

    private void FixedUpdate()
    {
        if (CreatureState != CreatureState.Moving)
            return;

        var playerController = Managers.Object.Player;

        if (playerController == null)
            return;

        Vector3 dir = playerController.transform.position - transform.position;

        Vector3 newPos = transform.position + dir.normalized * Time.fixedDeltaTime * moveSpeed;

        GetComponent<Rigidbody2D>().MovePosition(newPos);
        GetComponent<SpriteRenderer>().flipX = dir.x > 0.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (!player.IsValid())
            return;

        if (!this.IsValid())
            return;

        UniTaskUtils.CancelTokenSource(ref _cancellationTokenSource);
        _cancellationTokenSource = new CancellationTokenSource();

        TaskStartDotDamage(player).Forget();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (!player.IsValid())
            return;

        if (!this.IsValid())
            return;

        UniTaskUtils.CancelTokenSource(ref _cancellationTokenSource);
    }

    public async UniTaskVoid TaskStartDotDamage(PlayerController target)
    {
        var token = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, destroyCancellationToken).Token;

        while (true)
        {
            target.OnDamaged(this, 2);
            await UniTask.Delay(1000, cancellationToken: token);
        }
    }

    protected override void OnDead()
    {
        base.OnDead();

        UniTaskUtils.CancelTokenSource(ref _cancellationTokenSource);

        var gemController = Managers.Object.Spawn<GemController>(transform.position, 0);

        Managers.Object.Despawn(this);

        Managers.Game.KillCount++;
    }

    private void OnDisable()
    {
        UniTaskUtils.CancelTokenSource(ref _cancellationTokenSource);
    }

    private void OnDestroy()
    {
        UniTaskUtils.CancelTokenSource(ref _cancellationTokenSource);
    }
}
