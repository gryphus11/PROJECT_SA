using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static Define;


public abstract class CreatureController : BaseController
{
    [SerializeField]
    protected SpriteRenderer _creatureSprite;
    protected string _spriteName;
    
    public Rigidbody2D _rigidBody { get; set; }
    public Animator _animator{ get; set; }

    public virtual int dataId { get; set; }
    public virtual float moveSpeed { get; set; }


    public Vector3 centerPosition
    {
        get
        {
            return _offset.bounds.center;
        }
    }


    private Collider2D _offset;
    
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
}
