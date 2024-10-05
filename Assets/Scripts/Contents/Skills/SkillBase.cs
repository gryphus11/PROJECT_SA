using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;
using static Define;

[System.Serializable]
public class OverlapCollider2D
{
    // ������ �ݶ��̴�
    public Collider2D targetCollider;

    // ���� ���� (���̾�, �浹 ���� ��)
    public ContactFilter2D contactFilter;

    public List<Collider2D> GetObjectsInCollider()
    {
        // ����� ������ ����Ʈ
        List<Collider2D> results = new List<Collider2D>();

        // targetCollider�� ��ġ�� �ݶ��̴����� ����
        targetCollider.OverlapCollider(contactFilter, results);

        return results;
    }
}

public class SkillBase : BaseController
{
    public CreatureController Owner { get; set; }

    #region ������Ƽ

    public SkillType SkillType { get; set; }

    public int Level { get; set; }

    public SkillData SkillData { get; set; }

    public float TotalDamage { get; set; } = 0;
    public bool IsLearnedSkill { get { return Level > 0; } }
    #endregion

    public Data.SkillData UpdateSkillData(int dataId = 0)
    {
        int id = 0;

        // ��ų ������ 1 ���ϸ� ��ų Ÿ���� ���̵��, �ʰ��� ����-1�� ���̵�� ����
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

    protected virtual ProjectileController GenerateProjectile(CreatureController Owner, string prefabName, Vector3 startPos, Vector3 dir, Vector3 targetPos, SkillBase skill)
    {
        ProjectileController pc = Managers.Object.Spawn<ProjectileController>(startPos, prefabName: prefabName);
        pc.SetInfo(Owner, startPos, dir, targetPos, skill);
        return pc;
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
