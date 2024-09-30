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

    [SerializeField]
    public GameObject Indicator;
    [SerializeField]
    public GameObject IndicatorSprite;

    public Vector3 PlayerCenterPos { get { return Indicator.transform.position; } }
    public Vector3 PlayerDirection { get { return (IndicatorSprite.transform.position - PlayerCenterPos).normalized; } }

    public override bool Init()
    {
        base.Init();

        moveSpeed = 5.0f; // 임시 값 추후 캐릭터별 스테이터스에 따라 불러 올 예정
        creatureState = CreatureState.Idle;

        // 방향 콜백 등록
        Managers.Game.OnMoveDirChanged += OnMoveDirChanged;
        return true;
    }

    private void OnDestroy()
    {
        // 방향 콜백 해제
        Managers.Game.OnMoveDirChanged -= OnMoveDirChanged;
    }

    protected override void UpdateController()
    {
        UpdateSpriteDirection();
        MovePlayer();
    }



    private void OnMoveDirChanged(Vector2 vector)
    {
        _moveDir = vector;
    }

    void UpdateSpriteDirection()
    {
        // 스프라이트 뒤집기
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

            Indicator.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
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
    }

    protected override void UpdateMoving()
    {
        base.UpdateMoving();

        _animator.Play("Move");
    }
}
