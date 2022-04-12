using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorController : MonoBehaviour
{
    [SerializeField] Color[] colors;
    Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    public void SetColor(int colorIdx)
    {
        cam.backgroundColor = colors[colorIdx];
    }
}
