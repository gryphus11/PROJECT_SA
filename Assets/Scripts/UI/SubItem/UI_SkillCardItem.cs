using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class UI_SkillCardItem : UI_Base
{
    enum TMPros
    {
        NewText,
        DescriptionText,
        LevelText,
        SkillNameText,
    }

    enum Images
    {
        SkillIcon,
    }

    SkillBase _skill;

    private void Awake()
    {
        Init();
    }

    public override bool Init()
    {
        if(base.Init() == false) 
            return false;

        BindTMP(typeof(TMPros));
        BindImage(typeof(Images));
        gameObject.BindEvent(OnClicked);

        return true;
    }

    public void SetInfo(SkillBase skill)
    {
        _skill = skill;
        transform.localScale = Vector3.one;
        GetTMP((int)TMPros.NewText).gameObject.SetActive(false);

        GetImage((int)Images.SkillIcon).sprite = Managers.Resource.Load<Sprite>(skill.UpdateSkillData().IconLabel);
        GetTMP((int)TMPros.SkillNameText).text = _skill.SkillData.Name;
        GetTMP((int)TMPros.DescriptionText).text = _skill.SkillData.Description;
        GetTMP((int)TMPros.LevelText).text = $"Level : {_skill.Level + 1}";
    }

    public void OnClicked()
    {
        Managers.Game.Player.Skills.LevelUpSkill(_skill.SkillType);
        Managers.UI.ClosePopup();
    }
}
