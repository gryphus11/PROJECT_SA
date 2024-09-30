using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseScene : MonoBehaviour
{
    public SceneType sceneType = SceneType.Unknown;
    protected bool _init = false;

    public void Awake()
    {
        Init();
    }

    protected virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        Managers.Init();

        GameObject go = GameObject.Find("EventSystem");
        if (go == null)
        {
            go = Managers.Resource.Instantiate("EventSystem");

            // 선로딩이 아닌 경우 비동기로
            //Managers.Resource.InstantiateAsync("EventSystem", null, (go) =>
            //{
            //    go.name = "@EventSystem";
            //});
        }

        return true;
    }

    public virtual void Clear()
    {

    }
}
