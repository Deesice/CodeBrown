using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    Camera mainCamera;
    float cameraHalfWidth;
    float flag = 1;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.GetComponent<Camera>();
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect; 
    }
    void Update()
    {
        if (flag * (mainCamera.transform.position.x - transform.position.x) + cameraHalfWidth - GetComponent<SpriteRenderer>().size.x * transform.lossyScale.y < 0)
        {
            Flip();
            flag *= -1;
        }
    }

    void Flip()
    {
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        var pivot = transform.Find("Canvas").GetComponent<RectTransform>().pivot;
        pivot.x += Mathf.Cos(Mathf.PI * pivot.x);
        transform.Find("Canvas").GetComponent<RectTransform>().pivot = pivot;
        transform.Find("Canvas").GetComponent<RectTransform>().localPosition = new Vector3 (0,0,0);
    }
}
