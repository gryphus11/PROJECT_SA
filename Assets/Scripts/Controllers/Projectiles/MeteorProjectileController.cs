using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MeteorProjectileController : ProjectileController
{
    GameObject _meteorShadow;
    CancellationTokenSource _shadowScaleCTS;

    protected override void OnSetInfo(CreatureController owner, Vector2 position, Vector2 dir, Vector2 target, SkillBase skill)
    {
        UniTaskUtils.CancelTokenSource(ref _shadowScaleCTS);

        _dir = (_target - transform.position).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, _dir);
        _rigid.velocity = _dir * Skill.SkillData.ProjectileSpeed;
        _meteorShadow = Managers.Resource.Instantiate("MeteorShadow", pooling: true);
        _meteorShadow.transform.position = target;
        if (gameObject.activeInHierarchy)
        {
            MeteorTask().Forget();
        }
    }

    async UniTask MeteorTask()
    {
        _shadowScaleCTS = new CancellationTokenSource();

        while (true)
        {
            if (_meteorShadow != null)
            {
                Vector2 shadowPosition = _meteorShadow.transform.position;

                float distance = Vector2.Distance(shadowPosition, transform.position);
                float scale = Mathf.Lerp(0f, 2.5f, 1 - distance / 10f);
                _meteorShadow.transform.position = shadowPosition;
                _meteorShadow.transform.localScale = new Vector3(scale, scale, 1f);
            }

            if (Vector2.Distance(transform.position, _target) < 0.3f)
                ExplosionMeteor();

            await UniTask.WaitForFixedUpdate(cancellationToken: _shadowScaleCTS.Token);
        }
    }

    void ExplosionMeteor()
    {
        Managers.Resource.Destroy(_meteorShadow);
        float scanRange = 1.5f;
        string prefabName = "MeteorExplosion";
        GameObject obj = Managers.Resource.Instantiate(prefabName, pooling: true);
        obj.transform.position = transform.position;

        RaycastHit2D[] _targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0);

        foreach (RaycastHit2D _target in _targets)
        {
            CreatureController creature = _target.transform.GetComponent<CreatureController>();
            if (creature?.IsMonster == true)
                creature.OnDamaged(Owner, Skill);
        }

        _shadowScaleCTS.Cancel();

        DestroyProjectile();
    }
}
