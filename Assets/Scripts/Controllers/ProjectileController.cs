using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileController : SkillBase
{
    public SkillBase Skill { get; set; }
    Vector2 _spawnPos;
    Vector3 _dir = Vector3.zero;
    Vector3 _target = Vector3.zero;
    Define.SkillType _skillType;
    Rigidbody2D _rigid;
    int _numPenetrations;
    public int _bounceCount = 1;

    List<CreatureController> _enteredColliderList = new List<CreatureController>();

    UniTaskCompletionSource _dotSrc = null;

    public void SetInfo(CreatureController owner, Vector2 position, Vector2 dir, Vector2 target, SkillBase skill)
    {
        Owner = owner;

        _spawnPos = position;
        _dir = dir;
        Skill = skill;
        _rigid = GetComponent<Rigidbody2D>();
        _target = target;
        transform.localScale = Vector3.one * Skill.SkillData.ScaleMultiplier;
        _numPenetrations = skill.SkillData.NumPenetrations;
        _bounceCount = skill.SkillData.NumBounce;


        // 투사체가 특정한 선행 동작이 필요하다면 여기에 구현
        switch (skill.SkillType)
        {
            case Define.SkillType.Lightning:
                {
                    LightningTask().Forget();
                    _rigid.velocity = Vector2.zero;
                }
                break;
            // 기본은 특정 방향으로 발사
            default:
                {
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, _dir);
                    _numPenetrations = Skill.SkillData.NumPenetrations;
                    _rigid.velocity = _dir * Skill.SkillData.ProjectileSpeed;
                }
                break;
        }

        if (gameObject.activeInHierarchy)
            DestroyTask().Forget();

    }

    private async UniTask DestroyTask()
    {
        CancellationTokenSource src = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, destroyCancellationToken);

        while (true)
        {
            await UniTask.Delay(5000, cancellationToken: src.Token);
            DestroyProjectile();
        }
    }

    public void DestroyProjectile()
    {
        Managers.Object.Despawn(this);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        CreatureController creature = collision.transform.GetComponent<CreatureController>();
        if (creature.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;

        _enteredColliderList.Add(creature);

        // 투사체가 대상에 히트했을 때의 동작을 정의
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
        // 트리거 충돌이 끝날 때까지 작용을 해야하는 스킬의 동작

        CreatureController target = collision.transform.GetComponent<CreatureController>();
        if (target.IsValid() == false)
            return;

        if (this.IsValid() == false)
            return;

        _enteredColliderList.Remove(target);

        if (_enteredColliderList.Count == 0 && _dotSrc != null)
        {
            _dotSrc.TrySetResult();
            _dotSrc = null;
        }
    }

    async UniTask StartDotDamageTask(int dotMilliSec)
    {
        _dotSrc = new UniTaskCompletionSource();

        while (true)
        {
            await UniTask.Delay(dotMilliSec);

            foreach (CreatureController target in _enteredColliderList)
            {
                target.OnDamaged(Owner, Skill);
            }
        }
    }

    async UniTask LightningTask()
    {
        await UniTask.Delay(100);
        RaycastHit2D[] _targets = Physics2D.CircleCastAll(transform.position, 3.0f, Vector2.zero, 0);
        foreach (RaycastHit2D _target in _targets)
        {
            CreatureController creature = _target.transform.GetComponent<CreatureController>();
            if (creature?.IsMonster == true)
                creature.OnDamaged(Owner, Skill);
        }
        await UniTask.Delay(500);
        DestroyProjectile();
    }
}
