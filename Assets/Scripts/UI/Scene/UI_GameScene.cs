using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    enum Texts
    {
        WaveNumberText,
        WaveTimerText,
        KillCountText,
    }

    enum Sliders
    {
        LevelSlider,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

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
        if (time == 0)
            GetTMP((int)Texts.WaveTimerText).text = "";
    }

    public void OnWaveEnd()
    {

    }

    private void Update()
    {
        GetTMP((int)Texts.KillCountText).text = $"Kill Count : {Managers.Game.KillCount}";
    }
}
