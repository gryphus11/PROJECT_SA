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

    public virtual int DataId { get; set; }
    public virtual float Hp { get; set; }
    public virtual float MaxHp { get; set; }
    public virtual float MaxHpBonusRate { get; set; } = 1;
    public virtual float HealBonusRate { get; set; } = 1;
    public virtual float HpRegen { get; set; }
    public virtual float Atk { get; set; }
    public virtual float AttackRate { get; set; } = 1;
    public virtual float Def { get; set; }
    public virtual float DefRate { get; set; }
    public virtual float CriRate { get; set; }
    public virtual float CriDamage { get; set; } = 1.5f;
    public virtual float DamageReduction { get; set; }
    public virtual float MoveSpeedRate { get; set; } = 1;
    public virtual float MoveSpeed { get; set; }

    public CreatureData creatureData;

    private Collider2D _offset;
    public Vector3 CenterPosition
    {
        get
        {
            return _offset.bounds.center;
        }
    }


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
    #endregion

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
            // Animator의 기본 Runtime Animator Controller를 가져옴
            RuntimeAnimatorController originalController = _animator.runtimeAnimatorController;

            // AnimatorOverrideController 생성
            _overrideController = new AnimatorOverrideController(originalController);
            _overrideController.name = $"{originalController.name}";

            // Animator에 새로운 Override Controller 할당
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
        switch (CreatureState)
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
            Debug.LogWarning("애니메이션 교체 실패: OverrideController 또는 새로운 클립이 null입니다.");
            return;
        }

        if (!_overrideController[stateName])
        {
            Debug.LogWarning($"상태 {stateName}에 해당하는 애니메이션 클립이 없습니다.");
            return;
        }

        // 기존 애니메이션 클립을 새로운 클립으로 교체
        _overrideController[stateName] = newClip;

        // Animator에 변경 사항 적용
        _animator.Rebind();         // 런타임 중 변경 사항 반영
        _animator.Update(0);        // 즉시 변경 사항 적용

        Debug.Log($"애니메이션 {stateName}이(가) {newClip.name}으로 교체되었습니다.");

        UpdateAnimation();
    }
    
    public void SetInfo(int creatureId)
    {
        DataId = creatureId;
        Dictionary<int, Data.CreatureData> dict = Managers.Data.CreatureDic;
        creatureData = dict[creatureId];
        
        InitCreatureStat();
        Init();
        SetAnimation();
    }

    public virtual void InitCreatureStat()
    {

    }

    [ContextMenu("애니 재설정")]
    private void SetAnimation()
    {
        var clips = creatureData.animationLabels.Split("|");

        foreach (var clip in clips)
        {
            AnimationClip animationClip = Managers.Resource.Load<AnimationClip>(clip);

            if (animationClip != null)
            {
                // 애니메이션 교체, clip 이름을 기준으로 stateName 결정
                string stateName = clip.Split('_')[0]; // 예: "Idle_clip1" -> "Idle"
                OverrideAnimation(stateName, animationClip);
            }
            else
            {
                Debug.LogWarning($"애니메이션 클립 {clip}을(를) 찾을 수 없습니다.");
            }
        }
    }

    public virtual void OnDamaged(BaseController attacker, float damage)
    {
        if (Hp <= 0)
            return;

        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {

    }
}
