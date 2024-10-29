using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SpinCutterProjectileController : ProjectileController
{
    CancellationTokenSource _spinCutterCTS;
    protected List<CreatureController> _enteredColliderList = new List<CreatureController>();

    protected UniTaskCompletionSource _dotSrc = null;

    protected override void OnSetInfo(CreatureController owner, Vector2 position, Vector2 dir, Vector2 target, SkillBase skill)
    {
        UniTaskUtils.CancelTokenSource(ref _spinCutterCTS);
        if (gameObject.activeInHierarchy)
            SpinCutterTask().Forget();
    }

    async UniTask SpinCutterTask()
    {
        _spinCutterCTS = new CancellationTokenSource();
        CancellationTokenSource linkCts = CancellationTokenSource.CreateLinkedTokenSource(_spinCutterCTS.Token, destroyCancellationToken);

        Vector3 targePoint = Managers.Game.Player.PlayerCenterPos + _dir * Skill.SkillData.ProjectileSpeed;
        transform.localScale = Vector3.zero;
        transform.localScale = Vector3.one * Skill.SkillData.ScaleMultiplier;

        Sequence seq = DOTween.Sequence();
        // 1. 목표지점까지 빠르게 도착
        // 2. 도착 후 약간 더 전진
        // 3. 되돌아옴

        float projectileTravelTime = 1f; // 발사체가 목표지점까지 가는데 걸리는시간
        float secondSeqStartTime = 0.7f; // 두번쨰 시퀀스 시작시간
        float secondSeqDuringTime = 1.8f; //두번째 시퀀스 유지시간

        seq.Append(transform.DOMove(targePoint, projectileTravelTime).SetEase(Ease.OutExpo))
            .Insert(secondSeqStartTime, transform.DOMove(targePoint + _dir, secondSeqDuringTime).SetEase(Ease.Linear));

        await UniTask.Delay(100 + (int)(Skill.SkillData.Duration * 1000.0f), cancellationToken: linkCts.Token);

        while (true)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, Managers.Game.Player.PlayerCenterPos, Time.deltaTime * Skill.SkillData.ProjectileSpeed * 4f);
            if (Managers.Game.Player.PlayerCenterPos == transform.position)
            {
                _spinCutterCTS.Cancel();
                DestroyProjectile();
                break;
            }

            await UniTask.WaitForFixedUpdate(cancellationToken: linkCts.Token);
        }
    }

    protected override void DoEnterTrigger(CreatureController creature)
    {
        _enteredColliderList.Add(creature);
        if (_dotSrc == null)
            StartDotDamageTask(Define.ONE_SECOND_TO_MILLISEC).Forget();
    }

    protected override void DoExitTrigger(CreatureController creature)
    {
        _enteredColliderList.Remove(creature);

        if (_enteredColliderList.Count == 0 && _dotSrc != null)
        {
            _dotSrc.TrySetResult();
            _dotSrc = null;
        }
    }

    async UniTask StartDotDamageTask(int dotMilliSec)
    {
        _dotSrc = new UniTaskCompletionSource();
        CancellationTokenSource linkCts = CancellationTokenSource.CreateLinkedTokenSource(_spinCutterCTS.Token, destroyCancellationToken);

        while (true)
        {
            await UniTask.Delay(dotMilliSec, cancellationToken: linkCts.Token);

            for (int i = 0; i < _enteredColliderList.Count; ++i)
            {
                if (_enteredColliderList.Count <= i)
                    continue;

                var target = _enteredColliderList[i];

                if (target.IsValid())
                    target.OnDamaged(Owner, Skill);
            }
        }
    }
}
