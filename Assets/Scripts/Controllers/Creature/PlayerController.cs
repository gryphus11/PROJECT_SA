using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

        CreatureState = CreatureState.Idle;
        // 방향 콜백 등록
        Managers.Game.onMoveDirChanged += OnMoveDirChanged;
        return true;
    }

    private void Start()
    {
        InitSkill();
    }

    private void OnDestroy()
    {
        // 방향 콜백 해제
        Managers.Game.onMoveDirChanged -= OnMoveDirChanged;
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
        else if (_moveDir.x < 0)
            _creatureSprite.flipX = true;
    }

    void MovePlayer()
    {
        if (CreatureState == CreatureState.OnDamaged)
            return;

        _rigidBody.velocity = Vector2.zero;

        Vector3 dir = _moveDir * MoveSpeed * Time.deltaTime;
        transform.position += dir;

        if (dir != Vector3.zero)
        {
            if (CreatureState != CreatureState.Moving)
                CreatureState = CreatureState.Moving;

            Indicator.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
            OnPlayerMove?.Invoke();
        }
        else
        {
            if (CreatureState != CreatureState.Idle)
                CreatureState = CreatureState.Idle;
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

    public override void InitCreatureStat()
    {
        base.InitCreatureStat();

        // 현재 케릭터의 Stat 가져오기
        MaxHp = creatureData.maxHp + (creatureData.maxHp * creatureData.hpRate * 0.01f);
        Hp = MaxHp;
        
        Atk = creatureData.atk + (creatureData.atk * creatureData.atkRate * 0.01f);
        MoveSpeed = creatureData.moveSpeed + (creatureData.moveSpeed * creatureData.moveSpeedRate * 0.01f);
    }

    protected override void InitSkill()
    {
        base.InitSkill();
        var skillType = SkillData.GetSkillTypeFromInt(creatureData.defaultSkill);
        if (Skills.HasSkill(skillType))
        {
            Skills.LevelUpSkill(skillType);
        }
        else
        {
            Skills.AddSkill(skillType);
            Skills.LevelUpSkill(SkillData.GetSkillTypeFromInt(creatureData.defaultSkill));
        }
    }
}
