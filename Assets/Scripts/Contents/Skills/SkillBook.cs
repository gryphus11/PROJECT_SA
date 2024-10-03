using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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

    public List<SkillBase> ActivatedSkills
    {
        get { return SkillList.Where(skill => skill.IsLearnedSkill).ToList(); }
    }

    [SerializeField]
    public Dictionary<Define.SkillType, int> SavedBattleSkill = new Dictionary<SkillType, int>();

    public event Action UpdateSkillUi;
    public ObjectType _ownerType;

    public void Awake()
    {
        _ownerType = GetComponent<CreatureController>().ObjectType;

    }

    public void LoadSkill(Define.SkillType skillType, int level)
    {
        //��罺ų�� 0���� ������. ���� �� ��ŭ �������� ����
        AddSkill(skillType);
        for (int i = 0; i < level; i++)
            LevelUpSkill(skillType);

    }

    public void AddSkill(Define.SkillType skillType, int skillId = 0)
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
                AddList(skill, skillType);
            }
        }
        else
        {
            // ����ü����
            RepeatSkill skillBase = gameObject.GetComponent(Type.GetType(className)) as RepeatSkill;
            AddList(skillBase, skillType);
        }
    }

    private void AddList(SkillBase skill, SkillType skillType)
    {
        SkillList.Add(skill);
        if (SavedBattleSkill.ContainsKey(skillType))
            SavedBattleSkill[skillType] = skill.Level;
        else
            SavedBattleSkill.Add(skillType, skill.Level);
    }


    public void AddActiavtedSkills(SkillBase skill)
    {
        ActivatedSkills.Add(skill);
    }


    bool _stopped = false;

    public void StopSkills()
    {
        _stopped = true;

        foreach (var skill in ActivatedSkills)
        {
            skill.StopAllCoroutines();
        }
    }

    public void LevelUpSkill(Define.SkillType skillType)
    {
        for (int i = 0; i < SkillList.Count; i++)
        {
            if (SkillList[i].SkillType == skillType)
            {
                SkillList[i].OnLevelUp();
                if (SavedBattleSkill.ContainsKey(skillType))
                {
                    SavedBattleSkill[skillType] = SkillList[i].Level;
                }
                UpdateSkillUi?.Invoke();
            }
        }
    }

    public void OnSkillBookChanged()
    {
        UpdateSkillUi?.Invoke();
    }

    public void Clear()
    {
        SavedBattleSkill.Clear();
    }

    #region ��ų ��í
    public SkillBase RecommandDropSkill()
    {
        List<SkillBase> skillList = Managers.Game.Player.Skills.SkillList.ToList();
        List<SkillBase> activeSkills = skillList.FindAll(skill => skill.IsLearnedSkill);

        List<SkillBase> recommendSkills = activeSkills.FindAll(s => s.Level < 5);
        recommendSkills.Shuffle();
        //Util.Shuffle(recommandSkills);
        return recommendSkills[0];
    }

    public List<SkillBase> RecommendSkills()
    {
        List<SkillBase> skillList = Managers.Game.Player.Skills.SkillList.ToList();
        List<SkillBase> activeSkills = skillList.FindAll(skill => skill.IsLearnedSkill);

        //1. �̹� 6���� ��ų�� ������� ��� ��ų�� 5�� �̸��� ��ų�� ��õ
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
