using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static Define;

public class SkillBase : BaseController
{
    public CreatureController Owner { get; set; }

    protected void HitEvent(Collider2D collision)
    {

    }

}
