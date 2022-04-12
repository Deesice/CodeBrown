using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomJoystick : MonoBehaviour, IPointerDownHandler
{
    public int FingerId { get; private set; }
    [SerializeField] Image background;
    [SerializeField] Image handle;
    [SerializeField] float pixelSize;
    [SerializeField] bool lockVertical;
    IEnumerator coroutine;
    public Vector2 Axis => handle.rectTransform.anchoredPosition / pixelSize;
    public Vector2 HandlePosition => handle.rectTransform.position;
    public bool IsVisible => background.gameObject.activeSelf;
    private void Awake()
    {
        Cancel();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (background.gameObject.activeSelf)
            return;

        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.position == eventData.position)
            {
                FingerId = touch.fingerId;
                break;
            }
        }

        background.gameObject.SetActive(true);
        background.rectTransform.position = eventData.position;
        handle.rectTransform.anchoredPosition = Vector2.zero;
    }
    public void Cancel()
    {
        handle.rectTransform.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);
        FingerId = -1;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    void Update()
    {
        int i = 0;
        for (; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.fingerId != FingerId)
            {
                continue;
            }
            else
            {
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    Cancel();
                }
                else
                {
                    handle.rectTransform.position = touch.position;
                    var newAnchoredPosition = Vector2.ClampMagnitude(handle.rectTransform.anchoredPosition, pixelSize);
                    if (lockVertical)
                        newAnchoredPosition.y = 0;
                    handle.rectTransform.anchoredPosition = newAnchoredPosition;
                }
                break;
            }
        }
        if (i == Input.touchCount)
        {
            Cancel();
        }
    }
}
