using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

/// <summary>
/// 게임의 전반적인 상황을 다룸
/// </summary>
public class GameManager
{
    public bool BGMOn { get; set; }
    public bool EffectSoundOn { get; set; }

    #region 플레이어
    public PlayerController Player { get; set; }

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

    public event Action<Vector2> onMoveDirChanged;

    public WaveData CurrentWaveData
    {
        get { return WaveArray[CurrentWaveIndex]; }
    }

    public List<WaveData> WaveArray { get; set; }

    public int CurrentWaveIndex { get; set; } = 0;

    public int SelectedPlayerID { get; set; } = 100002;

    public float TimeRemaining { get; set; } = 60;
}
