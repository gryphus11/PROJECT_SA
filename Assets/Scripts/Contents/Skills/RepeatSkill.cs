using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class RepeatSkill : SkillBase
{
    public override bool Init()
    {
        base.Init();
        return true;
    }

    #region 스킬 동작
    UniTaskCompletionSource _startSkill;
    CancellationTokenSource _disableCts = new CancellationTokenSource();

    public override void ActivateSkill()
    {
        base.ActivateSkill();
        
        if (_startSkill != null)
        {
            _startSkill.TrySetResult();
            _startSkill = null;
        }

        gameObject.SetActive(true);
        StartSkill().Forget();
    }

    protected abstract void DoSkillJob();

    protected virtual async UniTask StartSkill()
    {
        _startSkill = new UniTaskCompletionSource();
        CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, destroyCancellationToken);

        // 데이터의 쿨타입에 따라 반복 호출됩니다.
        int coolTimeMilliSec = (int)(SkillData.CoolTime * 1000.0f);

        while (true)
        {
            await UniTask.Delay(coolTimeMilliSec, cancellationToken: cts.Token);

            if (SkillData.CoolTime != 0)
                Managers.Sound.Play(Define.Sound.Effect, SkillData.CastingSound);
            DoSkillJob();
        }
    }
    #endregion

    private void OnDisable()
    {
        UniTaskUtils.CancelTokenSource(ref _disableCts);
        _disableCts = new CancellationTokenSource();
    }
}
