using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;
using static Define;

public class SkillBase : BaseController
{
    public CreatureController Owner { get; set; }

    #region 프로퍼티

    public SkillType SkillType { get; set; }

    public int Level { get; set; }

    public SkillData SkillData { get; set; }

    public float TotalDamage { get; set; } = 0;
    public bool IsLearnedSkill { get { return Level > 0; } }
    #endregion

    public Data.SkillData UpdateSkillData(int dataId = 0)
    {
        int id = 0;

        // 스킬 레벨이 1 이하면 스킬 타입을 아이디로, 초과면 레벨-1을 아이디로 설정
        if (dataId == 0)
            id = Level < 2 ? (int)SkillType : (int)SkillType + Level - 1;
        else
            id = dataId;

        if (Managers.Data.SkillDic.TryGetValue(id, out var skillData))
        {
            SkillData = skillData;
            OnChangedSkillData();
            return SkillData;
        }
        else
        {
            return SkillData;
        }
    }

    public virtual void OnChangedSkillData() { }

    public virtual void ActivateSkill()
    {
        UpdateSkillData();
    }

    public virtual void OnLevelUp()
    {
        if (Level == 0)
            ActivateSkill();
        Level++;
        UpdateSkillData();
    }

    protected virtual void GenerateProjectile(CreatureController Owner, string prefabName, Vector3 startPos, Vector3 dir, Vector3 targetPos, SkillBase skill)
    {
        ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, prefabName: prefabName);
        pc.SetInfo(Owner, startPos, dir, targetPos, skill);
    }

    protected CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
    
    public virtual void StopSkill()
    {
        _cancelTokenSource.Cancel();
    }

    private void OnEnable()
    {
        UniTaskUtils.CancelTokenSource(ref _cancelTokenSource);
        _cancelTokenSource = new CancellationTokenSource();
    }

    protected virtual void OnDisable()
    {
        _cancelTokenSource.Cancel();
    }
}
