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
        // 1. ��ǥ�������� ������ ����
        // 2. ���� �� �ణ �� ����
        // 3. �ǵ��ƿ�

        float projectileTravelTime = 1f; // �߻�ü�� ��ǥ�������� ���µ� �ɸ��½ð�
        float secondSeqStartTime = 0.7f; // �ι��� ������ ���۽ð�
        float secondSeqDuringTime = 1.8f; //�ι�° ������ �����ð�

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
