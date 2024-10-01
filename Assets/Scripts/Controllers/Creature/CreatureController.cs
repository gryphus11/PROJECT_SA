using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;
using static Define;


public abstract class CreatureController : BaseController
{
    [SerializeField]
    protected SpriteRenderer _creatureSprite;
    protected string _spriteName;
    
    public Rigidbody2D _rigidBody { get; set; }
    public Animator _animator{ get; set; }
    private AnimatorOverrideController _overrideController;

    public virtual int dataId { get; set; }
    public virtual float hp { get; set; }
    public virtual float maxHp { get; set; }
    public virtual float maxHpBonusRate { get; set; } = 1;
    public virtual float healBonusRate { get; set; } = 1;
    public virtual float hpRegen { get; set; }
    public virtual float atk { get; set; }
    public virtual float attackRate { get; set; } = 1;
    public virtual float def { get; set; }
    public virtual float defRate { get; set; }
    public virtual float criRate { get; set; }
    public virtual float criDamage { get; set; } = 1.5f;
    public virtual float damageReduction { get; set; }
    public virtual float moveSpeedRate { get; set; } = 1;
    public virtual float moveSpeed { get; set; }

    public CreatureData creatureData;

    public Vector3 centerPosition
    {
        get
        {
            return _offset.bounds.center;
        }
    }


    private Collider2D _offset;
    public Vector3 CenterPosition
    {
        get
        {
            return _offset.bounds.center;
        }
    }


    Define.CreatureState _creatureState = Define.CreatureState.Moving;

    public virtual Define.CreatureState creatureState
    {
        get { return _creatureState; }
        set
        {
            _creatureState = value;
            UpdateAnimation();
        }
    }

    void Awake()
    {
        Init();
    }

    public override bool Init()
    {
        base.Init();

        _animator = GetComponent<Animator>();
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            // Animator�� �⺻ Runtime Animator Controller�� ������
            RuntimeAnimatorController originalController = _animator.runtimeAnimatorController;

            // AnimatorOverrideController ����
            _overrideController = new AnimatorOverrideController(originalController);
            _overrideController.name = $"{originalController.name}";

            // Animator�� ���ο� Override Controller �Ҵ�
            _animator.runtimeAnimatorController = _overrideController;
        }

        _offset = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _creatureSprite = GetComponent<SpriteRenderer>();
        if (_creatureSprite == null)
            _creatureSprite = Utils.FindChild<SpriteRenderer>(gameObject);

        return true;
    }

    public virtual void UpdateAnimation() 
    {
        switch (creatureState)
        {
            case Define.CreatureState.Idle:
                UpdateIdle();
                break;
            case Define.CreatureState.Skill:
                UpdateSkill();
                break;
            case Define.CreatureState.Moving:
                UpdateMoving();
                break;
            case Define.CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateDead() { }
    
    public void OverrideAnimation(string stateName, AnimationClip newClip)
    {
        if (_overrideController == null || newClip == null)
        {
            Debug.LogWarning("�ִϸ��̼� ��ü ����: OverrideController �Ǵ� ���ο� Ŭ���� null�Դϴ�.");
            return;
        }

        if (!_overrideController[stateName])
        {
            Debug.LogWarning($"���� {stateName}�� �ش��ϴ� �ִϸ��̼� Ŭ���� �����ϴ�.");
            return;
        }

        // ���� �ִϸ��̼� Ŭ���� ���ο� Ŭ������ ��ü
        _overrideController[stateName] = newClip;

        // Animator�� ���� ���� ����
        _animator.Rebind();         // ��Ÿ�� �� ���� ���� �ݿ�
        _animator.Update(0);        // ��� ���� ���� ����

        Debug.Log($"�ִϸ��̼� {stateName}��(��) {newClip.name}���� ��ü�Ǿ����ϴ�.");

        UpdateAnimation();
    }
    
    public void SetInfo(int creatureId)
    {
        dataId = creatureId;
        Dictionary<int, Data.CreatureData> dict = Managers.Data.CreatureDic;
        creatureData = dict[creatureId];
        
        InitCreatureStat();
        Init();
        SetAnimation();
    }

    public virtual void InitCreatureStat()
    {

    }

    [ContextMenu("�ִ� �缳��")]
    private void SetAnimation()
    {
        var clips = creatureData.animationLabels.Split("|");

        foreach (var clip in clips)
        {
            AnimationClip animationClip = Managers.Resource.Load<AnimationClip>(clip);

            if (animationClip != null)
            {
                // �ִϸ��̼� ��ü, clip �̸��� �������� stateName ����
                string stateName = clip.Split('_')[0]; // ��: "Idle_clip1" -> "Idle"
                OverrideAnimation(stateName, animationClip);
            }
            else
            {
                Debug.LogWarning($"�ִϸ��̼� Ŭ�� {clip}��(��) ã�� �� �����ϴ�.");
            }
        }
    }

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        if (hp <= 0)
            return;

        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {

    }
}
