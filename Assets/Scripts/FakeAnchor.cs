using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeAnchor : MonoBehaviour
{
    public enum AnchorType { Left, Right, Top, Botton, TopLeft, TopRight, BottomLeft, BottomRight, Center}
    public AnchorType anchorType;
    Camera mainCamera;
    float sizeX;
    float sizeY;
    public float scale;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;        
    }

    // Update is called once per frame
    void Update()
    {
        sizeY = mainCamera.orthographicSize;
        sizeX = sizeY * mainCamera.aspect;
        switch (anchorType)
        {
            case AnchorType.TopLeft:
                transform.position = mainCamera.transform.position + new Vector3(-sizeX, sizeY, 0) + offset* mainCamera.aspect;
                transform.localScale = new Vector3(mainCamera.aspect*scale, mainCamera.aspect*scale, mainCamera.aspect*scale);
                break;
            case AnchorType.Left:
                transform.position = mainCamera.transform.position + new Vector3(-sizeX, 0, 0) + offset* mainCamera.aspect;
                transform.localScale = new Vector3(mainCamera.aspect * scale, mainCamera.aspect * scale, mainCamera.aspect * scale);
                break;
            case AnchorType.Center:
                transform.position = mainCamera.transform.position + offset* mainCamera.aspect;
                transform.localScale = new Vector3(mainCamera.aspect * scale, mainCamera.aspect * scale, mainCamera.aspect * scale);
                break;
            default:
                break;
        }
    }
}
