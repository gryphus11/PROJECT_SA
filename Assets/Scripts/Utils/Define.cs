using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum UIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }


    public enum SceneType
    {
        Unknown,
        Title,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        SubBgm,
        Effect,
        MaxCount,
    }

    public enum ObjectType
    {
        Unknown,
        Player,
        Monster,
        Projectile,
    }

    public enum CreatureState
    {
        Idle,
        Skill,
        Moving,
        OnDamaged,
        Dead
    }

    #region sortingOrder
    public static readonly int UI_GAMESCENE_SORT_CLOSED = 321;
    public static readonly int SOUL_SORT = 105;

    public static readonly int UI_GAMESCENE_SORT_OPEN = 323;
    public static readonly int SOUL_SORT_GETITEM = 322;
    #endregion

    public static float KNOCKBACK_TIME = 0.1f;// 밀려나는시간
    public static float KNOCKBACK_SPEED = 10;  // 속도 
    public static float KNOCKBACK_COOLTIME = 0.5f;
}
