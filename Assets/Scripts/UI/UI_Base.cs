using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Type = System.Type;

public class UI_Base : MonoBehaviour
{
    Dictionary<Type, List<Object>> _uiObjects = new Dictionary<System.Type, List<Object>>();

    #region Bind
    protected void BindUI<T>(Type nameType) where T : Object
    { 
        var names = System.Enum.GetNames(nameType);

        foreach (var name in names)
        {
            Object find = null;
            if(typeof(T) == typeof(GameObject))
                find = Utils.FindChild(gameObject, name, true);
            else
                find = Utils.FindChild<T>(gameObject, name, true);

            if (find == null)
            {
                Debug.Log($"Failed To Find UI Element : {name} of {typeof(T).Name}");
                continue;
            }

            if (!_uiObjects.TryGetValue(typeof(T), out List<Object> objList))
            {
                _uiObjects.Add(typeof(T), new List<Object>());
                objList = _uiObjects[typeof(T)];
            }

            objList.Add(find);
        }
    }

    protected void BindObject(Type type)
    { 
        BindUI<GameObject>(type);
    }

    protected void BindText(Type type)
    {
        BindUI<Text>(type);
    }

    protected void BindTMP(Type type)
    {
        BindUI<TMP_Text>(type);
    }

    protected void BindImage(Type type)
    {
        BindUI<Image>(type);
    }

    protected void BindButton(Type type)
    {
        BindUI<Button>(type);
    }

    protected void BindToggle(Type type)
    {
        BindUI<Toggle>(type);
    }

    protected void BindSlider(Type type)
    {
        BindUI<Slider>(type);
    }
    #endregion

    #region Get
    public T GetUI<T>(int idx) where T : Object
    {
        if (!_uiObjects.TryGetValue(typeof(T), out List<Object> objList))
            return null;

        if (objList.Count == 0)
            return null;

        if (objList.Count <= idx)
            return null;

        return objList[idx] as T;
    }

    protected GameObject GetObject(int idx)
    {
        return GetUI<GameObject>(idx);
    }

    protected Text GetText(int idx)
    {
        return GetUI<Text>(idx);
    }

    protected TMP_Text GetTMP(int idx)
    {
        return GetUI<TMP_Text>(idx);
    }

    protected Image GetImage(int idx)
    {
        return GetUI<Image>(idx);
    }

    protected Button GetButton(int idx)
    {
        return GetUI<Button>(idx);
    }

    protected Toggle GetToggle(int idx)
    {
        return GetUI<Toggle>(idx);
    }

    protected Slider GetSlider(int idx)
    {
        return GetUI<Slider>(idx);
    }
    #endregion

    protected bool _init = false;

    public virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    private void Start()
    {
        Init();
    }


    public static void BindEvent(GameObject go, System.Action action = null, System.Action<BaseEventData> dragAction = null, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Pressed:
                evt.OnPressedHandler -= action;
                evt.OnPressedHandler += action;
                break;
            case Define.UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case Define.UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= dragAction;
                evt.OnDragHandler += dragAction;
                break;
            case Define.UIEvent.BeginDrag:
                evt.OnBeginDragHandler -= dragAction;
                evt.OnBeginDragHandler += dragAction;
                break;
            case Define.UIEvent.EndDrag:
                evt.OnEndDragHandler -= dragAction;
                evt.OnEndDragHandler += dragAction;
                break;
        }
    }

}
