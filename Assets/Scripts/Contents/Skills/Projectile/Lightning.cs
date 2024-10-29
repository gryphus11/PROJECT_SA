using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : RepeatSkill
{
    private void Awake()
    {
        SkillType = Define.SkillType.Lightning;

    }

    async UniTask GenerateLightning()
    {
        List<MonsterController> targets = Managers.Object.GetMonsterWithinCamera(SkillData.NumProjectiles);
        if (targets == null)
            return;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].IsValid() == true)
            {
                GenerateProjectile<LightningProjectileController>(Managers.Game.Player, SkillData.PrefabLabel, targets[i].CenterPosition, Vector3.zero, targets[i].CenterPosition, this);
                await UniTask.Delay((int)(SkillData.AttackInterval * 1000));
            }
        }
    }

    protected override void DoSkillJob()
    {
        GenerateLightning().Forget();
    }
}
