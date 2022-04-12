using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenKey : MonoBehaviour
{
    public int inputValue;
    public bool pressed;
    void Awake()
    {
        GetComponentInChildren<Text>(true).text = inputValue.ToString();
    }
    void OnMouseDown()
    {
        pressed = true;
    }
}
