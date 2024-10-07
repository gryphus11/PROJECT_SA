using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    GameManager _game;

    enum Texts
    {
        WaveNumberText,
        WaveTimerText,
        KillCountText,
        LevelText
    }

    enum Sliders
    {
        LevelSlider,
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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _game = Managers.Game;

        _game.Player.OnPlayerLevelUp -= OnPlayerLevelUp;
        _game.Player.OnPlayerLevelUp += OnPlayerLevelUp;

        _game.Player.OnPlayerDataUpdated -= OnPlayerDataUpdated;
        _game.Player.OnPlayerDataUpdated += OnPlayerDataUpdated;
        
        _game.Player.Skills.onSkillChanged -= OnSkillLevelUp;
        _game.Player.Skills.onSkillChanged += OnSkillLevelUp;

        BindTMP(typeof(Texts));
        BindSlider(typeof(Sliders));
        BindImage(typeof(Images));

        Refresh();
        return true;
    }

    private void OnDestroy()
    {
        if (Managers.Game.Player != null)
        {
            Managers.Game.Player.Skills.onSkillChanged -= OnSkillLevelUp;
        }
    }

    public void OnWaveStart(int currentStageIndex)
    {
        GetTMP((int)Texts.WaveNumberText).text = $"Wave Round : {currentStageIndex}";
    }

    public void OnSecondChange(int time)
    {
        GetTMP((int)Texts.WaveTimerText).text = $"Wave Time : {time}";
        if (time < 0)
            GetTMP((int)Texts.WaveTimerText).text = "";
    }

    public void OnWaveEnd()
    {

    }

    private void OnPlayerLevelUp()
    {
        if (Managers.Game.IsGameEnd)
            return;

        List<SkillBase> list = Managers.Game.Player.Skills.RecommendSkills();

        if (list.Count > 0)
        {
            Managers.UI.ShowPopup<UI_SkillSelectPopup>();
        }

        GetSlider((int)Sliders.LevelSlider).value = _game.Player.ExpRatio;
        GetTMP((int)Texts.LevelText).text = $"{_game.Player.Level}";
    }

    void OnPlayerDataUpdated()
    {
        GetSlider((int)Sliders.LevelSlider).value = _game.Player.ExpRatio;
        GetTMP((int)Texts.KillCountText).text = $"{_game.KillCount}";
    }

    void Refresh() // 데이터 갱신
    {
        OnSkillLevelUp();
        GetSlider((int)Sliders.LevelSlider).value = _game.Player.ExpRatio;
        GetTMP((int)Texts.LevelText).text = $"{_game.Player.Level}";
        GetTMP((int)Texts.KillCountText).text = $"{_game.Player.KillCount}";
    }

    private void OnSkillLevelUp()
    {
        ClearSkillSlot();

        //배틀스킬아이콘
        List<SkillBase> activeSkills = Managers.Game.Player.Skills.SkillList.Where(skill => skill.IsLearnedSkill).ToList();
        for (int i = 0; i < activeSkills.Count; i++)
            AddSkillSlot(i, activeSkills[i].SkillData.IconLabel);
    }

    void AddSkillSlot(int index, string iconLabel)
    {
        GetImage(index).sprite = Managers.Resource.Load<Sprite>(iconLabel);
        GetImage(index).enabled = true;
    }

    void ClearSkillSlot()
    {
        int count = Enum.GetValues(typeof(Images)).Length;
        for (int i = 0; i < count; i++)
        {
            GetImage(i).enabled = false;
        }
    }
}
