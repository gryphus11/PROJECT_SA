using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Joystick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    Image background;

    [SerializeField]
    Image handler;

    float joystickRadius;
    Vector2 touchPosition = Vector2.zero;
    Vector2 dragPosition = Vector2.zero;
    Vector2 moveDirection = Vector2.zero;

    public Action<Vector2> OnDragHandler { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        joystickRadius = background.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        dragPosition = eventData.position;

        Vector2 touchDir = dragPosition - touchPosition;
        moveDirection = touchDir.normalized;

        float moveDistance = Mathf.Min(touchDir.magnitude, joystickRadius);

        handler.transform.position = touchPosition + (moveDirection * moveDistance);

        OnDragHandler?.Invoke(moveDirection);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true);
        handler.gameObject.SetActive(true);

        touchPosition = eventData.position;
        background.transform.position = touchPosition;
        handler.transform.position = touchPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Initialize();
    }

    private void Initialize()
    {
        background.gameObject.SetActive(false);
        handler.gameObject.SetActive(false);

        touchPosition = Vector2.zero;
        dragPosition = Vector2.zero;
        moveDirection = Vector2.zero;

        OnDragHandler?.Invoke(moveDirection);
    }
}
