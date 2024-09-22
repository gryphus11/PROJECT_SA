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
}
