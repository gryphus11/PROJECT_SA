using System.Collections.Generic;
using UnityEngine;
using Data;
using static UnityEditor.Experimental.GraphView.GraphView;

/// <summary>
/// 게임의 전반적인 상황을 다룸
/// </summary>
public class GameManager
{
    public bool IsGameEnd { get; set; } = false;
    public bool BGMOn { get; set; }
    public bool EffectSoundOn { get; set; }

    #region 플레이어
    public PlayerController Player { get; set; }

    public CharacterStatus Character { get; set; }

    Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            onMoveDirChanged?.Invoke(_moveDir);
        }
    }

    public int KillCount { get; set; }
    #endregion

    public event System.Action<Vector2> onMoveDirChanged;

    public WaveData CurrentWaveData
    {
        get { return WaveArray[CurrentWaveIndex]; }
    }

    public List<WaveData> WaveArray { get; set; }

    public int CurrentWaveIndex { get; set; } = 0;

    public int SelectedPlayerID { get; set; } = 100002;

    public float TimeRemaining { get; set; } = 60;

    public MapController CurrentMap { get; set; }

    public float Exp { get; set; } = 0;
    public int Level { get; set; } = 1;

    public float TotalExp { get; set; } = 0;

    public GemInfo GetCurrentWaveGemInfo()
    {
        float smallGemChance = CurrentWaveData.SmallGemDropRate;
        float greenGemChance = CurrentWaveData.GreenGemDropRate + smallGemChance;
        float blueGemChance = CurrentWaveData.BlueGemDropRate + greenGemChance;
        float yellowGemChance = CurrentWaveData.YellowGemDropRate + blueGemChance;
        float rand = Random.value;

        Vector3 half = Vector3.one * 0.5f;

        if (rand < smallGemChance)
            return new GemInfo(GemInfo.GemType.Small, new Vector3(0.35f, 0.35f, 0.35f));
        else if (rand < greenGemChance)
            return new GemInfo(GemInfo.GemType.Green, half);
        else if (rand < blueGemChance)
            return new GemInfo(GemInfo.GemType.Blue, half);
        else if (rand < yellowGemChance)
            return new GemInfo(GemInfo.GemType.Magenta, half);
        return null;
    }

    public void GameOver()
    {
        IsGameEnd = true;
        Player.StopAllTask();
        Managers.UI.ShowPopup<UI_GameOverPopup>().SetInfo();
    }
}
