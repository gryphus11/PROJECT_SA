using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 전반적인 상황을 다룸
/// </summary>
public class GameManager
{
    public bool BGMOn { get; set; }
    public bool EffectSoundOn { get; set; }

    Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(_moveDir);
            Debug.Log(_moveDir);
        }
    }

    public event Action<Vector2> OnMoveDirChanged;
}
