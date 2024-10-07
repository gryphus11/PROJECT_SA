using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemController : BaseController
{
    public float CollectDistance { get; set; } = 4.0f;
    public Coroutine _coroutine;
    public Define.ObjectType itemType;
    
    public float moveSpeed = 15.0f;

    protected UniTaskCompletionSource _moveToPlayerTask = null;
    public override bool Init()
    {
        base.Init();
        return true;
    }

    public virtual void OnDisable()
    {
        if (_moveToPlayerTask != null)
        {
            _moveToPlayerTask.TrySetCanceled();
            _moveToPlayerTask = null;
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

    public virtual async UniTask MoveToPlayerTask()
    {
        _moveToPlayerTask = new UniTaskCompletionSource();

        while (this.IsValid() == true)
        {
            float distance = Vector3.Distance(gameObject.transform.position, Managers.Game.Player.PlayerCenterPos);

            transform.position = Vector3.MoveTowards(transform.position, Managers.Game.Player.PlayerCenterPos, Time.deltaTime * moveSpeed);
            if (distance < CollectDistance)
            {
                CompleteGetItem();
                return;
            }

            await UniTask.WaitForFixedUpdate();
        }
    }
}
