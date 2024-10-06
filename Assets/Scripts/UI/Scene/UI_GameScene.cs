using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _game = Managers.Game;

        _game.Player.OnPlayerLevelUp = OnPlayerLevelUp;
        BindTMP(typeof(Texts));
        BindSlider(typeof(Sliders));

        return true;
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

    private void Update()
    {
        GetTMP((int)Texts.KillCountText).text = $"Kill Count : {Managers.Game.KillCount}";
    }

    private void OnPlayerLevelUp()
    {
        List<SkillBase> list = Managers.Game.Player.Skills.RecommendSkills();

        if (list.Count > 0)
        {
            Managers.UI.ShowPopup<UI_SkillSelectPopup>();
        }

        GetSlider((int)Sliders.LevelSlider).value = _game.Player.ExpRatio;
        GetTMP((int)Texts.LevelText).text = $"{_game.Player.Level}";
    }
}
