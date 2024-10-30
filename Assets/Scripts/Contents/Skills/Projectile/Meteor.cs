using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : RepeatSkill
{
    private void Awake()
    {
        SkillType = Define.SkillType.Meteor;
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
    }

    protected override void DoSkillJob()
    {
        GenerateMeteorTask().Forget();
    }

    async UniTask GenerateMeteorTask()
    {
        List<MonsterController> targets = Managers.Object.GetMonsterWithinCamera(SkillData.NumProjectiles);
        if (targets == null)
            return;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].IsValid() == true)
            {
                Vector2 startPos = GetMeteorPosition(targets[i].CenterPosition);
                GenerateProjectile<MeteorProjectileController>(Managers.Game.Player, "Meteor", startPos, Vector3.zero, targets[i].CenterPosition, this);
                await UniTask.Delay(100 + (int)(SkillData.AttackInterval * 1000.0f));
            }
        }
    }

    public Vector2 GetMeteorPosition(Vector3 target)
    {
        float radian = 60.0f * Mathf.Deg2Rad;
        float spawnOffset = 1.0f;

        // 화면의 높이 절반
        float halfHeight = Camera.main.orthographicSize;
        
        // 화면의 너비 절반
        float halfWidth = Camera.main.aspect * halfHeight;

        // 타겟 위치에서 각도 방향에서 일정 거리 떨어진 곳에 생성하기 위함
        float spawnX = target.x + (halfWidth + spawnOffset) * Mathf.Cos(radian);
        float spawnY = target.y + (halfHeight + spawnOffset) * Mathf.Sin(radian);

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        return spawnPosition;
    }
}
