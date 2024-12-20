using Cysharp.Threading.Tasks.Triggers;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static Define;


public class PlayerController : CreatureController
{
    Vector2 _moveDir = Vector2.zero;
    CancellationTokenSource _cancellationToken = new CancellationTokenSource();

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
    
    public float ItemCollectRadius { get; } = 1.5f;

    public int Level
    {
        get { return Managers.Game.Level; }
        set { Managers.Game.Level = value; }
    }

    public float TotalExp
    {
        get { return Managers.Game.TotalExp; }
        set { Managers.Game.TotalExp = value; }
    }

    public float Exp { 
        get { return Managers.Game.Exp; } 
        set {
            Managers.Game.Exp = value;

            // 레벨업 체크
            int level = Level;
            while (true)
            {
                //만렙인경우 break;
                LevelUpExpData nextLevel;
                if (Managers.Data.LevelUpExpDic.TryGetValue(level + 1, out nextLevel) == false)
                    break;

                LevelUpExpData currentLevel;
                Managers.Data.LevelUpExpDic.TryGetValue(level, out currentLevel);
                if (Managers.Game.Exp < currentLevel.TotalExp)
                    break;
                level++;
            }

            if (level != Level)
            {
                Level = level;
                LevelUpExpData currentLevel;
                Managers.Data.LevelUpExpDic.TryGetValue(level, out currentLevel);
                TotalExp = currentLevel.TotalExp;
                LevelUp(Level);

            }

            OnPlayerDataUpdated?.Invoke();
        } }

    public float ExpRatio
    {
        get
        {
            LevelUpExpData currentLevelData;
            if (Managers.Data.LevelUpExpDic.TryGetValue(Level, out currentLevelData))
            {
                int currentLevelExp = currentLevelData.TotalExp;
                int nextLevelExp = currentLevelExp;
                int previousLevelExp = 0;

                LevelUpExpData prevLevelData;
                if (Managers.Data.LevelUpExpDic.TryGetValue(Level - 1, out prevLevelData))
                {
                    previousLevelExp = prevLevelData.TotalExp;
                }

                // 만렙이 아닌 경우
                LevelUpExpData nextLevelData;
                if (Managers.Data.LevelUpExpDic.TryGetValue(Level + 1, out nextLevelData))
                {
                    nextLevelExp = nextLevelData.TotalExp;
                }

                return (float)(Exp - previousLevelExp) / (currentLevelExp - previousLevelExp);
            }

            return 0f;
        }
    }

    public int KillCount
    {
        get { return Managers.Game.KillCount; }
        set
        {
            Managers.Game.KillCount = value;
            OnPlayerDataUpdated?.Invoke();
        }
    }

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
        CollectDropItem();
    }

    protected void CollectDropItem()
    { 
        List<DropItemController> items = Managers.Game.CurrentMap.Grid.GatherObjects(transform.position, ItemCollectRadius);

        foreach (DropItemController item in items)
        {
            Vector3 dir = item.transform.position - transform.position;
            switch (item.itemType)
            {
                // 아이템에 따라 수집 형태를 추가 가능
                case ObjectType.Gem :
                case ObjectType.Magnet:
                    float cd = item.CollectDistance;
                    if (dir.sqrMagnitude <= cd * cd)
                    {
                        item.GetItem();
                    }
                    break;
            }
        }
    }

    void LevelUp(int level = 0)
    {
        if (Level > 1)
            OnPlayerLevelUp?.Invoke();

        UpdateCreatureStat();
    }

    protected override void UpdateCreatureStat()
    {
        base.UpdateCreatureStat();

        InitCreatureStat(false);

        MaxHp *= MaxHpBonusRate;
        Hp *= MaxHpBonusRate;
        Atk *= AttackRate;
        Def *= DefRate;
        MoveSpeed *= MoveSpeedRate;
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

    public override void InitCreatureStat(bool isFullHp = true)
    {
        // 현재 케릭터의 Stat 가져오기
        MaxHp = Managers.Game.Character.MaxHp;
        Atk = Managers.Game.Character.Atk;
        MoveSpeed = creatureData.moveSpeed * creatureData.moveSpeedRate;

        MaxHp *= MaxHpBonusRate;
        Atk *= AttackRate;
        Def *= DefRate;
        MoveSpeed *= MoveSpeedRate;

        if (isFullHp == true)
            Hp = MaxHp;
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

    public void StopAllTask()
    { 
        _cancellationToken.Cancel();
    }

    public override void OnDamaged(BaseController attacker, SkillBase skill = null, float damage = 0)
    {
        base.OnDamaged(attacker, skill, damage);


    }

    protected override void OnDead()
    {
        base.OnDead();
        OnPlayerDead?.Invoke();
    }
}
