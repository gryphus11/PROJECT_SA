using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sanctuary : RepeatSkill
{
    private HashSet<CreatureController> _targets = new HashSet<CreatureController>();
    UniTaskCompletionSource _dotCompletionSrc = null;

    private void Awake()
    {
        base.Init();
        SkillType = Define.SkillType.Sanctuary;
        gameObject.SetActive(false);
    }

    public override void ActivateSkill()
    {
        base.ActivateSkill();
        gameObject.SetActive(true);

        OnChangedSkillData();
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();

        OnChangedSkillData();
    }

    public override void OnChangedSkillData()
    {
        transform.localScale = Vector3.one * SkillData.ScaleMultiplier;
    }

    public void Update()
    {
        transform.position = Managers.Game.Player.PlayerCenterPos;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        CreatureController target = collision.transform.GetComponent<CreatureController>();

        if (target.IsValid() == false)
            return;

        if (target?.IsMonster == false)
            return;

        _targets.Add(target);

        target.OnDamaged(Managers.Game.Player, this);

        if (_dotCompletionSrc == null)
            StartDotDamageTask().Forget();
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        CreatureController target = collision.transform.GetComponent<CreatureController>();
        if (target.IsValid() == false)
            return;

        _targets.Remove(target);

        if (_targets.Count == 0 && _dotCompletionSrc != null)
        {
            _dotCompletionSrc.TrySetResult();
            _dotCompletionSrc = null;
        }
    }

    protected async UniTask StartDotDamageTask()
    {
        _dotCompletionSrc = new UniTaskCompletionSource();

        while (true)
        {
            await UniTask.Delay(300);

            List<CreatureController> list = _targets.ToList();

            foreach (CreatureController target in list)
            {
                if (target.IsValid() == false || target.gameObject.IsValid() == false)
                {
                    _targets.Remove(target);
                    continue;
                }
                target.OnDamaged(Managers.Game.Player, this);
            }
        }
    }


    protected override void DoSkillJob()
    {

    }
}
