using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_SkillSelectPopup : UI_Popup
{
    enum GameObjects
    {
        ContentsGroup,
    }

    enum VerticalLayoutGroups
    {
        SkillContent,
    }

    enum Images
    {
        LearnedSkillIcon01,
        LearnedSkillIcon02,
        LearnedSkillIcon03,
        LearnedSkillIcon04,
        LearnedSkillIcon05,
        LearnedSkillIcon06,
    }

    private void OnEnable()
    {
        Init();
        PopupOpenAnimation(GetObject((int)GameObjects.ContentsGroup));
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindUI<VerticalLayoutGroup>(typeof(VerticalLayoutGroups));
        SetRecommendSkills();
        
        List<SkillBase> activeSkills = Managers.Game.Player.Skills.SkillList.Where(skill => skill.IsLearnedSkill).ToList();

        for (int i = 0; i < activeSkills.Count; i++)
        {
            SetLearnedSkill(i, activeSkills[i]);
        }

        return false;
    }
    
    void SetLearnedSkill(int index, SkillBase skill)
    {
        GetImage(index).sprite = Managers.Resource.Load<Sprite>(skill.SkillData.IconLabel);
        GetImage(index).enabled = true;
    }

    void SetRecommendSkills()
    {
        VerticalLayoutGroup container = GetUI<VerticalLayoutGroup>((int)VerticalLayoutGroups.SkillContent);

        //√ ±‚»≠
        container.gameObject.DestroyChilds();

        List<SkillBase> List = Managers.Game.Player.Skills.RecommendSkills();

        foreach (SkillBase skill in List)
        {
            UI_SkillCardItem item = Managers.UI.MakeSubItem<UI_SkillCardItem>(container.transform);
            item.GetComponent<UI_SkillCardItem>().SetInfo(skill);
        }
    }

}
