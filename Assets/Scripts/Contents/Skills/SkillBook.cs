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
    /// 확인용
    /// </summary>
    [SerializeField]
    private List<SkillBase> _skillList = new List<SkillBase>();

    /// <summary>
    /// 모든 스킬을 들고 있으며 관리해 준다.
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
        //모든스킬은 0으로 시작함. 레벨 수 만큼 레벨업ㅎ ㅏ기
        AddSkill(skillType);
        for (int i = 0; i < level; i++)
            LevelUpSkill(skillType);

    }

    public void AddSkill(Define.SkillType skillType, int skillId = 0)
    {
        string className = skillType.ToString();

        // Melee 타입
        if (skillType == SkillType.Slash)
        {
            // Melee 타입은 시전자를 부모로 붙어 생성
            GameObject go = Managers.Resource.Instantiate(skillType.ToString(), gameObject.transform);
            
            if (go != null)
            {
                SkillBase skill = go.GetOrAddComponent<SkillBase>();
                AddList(skill, skillType);
            }
        }
        else
        {
            // 투사체형은
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

    #region 스킬 가챠
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

        //1. 이미 6개의 스킬을 배웠으면 배운 스킬중 5렙 미만인 스킬을 추천
        if (activeSkills.Count == MAX_SKILL_COUNT)
        {
            List<SkillBase> recommendSkills = activeSkills.FindAll(s => s.Level < MAX_SKILL_LEVEL);
            recommendSkills.Shuffle();
            return recommendSkills.Take(3).ToList();
        }
        else
        {
            // 레벨이 5 미만인 스킬 
            List<SkillBase> recommendSkills = skillList.FindAll(s => s.Level < MAX_SKILL_LEVEL);
            recommendSkills.Shuffle();
            return recommendSkills.Take(3).ToList();
        }
    }
    #endregion
}
