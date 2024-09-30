using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class PlayerController : CreatureController
{
    Vector2 _moveDir = Vector2.zero;

    #region Action
    public Action OnPlayerDataUpdated;
    public Action OnPlayerLevelUp;
    public Action OnPlayerDead;
    public Action OnPlayerDamaged;
    public Action OnPlayerMove;
    #endregion

    public override bool Init()
    {
        base.Init();

        moveSpeed = 5.0f;

        Managers.Game.OnMoveDirChanged += OnMoveDirChanged;
        return true;
    }

    private void OnDestroy()
    {
        Managers.Game.OnMoveDirChanged -= OnMoveDirChanged;
    }

    protected override void UpdateController()
    {
        UpdatePlayerDirection();
        MovePlayer();
    }



    private void OnMoveDirChanged(Vector2 vector)
    {
        _moveDir = vector;
    }

    void UpdatePlayerDirection()
    {
        if (_moveDir.x > 0)
            _creatureSprite.flipX = false;
        else
            _creatureSprite.flipX = true;
    }

    void MovePlayer()
    {
        if (creatureState == CreatureState.OnDamaged)
            return;

        _rigidBody.velocity = Vector2.zero;

        Vector3 dir = _moveDir * moveSpeed * Time.deltaTime;
        transform.position += dir;

        if (dir != Vector3.zero)
        {
            if (creatureState != CreatureState.Moving)
                creatureState = CreatureState.Moving;

            OnPlayerMove?.Invoke();
        }
        else
        {
            if (creatureState != CreatureState.Idle)
                creatureState = CreatureState.Idle;
            _rigidBody.velocity = Vector2.zero;
        }
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        _animator.Play("Idle");
        Debug.Log("Anim:Idle");
    }

    protected override void UpdateMoving()
    {
        base.UpdateMoving();

        _animator.Play("Move");
        Debug.Log("Anim:Move");
    }
}
