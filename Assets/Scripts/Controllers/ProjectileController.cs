using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileController : SkillBase
{
    public SkillBase Skill;
    Vector2 _spawnPos;
    Vector3 _dir = Vector3.zero;
    Vector3 _target = Vector3.zero;
    Define.SkillType _skillType;
    Rigidbody2D _rigid;
    int _numPenetrations;
    public int _bounceCount = 1;
    GameObject _meteorShadow;

    List<CreatureController> _enteredColliderList = new List<CreatureController>();

    private CancellationTokenSource _disableCts = new CancellationTokenSource();
    private UniTaskCompletionSource _dotDamageSrc = null;

    public void SetInfo(CreatureController owner, Vector2 position, Vector2 dir, Vector2 target, SkillBase skill)
    {
        Owner = owner;

        _spawnPos = position;
        _dir = dir;
        Skill = skill;
        _rigid = GetComponent<Rigidbody2D>();
        _target = target;
        transform.localScale = Vector3.one * Skill.SkillData.ScaleMultiplier;
        _numPenetrations = skill.SkillData.NumPenerations;
        _bounceCount = skill.SkillData.NumBounce;


        switch (skill.SkillType)
        {
            case Define.SkillType.IcicleArrow:
                break;
            case Define.SkillType.ThunderStorm:
                break;
            case Define.SkillType.Slash:
                break;

        }

        if (gameObject.activeInHierarchy)
            DestroyTask().Forget();

    }

    private async UniTask DestroyTask()
    {
        CancellationTokenSource src = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, destroyCancellationToken);

        while (true)
        {
            await UniTask.Delay(7000, cancellationToken: src.Token);
            DestroyProjectile();
        }
    }

    public void DestroyProjectile()
    {
        Managers.Object.Despawn(this);
        //
    }

    private async UniTask StartDotDamageTask()
    {
        _dotDamageSrc = new UniTaskCompletionSource();
        CancellationTokenSource src = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, destroyCancellationToken);
        
        while (true)
        {
            await UniTask.Delay(1000, cancellationToken: src.Token);

            foreach (CreatureController target in _enteredColliderList)
            {
                target.OnDamaged(Owner, Skill);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CreatureController creature = collision.transform.GetComponent<CreatureController>();
        if (creature.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        switch (Skill.SkillType)
        {
            case Define.SkillType.IcicleArrow:
                _numPenetrations--;
                if (_numPenetrations < 0)
                {
                    _rigid.velocity = Vector3.zero;
                    DestroyProjectile();
                }
                break;
            default:
                break;
        }
        creature.OnDamaged(Owner, Skill);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        CreatureController target = collision.transform.GetComponent<CreatureController>();
        if (target.IsValid() == false)
            return;

        if (this.IsValid() == false)
            return;

        _enteredColliderList.Remove(target);

        if (_enteredColliderList.Count == 0 && _dotDamageSrc != null)
        {
            _dotDamageSrc.TrySetResult();
            _dotDamageSrc = null;
        }
    }

    private void OnDisable()
    {
        UniTaskUtils.CancelTokenSource(ref _disableCts);
        _disableCts = new CancellationTokenSource();
    }
}
