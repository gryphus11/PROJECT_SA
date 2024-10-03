using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemController : BaseController
{
    public float CollectDist { get; set; } = 4.0f;
    public Coroutine _coroutine;
    public Define.ObjectType itemType;

    UniTaskCompletionSource _uniTaskCompletionSource = null;
    public override bool Init()
    {
        base.Init();
        return true;
    }

    public virtual void OnDisable()
    {
        if (_uniTaskCompletionSource != null)
        {
            _uniTaskCompletionSource.TrySetCanceled();
            _uniTaskCompletionSource = null;
        }
    }

    public void OnEnable()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Define.DROP_SORT;
    }

    public virtual void GetItem()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Define.DROP_GETITEM_SORT;
    }

    public virtual void CompleteGetItem()
    {
    }

    public async UniTask CheckDistanceTask()
    {
        _uniTaskCompletionSource = new UniTaskCompletionSource();

        while (this.IsValid() == true)
        {
            float dist = Vector3.Distance(gameObject.transform.position, Managers.Game.Player.PlayerCenterPos);

            transform.position = Vector3.MoveTowards(transform.position, Managers.Game.Player.PlayerCenterPos, Time.deltaTime * 15.0f);
            if (dist < 1f)
            {
                CompleteGetItem();
                return;
            }

            await UniTask.WaitForFixedUpdate();
        }
    }
}
