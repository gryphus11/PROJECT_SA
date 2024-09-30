using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Joystick : UI_Scene
{
    Image _background;
    Image _handler;

    TMP_Text _debugJoystickText;
    TMP_Text _debugKeyText;

    float _joystickRadius;
    Vector2 _joystickOriginPos = Vector2.zero;

    Vector2 _touchPosition = Vector2.zero;
    Vector2 _dragPosition = Vector2.zero;
    Vector2 _moveDirection = Vector2.zero;

    Vector2 _keyInputDirection = Vector2.zero;
    Vector2 _prevKeyInputDirection = Vector2.zero;

    System.Text.StringBuilder _debugJoystickString = new System.Text.StringBuilder();
    System.Text.StringBuilder _debugKeyString = new System.Text.StringBuilder();

    enum GameObjects
    {
        JoystickBackground,
        Handler,
        Debug,
    }

    enum Texts
    {
        DebugJoystickDir,
        DebugKeyboardDir,
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
        BindTMP(typeof(Texts));

        _handler = GetObject((int)GameObjects.Handler).GetComponent<Image>();
        _background = GetObject((int)GameObjects.JoystickBackground).GetComponent<Image>();
        _joystickRadius = _background.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        _joystickOriginPos = _background.transform.position;

        _debugJoystickText = GetTMP((int)Texts.DebugJoystickDir);
        _debugKeyText = GetTMP((int)Texts.DebugKeyboardDir);

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

        Managers.Game.MoveDir = _moveDirection;
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

        Managers.Game.MoveDir = _moveDirection;
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

    private void Update()
    {
        if (_moveDirection == Vector2.zero)
        {
            _prevKeyInputDirection = _keyInputDirection;
            _keyInputDirection = GetKeyInput();

            if (_prevKeyInputDirection == Vector2.zero && _keyInputDirection == Vector2.zero)
                return;

            Managers.Game.MoveDir = _keyInputDirection;
        }
        else
        {
            _prevKeyInputDirection = Vector2.zero;
            _keyInputDirection = Vector2.zero;
        }

        _debugJoystickString.Clear();
        _debugJoystickString.Append($"Joystick Dir: {_moveDirection}");
        _debugJoystickText.text = _debugJoystickString.ToString();

        _debugKeyString.Clear();
        _debugKeyString.Append($"Keyboard Dir: {_keyInputDirection}");
        _debugKeyText.text = _debugKeyString.ToString();
    }

    private Vector2 GetKeyInput()
    {
        Vector2 moveDir = Vector2.zero;

        if(Input.GetKey(KeyCode.W))
            moveDir.y = 1;
        if (Input.GetKey(KeyCode.S))
            moveDir.y = -1;
        if (Input.GetKey(KeyCode.A))
            moveDir.x = -1;
        if (Input.GetKey(KeyCode.D))
            moveDir.x = 1;

        return moveDir.normalized;
    }

    public void ToggleDebug()
    { 
    
    }
}
