using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BounceArrowProjectileController : ProjectileController
{
    protected override void DoEnterTrigger(CreatureController creature)
    {
        _bounceCount--;
        BounceProjectile(creature);
        if (_bounceCount < 0)
        {
            _rigid.velocity = Vector3.zero;
            DestroyProjectile();
        }
    }

    void BounceProjectile(CreatureController creature)
    {
        List<Transform> list = new List<Transform>();
        list = Managers.Object.GetFindMonstersInFanShape(creature.CenterPosition, _dir, 5.5f, 240);

        List<Transform> sortedList = (from t in list
                                      orderby Vector3.Distance(t.position, transform.position) descending
                                      select t).ToList();

        if (sortedList.Count == 0)
        {
            DestroyProjectile();
        }
        else
        {
            int index = Random.Range(sortedList.Count / 2, sortedList.Count);
            _dir = (sortedList[index].position - transform.position).normalized;
            _rigid.velocity = _dir * Skill.SkillData.BounceSpeed;
        }
    }
}
