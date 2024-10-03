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
    /// 확인용
    /// </summary>
    [SerializeField]
    private List<SkillBase> _skillList = new List<SkillBase>();

    /// <summary>
    /// 모든 스킬을 들고 있으며 관리해 준다.
    /// </summary>
    public List<SkillBase> SkillList { get { return _skillList; } }

    /// <summary>
    /// 레벨 1 이상의 스킬들만 반환
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
        //모든스킬은 0으로 시작함. 레벨 수 만큼 레벨업ㅎ ㅏ기
        AddSkill(skillType);
        for (int i = 0; i < level; i++)
            LevelUpSkill(skillType);

    }

    public void AddSkill(Define.SkillType skillType)
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
                AddList(skill);
            }
        }
        else
        {
            // 투사체형은
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

    [ContextMenu("스킬 중단")]
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
            // 스킬 타입이 같은 스킬을 찾아 레벨업
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

    #region 스킬 선택 관련
    public SkillBase RecommendDropSkill()
    {
        List<SkillBase> skillList = Managers.Game.Player.Skills.SkillList.ToList();

        // 배운 스킬중 레벨이 5 미만인 스킬을 추천
        List<SkillBase> activeSkills = skillList.FindAll(skill => skill.IsLearnedSkill);
        List<SkillBase> recommendSkills = activeSkills.FindAll(s => s.Level < 5);

        // 섞기
        recommendSkills.Shuffle();
        return recommendSkills[0];
    }

    public List<SkillBase> RecommendSkills()
    {
        List<SkillBase> skillList = Managers.Game.Player.Skills.SkillList.ToList();
        List<SkillBase> activeSkills = skillList.FindAll(skill => skill.IsLearnedSkill);

        // 이미 최대 갯수의 스킬을 배웠으면 배운 스킬중 5렙 미만인 스킬을 추천
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
