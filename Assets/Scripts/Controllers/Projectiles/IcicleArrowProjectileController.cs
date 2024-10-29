using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleArrowProjectileController : ProjectileController
{
    protected override void DoEnterTrigger(CreatureController creature)
    {
        _numPenetrations--;
        if (_numPenetrations < 0)
        {
            _rigid.velocity = Vector3.zero;
            DestroyProjectile();
        }
    }
}
