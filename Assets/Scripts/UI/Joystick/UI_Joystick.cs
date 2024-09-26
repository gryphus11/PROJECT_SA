using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Joystick : UI_Scene
{
    Image _background;
    Image _handler;

    float _joystickRadius;
    Vector2 _joystickOriginPos = Vector2.zero;

    Vector2 _touchPosition = Vector2.zero;
    Vector2 _dragPosition = Vector2.zero;
    Vector2 _moveDirection = Vector2.zero;

    public Action<Vector2> OnDragHandler { get; set; }

    enum GameObjects
    {
        JoystickBackground,
        Handler,
    }

    private void OnDestroy()
    {
        Managers.UI.OnTimeScaleChanged -= OnTimeScaleChanged;
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.OnTimeScaleChanged += OnTimeScaleChanged;

        BindObject(typeof(GameObjects));
        
        _handler = GetObject((int)GameObjects.Handler).GetComponent<Image>();
        _background = GetObject((int)GameObjects.JoystickBackground).GetComponent<Image>();
        _joystickRadius = _background.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        _joystickOriginPos = _background.transform.position;

        gameObject.BindEvent(OnPointerDown, null, Define.UIEvent.PointerDown);
        gameObject.BindEvent(OnPointerUp, null, Define.UIEvent.PointerUp);
        gameObject.BindEvent(null, OnDrag, Define.UIEvent.Drag);

        Initialize();
        return true;
    }

    public void OnDrag(BaseEventData eventData)
    {
        _dragPosition = (eventData as PointerEventData).position;

        Vector2 touchDir = _dragPosition - _touchPosition;
        _moveDirection = touchDir.normalized;

        float moveDistance = Mathf.Min(touchDir.magnitude, _joystickRadius);

        _handler.transform.position = _touchPosition + (_moveDirection * moveDistance);

        OnDragHandler?.Invoke(_moveDirection);
    }

    public void OnPointerDown()
    {
        SetActiveJoyStick(true);

        if(Input.touchCount > 0)
            _touchPosition = Input.GetTouch(0).position;
        else
            _touchPosition = Input.mousePosition;

        _background.transform.position = _touchPosition;
        _handler.transform.position = _touchPosition;
    }

    public void OnPointerUp()
    {
        Initialize();
    }

    private void Initialize()
    {
        SetActiveJoyStick(false);

        _touchPosition = Vector2.zero;
        _dragPosition = Vector2.zero;
        _moveDirection = Vector2.zero;

        OnDragHandler?.Invoke(_moveDirection);
    }

    private void SetActiveJoyStick(bool isActive)
    {
        _background.gameObject.SetActive(isActive);
        _handler.gameObject.SetActive(isActive);
    }

    public void OnTimeScaleChanged(int timeScale)
    {
        if (timeScale == 1)
        {
            gameObject.SetActive(true);
            Initialize();
        }
        else
            gameObject.SetActive(false);
    }
}
