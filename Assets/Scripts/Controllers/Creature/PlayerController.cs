using Cysharp.Threading.Tasks.Triggers;
using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
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

            // ������ üũ
            int level = Level;
            while (true)
            {
                //�����ΰ�� break;
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

    public override bool Init()
    {
        base.Init();

        CreatureState = CreatureState.Idle;
        // ���� �ݹ� ���
        Managers.Game.onMoveDirChanged += OnMoveDirChanged;
        return true;
    }

    private void Start()
    {
        InitSkill();
    }

    private void OnDestroy()
    {
        // ���� �ݹ� ����
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
                // �����ۿ� ���� ���� ���¸� �߰� ����
                case ObjectType.Gem :
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
        // ��������Ʈ ������
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
        // ���� �ɸ����� Stat ��������
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

    public Grid GridForGizmo { get; set; }

    private void OnDrawGizmos()
    {
        if (GridForGizmo == null)
            return;

        // ����� �׸� ��ġ�� �ݰ� ����
        Vector3 pos = transform.position;
        float radius = ItemCollectRadius;

        // �� �ڳ��� ��ġ ���
        Vector3Int left = GridForGizmo.WorldToCell(pos + new Vector3(-radius, 0.0f));
        Vector3Int right = GridForGizmo.WorldToCell(pos + new Vector3(radius, 0.0f));
        Vector3Int top = GridForGizmo.WorldToCell(pos + new Vector3(0.0f, radius));
        Vector3Int bottom = GridForGizmo.WorldToCell(pos + new Vector3(0.0f, -radius));

        int minX = left.x;
        int minY = bottom.y;
        int maxX = right.x;
        int maxY = top.y;

        // �׸��� ���� ��ĥ�ϴ� ����� ����
        Gizmos.color = Color.yellow;

        // minX ~ maxX, minY ~ maxY ������ ������ ��ȸ�ϸ� ����� �׸��� �� ��ġ�� �°� �׸���
        for (int x = minX; x <= maxX; ++x)
        {
            for (int y = minY; y <= maxY; ++y)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                Vector3 worldPos = GridForGizmo.CellToWorld(cellPos);

                // �׸��� ���� �߽� ��ġ ���
                Vector3 cellCenter = worldPos + GridForGizmo.cellSize / 2;

                // ���� ��踦 �׸��� �簢�� �׸���
                Gizmos.DrawWireCube(cellCenter, GridForGizmo.cellSize);
            }
        }

        // �÷��̾��� ��ġ�� �߽����� �ϴ� �� �׸���
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, radius);
    }
}
