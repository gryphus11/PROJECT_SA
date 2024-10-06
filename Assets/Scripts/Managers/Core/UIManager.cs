using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager
{
    int _canvasOrder = 20;
    UI_Base _sceneUI;

    Stack<UI_Base> _uiStack = new Stack<UI_Base>();

    public event Action<int> OnTimeScaleChanged;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }

    /// <summary>
    /// 캔버스의 순서를 자동으로 조절합니다.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="sort"></param>
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _canvasOrder;
            _canvasOrder++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T GetSceneUI<T>() where T : UI_Base
    {
        return _sceneUI as T;
    }

    #region 선로드가 완료된 리소스에 한해서 사용합니다.
    public T MakeSubItem<T>(Transform parent = null, string key = null, bool pooling = true) where T : UI_Base
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        var subItem = Managers.Resource.Instantiate(key, parent, pooling);
        subItem.transform.SetParent(parent);
        return subItem.gameObject.GetOrAddComponent<T>();
    }

    public T ShowSceneUI<T>(string key = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        _sceneUI = Managers.Resource.Instantiate(key).GetOrAddComponent<T>();
        _sceneUI.transform.SetParent(Root.transform);
        return _sceneUI as T;
    }

    public T ShowPopup<T>(string key = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        var ui = Managers.Resource.Instantiate(key, Root.transform).GetOrAddComponent<T>();
        _uiStack.Push(ui);
        RefreshTimeScale();
        return ui;
    }
    #endregion

    #region 비동기로딩시 사용합니다.
    public void MakeSubItemAsync<T>(Transform parent = null, string key = null, Action<T> callback = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        Managers.Resource.InstantiateAsync(key, parent, (go) =>
        {
            T subItem = Utils.GetOrAddComponent<T>(go);
            callback?.Invoke(subItem);
        });
    }

    public void ShowSceneUIAsync<T>(string key = null, Action<T> callback = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        Managers.Resource.InstantiateAsync(key, Root.transform, (go) =>
        {
            T sceneUI = Utils.GetOrAddComponent<T>(go);
            _sceneUI = sceneUI;
            callback?.Invoke(sceneUI);
        });

    }

    public void ShowPopupAsync<T>(string key = null, Transform parent = null, Action<T> callback = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(key))
            key = typeof(T).Name;

        Managers.Resource.InstantiateAsync(key, null, (go) =>
        {
            T popup = Utils.GetOrAddComponent<T>(go);
            _uiStack.Push(popup);

            if (parent != null)
                go.transform.SetParent(parent);
            else
                go.transform.SetParent(Root.transform);

            callback?.Invoke(popup);

            RefreshTimeScale();
        });
    }
    #endregion


    public T FindPopup<T>() where T : UI_Base
    {
        return _uiStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    public T PeekPopup<T>() where T : UI_Base
    {
        if (_uiStack.Count == 0)
            return null;

        return _uiStack.Peek() as T;
    }

    public void ClosePopup(UI_Base popup)
    {
        if (_uiStack.Count == 0)
            return;

        if (_uiStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed");
            return;
        }

        ClosePopup();
    }

    public void ClosePopup()
    {
        if (_uiStack.Count == 0)
            return;

        var ui = _uiStack.Pop();
        Managers.Resource.Destroy(ui.gameObject);
        ui = null;
        RefreshTimeScale();
    }

    public void CloseAllPopup()
    {
        while (_uiStack.Count > 0)
            ClosePopup();
    }

    public void Clear()
    {
        CloseAllPopup();
        _sceneUI = null;
    }


    public void RefreshTimeScale()
    {
        if (SceneManager.GetActiveScene().name != Define.SceneType.Game.ToString())
        {
            Time.timeScale = 1;
            return;
        }

        if (_uiStack.Count > 0)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;

        OnTimeScaleChanged?.Invoke((int)Time.timeScale);
    }
}
