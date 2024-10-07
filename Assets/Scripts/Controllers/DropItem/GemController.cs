using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GemInfo
{
    public enum GemType
    {
        Small,
        Green,
        Blue,
        Magenta,
    }

    public GemType type;
    public string spriteName;
    public Vector3 gemScale;
    public int expAmount;

    public GemInfo(GemType type, Vector3 gemScale)
    {
        this.type = type;
        spriteName = $"{type}Gem.sprite";
        this.gemScale = gemScale;
        switch (type)
        {
            case GemType.Small:
                expAmount = Define.SMALL_EXP_AMOUNT;
                break;
            case GemType.Green:
                expAmount = Define.GREEN_EXP_AMOUNT;
                break;
            case GemType.Blue:
                expAmount = Define.BLUE_EXP_AMOUNT;
                break;
            case GemType.Magenta:
                expAmount = Define.MAGENTA_EXP_AMOUNT;
                break;
        }
    }
}

public class GemController : DropItemController
{
    GemInfo _gemInfo;
    SpriteRenderer _renderer;

    public override bool Init()
    {
        moveSpeed = 30.0f;

        itemType = Define.ObjectType.Gem;
        base.Init();
        return true;
    }

    public void SetInfo(GemInfo gemInfo)
    {
        if (gemInfo == null)
            return;

        _gemInfo = gemInfo;
        _renderer = GetComponent<SpriteRenderer>();

        _renderer.sprite = Managers.Resource.Load<Sprite>($"{_gemInfo.spriteName}");
        transform.localScale = _gemInfo.gemScale;
    }

    public override void GetItem()
    {
        base.GetItem();

        if (_moveToPlayerTask == null && this.IsValid())
        {
            MoveToPlayerTask().Forget();
        }
    }

    public override void CompleteGetItem()
    {
        base.CompleteGetItem();

        Managers.Game.Player.Exp += _gemInfo.expAmount;
        Managers.Object.Despawn(this);
    }
}
