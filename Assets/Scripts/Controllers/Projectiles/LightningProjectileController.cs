using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LightningProjectileController : ProjectileController
{
    CancellationTokenSource _lightningCTS;

    protected override void OnSetInfo(CreatureController owner, Vector2 position, Vector2 dir, Vector2 target, SkillBase skill)
    {
        UniTaskUtils.CancelTokenSource(ref _lightningCTS);
        _rigid.velocity = Vector2.zero;
        LightningTask().Forget();
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
    }
    
    async UniTask LightningTask()
    {
        _lightningCTS = new CancellationTokenSource();

        await UniTask.Delay(100, cancellationToken: _lightningCTS.Token);
        RaycastHit2D[] _targets = Physics2D.CircleCastAll(transform.position, 3.0f, Vector2.zero, 0);
        foreach (RaycastHit2D _target in _targets)
        {
            CreatureController creature = _target.transform.GetComponent<CreatureController>();
            if (creature?.IsMonster == true)
                creature.OnDamaged(Owner, Skill);
        }
        await UniTask.Delay(500, cancellationToken: _lightningCTS.Token);

        _lightningCTS.Cancel();
        DestroyProjectile();
    }
}
