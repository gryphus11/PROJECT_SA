using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using static Define;

public class SkillBook : MonoBehaviour
{
    /// <summary>
    /// Ȯ�ο�
    /// </summary>
    [SerializeField]
    private List<SkillBase> _skillList = new List<SkillBase>();

    /// <summary>
    /// ��� ��ų�� ��� ������ ������ �ش�.
    /// </summary>
    public List<SkillBase> SkillList { get { return _skillList; } }

    /// <summary>
    /// ���� 1 �̻��� ��ų�鸸 ��ȯ
    /// </summary>
    public List<SkillBase> ActivatedSkills
    {
        get { return SkillList.Where(skill => skill.IsLearnedSkill).ToList(); }
    }

    public event Action onSkillChanged;
    public ObjectType OwnerType { get; protected set; }

    public void Awake()
    {
        OwnerType = GetComponent<CreatureController>().ObjectType;

    }

    public void LoadSkill(Define.SkillType skillType, int level)
    {
        //��罺ų�� 0���� ������. ���� �� ��ŭ �������� ����
        AddSkill(skillType);
        for (int i = 0; i < level; i++)
            LevelUpSkill(skillType);

    }

    public void AddSkill(Define.SkillType skillType)
    {
        string className = skillType.ToString();

        // Melee Ÿ��
        if (skillType == SkillType.Slash)
        {
            // Melee Ÿ���� �����ڸ� �θ�� �پ� ����
            GameObject go = Managers.Resource.Instantiate(skillType.ToString(), gameObject.transform);
            
            if (go != null)
            {
                SkillBase skill = go.GetOrAddComponent<SkillBase>();
                AddList(skill);
            }
        }
        else
        {
            // ����ü����
            RepeatSkill skillBase = gameObject.GetComponent(Type.GetType(className)) as RepeatSkill;
            AddList(skillBase);
        }
    }

    private void AddList(SkillBase skill)
    {
        SkillList.Add(skill);
    }

    public bool HasSkill(SkillType skillType)
    {
        return _skillList.Find(skill => skill.name == skillType.ToString()) != null;
    }

    bool _stopped = false;

    [ContextMenu("��ų �ߴ�")]
    public void StopSkills()
    {
        _stopped = true;

        foreach (var skill in ActivatedSkills)
        {
            skill.StopSkill();
        }
    }

    public void LevelUpSkill(Define.SkillType skillType)
    {
        for (int i = 0; i < SkillList.Count; i++)
        {
            // ��ų Ÿ���� ���� ��ų�� ã�� ������
            if (SkillList[i].SkillType == skillType)
            {
                SkillList[i].OnLevelUp();
                OnSkillBookChanged();
            }
        }
    }

    public void OnSkillBookChanged()
    {
        onSkillChanged?.Invoke();
    }

    #region ��ų ���� ����
    public SkillBase RecommendDropSkill()
    {
        List<SkillBase> skillList = Managers.Game.Player.Skills.SkillList.ToList();

        // ��� ��ų�� ������ 5 �̸��� ��ų�� ��õ
        List<SkillBase> activeSkills = skillList.FindAll(skill => skill.IsLearnedSkill);
        List<SkillBase> recommendSkills = activeSkills.FindAll(s => s.Level < 5);

        // ����
        recommendSkills.Shuffle();
        return recommendSkills[0];
    }

    public List<SkillBase> RecommendSkills()
    {
        List<SkillBase> skillList = Managers.Game.Player.Skills.SkillList.ToList();
        List<SkillBase> activeSkills = skillList.FindAll(skill => skill.IsLearnedSkill);

        // �̹� �ִ� ������ ��ų�� ������� ��� ��ų�� 5�� �̸��� ��ų�� ��õ
        if (activeSkills.Count == MAX_SKILL_COUNT)
        {
            List<SkillBase> recommendSkills = activeSkills.FindAll(s => s.Level < MAX_SKILL_LEVEL);
            
            recommendSkills.Shuffle();
            return recommendSkills.Take(3).ToList();
        }
        else
        {
            // ������ 5 �̸��� ��ų 
            List<SkillBase> recommendSkills = skillList.FindAll(s => s.Level < MAX_SKILL_LEVEL);
            recommendSkills.Shuffle();
            return recommendSkills.Take(3).ToList();
        }
    }
    #endregion
}
