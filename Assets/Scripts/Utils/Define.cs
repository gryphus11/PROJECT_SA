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
        Elite,
        Projectile,
        Gem,
        Map,
        DropBox,
        Potion,
        Magnet,
        Bomb,
    }

    public enum CreatureState
    {
        Idle,
        Skill,
        Moving,
        OnDamaged,
        Dead
    }
    public enum DropItemType
    {
        Potion,
        Magnet,
        DropBox,
        Bomb
    }

    /// <summary>
    /// 스킬의 인덱스를 정의
    /// </summary>
    public enum SkillType
    {
        None = 0,
        IcicleArrow = 10001,       //100001 ~ 100006
        Lightning = 10011,
        Slash = 10021,
        Sanctuary = 10031,
        BounceArrow = 10041,
        SpinCutter = 10051,
        Meteor = 10061,
    }

    #region sortingOrder
    public static readonly int UI_GAMESCENE_SORT_CLOSED = 321;
    public static readonly int DROP_SORT = 105;

    public static readonly int UI_GAMESCENE_SORT_OPEN = 323;
    public static readonly int DROP_GETITEM_SORT = 322;
    #endregion

    public static readonly float DROP_ITEM_COLLECT_DISTANCE = 2.5f;

    public static float KNOCKBACK_TIME = 0.1f;// 밀려나는시간
    public static float KNOCKBACK_SPEED = 10;  // 속도 
    public static float KNOCKBACK_COOLTIME = 0.7f;

    public static int MAX_SKILL_LEVEL = 6;
    public static int MAX_SKILL_COUNT = 6;

    public const int ONE_SECOND_TO_MILLISEC = 1000;

    #region 보석 경험치 획득량
    public const int SMALL_EXP_AMOUNT = 1;
    public const int GREEN_EXP_AMOUNT = 2;
    public const int BLUE_EXP_AMOUNT = 5;
    public const int MAGENTA_EXP_AMOUNT = 10;
    #endregion
}
