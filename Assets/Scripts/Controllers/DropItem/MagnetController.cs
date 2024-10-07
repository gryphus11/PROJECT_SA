using Cysharp.Threading.Tasks;
using UnityEngine;

public class MagnetController : DropItemController
{
    Data.DropItemData _dropItemData;

    public override bool Init()
    {
        base.Init();
        transform.localScale = Vector3.one * 0.5f;
        itemType = Define.ObjectType.Magnet;
        return true;
    }

    public override void GetItem()
    {
        base.GetItem();
        
        if (_moveToPlayerTask == null && this.IsValid())
        {
            MoveToPlayerTask().Forget();
        }
    }

    public void SetInfo(Data.DropItemData data)
    {
        _dropItemData = data;
        CollectDistance = Define.DROP_ITEM_COLLECT_DISTANCE;
        GetComponent<SpriteRenderer>().sprite = Managers.Resource.Load<Sprite>(_dropItemData.SpriteName);

    }

    public override void CompleteGetItem()
    {
        Managers.Object.CollectAllItems();
        Managers.Object.Despawn(this);
    }
}
