using UnityEngine;

public class IcicleArrow : RepeatSkill
{
    private void Awake()
    {
        SkillType = Define.SkillType.IcicleArrow;
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
        string prefabName = SkillData.PrefabLabel;

        if (Managers.Game.Player != null)
        {
            Vector3 startPos = Managers.Game.Player.PlayerCenterPos;
            Vector3 dir = Managers.Game.Player.PlayerDirection;
            for (int i = 0; i < SkillData.NumProjectiles; i++)
            {
                // °¹¼ö * °¢µµ·Î ÇÏµÇ, ÁÂ¿ì ´ëÄªÀÌ µÇµµ·Ï
                float angle = SkillData.AngleBetweenProject * (i - (SkillData.NumProjectiles - 1) / 2f);
                Vector3 res = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                var pc = GenerateProjectile(Managers.Game.Player, prefabName, startPos, res.normalized, Vector3.zero, this);
                var particles = pc.GetComponentsInChildren<ParticleSystem>();
                foreach (var particle in particles)
                { 
                    particle.Clear();
                    particle.Play();
                }
            }
        }
    }
}