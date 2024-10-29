using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceArrow : RepeatSkill
{
    private void Awake()
    {
        SkillType = Define.SkillType.BounceArrow;
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
    }

    public override void OnChangedSkillData()
    {

    }

    protected override void DoSkillJob()
    {
        SetBounceArrow().Forget();
    }

    async UniTask SetBounceArrow()
    {
        string prefabName = SkillData.PrefabLabel;

        if (Managers.Game.Player != null)
        {
            List<MonsterController> target = Managers.Object.GetMonsterWithinCamera(SkillData.NumProjectiles);

            if (target == null)
                return;

            for (int i = 0; i < target.Count; i++)
            {
                if (target != null)
                {
                    if (target[i].IsValid() == false)
                        continue;
                    Vector3 dir = target[i].CenterPosition - Managers.Game.Player.CenterPosition;
                    Vector3 startPos = Managers.Game.Player.CenterPosition;
                    GenerateProjectile<BounceArrowProjectileController>(Managers.Game.Player, prefabName, startPos, dir.normalized, Vector3.zero, this);
                }

                await UniTask.Delay((int)(SkillData.ProjectileSpacing * 1000.0f));
            }
        }
    }
}
