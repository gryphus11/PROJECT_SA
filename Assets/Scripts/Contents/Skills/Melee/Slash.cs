using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Slash : RepeatSkill
{
    [SerializeField]
    private List<ParticleSystem> _swingParticles = new List<ParticleSystem>();

    [SerializeField]
    private List<OverlapCollider2D> _overlapColliders = new List<OverlapCollider2D>();

    float _radian;

    protected enum SwingType
    {
        First,
        Second,
        Third,
    }

    public override bool Init()
    {
        base.Init();
        SkillType = Define.SkillType.Slash;

        for (int i = 0; i < transform.childCount; ++i)
        {
            var particleSystem = transform.GetChild(i).GetComponent<ParticleSystem>();
            var collider = transform.GetChild(i).GetComponent<Collider2D>();

            if (collider != null)
            {
                var overlap = new OverlapCollider2D { targetCollider = collider };
                overlap.contactFilter.SetLayerMask(LayerMask.GetMask("Monster"));
                _overlapColliders.Add(overlap);
            }

            if (particleSystem == null)
                continue;

            _swingParticles.Add(particleSystem);
        }

        return true;
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();
        transform.localScale = Vector3.one * SkillData.ScaleMultiplier;
    }

    protected override async UniTask StartSkill()
    {
        _startSkill = new UniTaskCompletionSource();
        CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, destroyCancellationToken);

        var swingTypes = (SwingType[])System.Enum.GetValues(typeof(SwingType));
        while (true)
        {
            // 모든 스윙 타입에 대해 동일한 작업 수행
            foreach (SwingType swingType in swingTypes)
            {
                int index = (int)swingType;

                SetParticles(index);
                var particle = _swingParticles[index];
                particle.gameObject.SetActive(true);
                var targets = _overlapColliders[index].GetObjectsInCollider();

                foreach (var target in targets)
                {
                    CreatureController creature = target.GetComponent<CreatureController>();
                    if (creature.IsValid() == false)
                        continue;

                    if (creature.IsMonster)
                    {
                        creature.OnDamaged(Managers.Game.Player, this);
                    }
                }

                float duration = particle.main.duration;
                await UniTask.Delay((int)((duration + SkillData.AttackInterval) * 1000), cancellationToken: cts.Token);
                particle.gameObject.SetActive(false);
            }

            await UniTask.Delay((int)(SkillData.CoolTime * 1000), cancellationToken: cts.Token);
        }
    }

    void SetParticles(int swingType)
    {
        Vector3 tempAngle = Managers.Game.Player.Indicator.transform.eulerAngles;
        transform.localEulerAngles = tempAngle;
        transform.position = Managers.Game.Player.PlayerCenterPos;

        _radian = Mathf.Deg2Rad * tempAngle.z * -1;

        var main = _swingParticles[swingType].main;
        main.startRotation = _radian;
    }

    public override void OnChangedSkillData()
    {
        transform.localScale = Vector3.one * SkillData.ScaleMultiplier;
    }

    protected override void DoSkillJob()
    {

    }
}
