using Cysharp.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;
using System.Collections;

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
            case Define.SkillType.BounceArrow:
                {
                    _bounceCount = Skill.SkillData.NumBounce;
                    _rigid.velocity = _dir * Skill.SkillData.ProjectileSpeed;
                }
                break;
            case Define.SkillType.SpinCutter:
                if (gameObject.activeInHierarchy)
                    WindCutterTask().Forget();
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
            case Define.SkillType.BounceArrow:
                {
                    _bounceCount--;
                    BounceProjectile(creature);
                    if (_bounceCount < 0)
                    {
                        _rigid.velocity = Vector3.zero;
                        DestroyProjectile();
                    }
                }
                break;
            case Define.SkillType.SpinCutter:
                _enteredColliderList.Add(creature);
                if (_dotSrc == null)
                    StartDotDamageTask(Define.ONE_SECOND_TO_MILLISEC).Forget();
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

            for (int i = 0; i < _enteredColliderList.Count; ++i)
            {
                if (_enteredColliderList.Count <= i)
                    continue;

                var target = _enteredColliderList[i];

                if(target.IsValid())
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

    void BounceProjectile(CreatureController creature)
    {
        List<Transform> list = new List<Transform>();
        list = Managers.Object.GetFindMonstersInFanShape(creature.CenterPosition, _dir, 5.5f, 240);

        List<Transform> sortedList = (from t in list
                                      orderby Vector3.Distance(t.position, transform.position) descending
                                      select t).ToList();

        if (sortedList.Count == 0)
        {
            DestroyProjectile();
        }
        else
        {
            int index = Random.Range(sortedList.Count / 2, sortedList.Count);
            _dir = (sortedList[index].position - transform.position).normalized;
            _rigid.velocity = _dir * Skill.SkillData.BounceSpeed;
        }
    }

    async UniTask WindCutterTask()
    {
        Vector3 targePoint = Managers.Game.Player.PlayerCenterPos + _dir * Skill.SkillData.ProjectileSpeed;
        transform.localScale = Vector3.zero;
        transform.localScale = Vector3.one * Skill.SkillData.ScaleMultiplier;

        Sequence seq = DOTween.Sequence();
        // 1. 목표지점까지 빠르게 도착
        // 2. 도착수 약간 더 전진
        // 3. 되돌아옴

        float projectileTravelTime = 1f; // 발사체가 목표지점까지 가는데 걸리는시간
        float secondSeqStartTime = 0.7f; // 두번쨰 시퀀스 시작시간
        float secondSeqDuringTime = 1.8f; //두번째 시퀀스 유지시간

        seq.Append(transform.DOMove(targePoint, projectileTravelTime).SetEase(Ease.OutExpo))
            .Insert(secondSeqStartTime, transform.DOMove(targePoint + _dir, secondSeqDuringTime).SetEase(Ease.Linear));

        await UniTask.Delay((int)(Skill.SkillData.Duration * 1000.0f));

        while (true)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, Managers.Game.Player.PlayerCenterPos, Time.deltaTime * Skill.SkillData.ProjectileSpeed * 4f);
            if (Managers.Game.Player.PlayerCenterPos == transform.position)
            {
                DestroyProjectile();
                break;
            }

            await UniTask.WaitForFixedUpdate();
        }
    }
}
