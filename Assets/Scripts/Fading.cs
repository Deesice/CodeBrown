using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Fading : MonoBehaviour
{
    public bool anyKeyPressed = false;
    public float realiseTime = 1;
    public float endSize = 1.2f;
    public UnityEvent next;
    bool isText = false;
    float lifeTime = 0;
    Color colorData;
    Vector3 scale;
    // Start is called before the first frame update
    Color color
    {
        get
        {
            if (isText)
                return GetComponent<Text>().color;
            else
                return GetComponent<Image>().color;
        }
        set
        {
            if (isText)
                GetComponent<Text>().color = value;
            else
                GetComponent<Image>().color = value;
        }
    }
    void SetOpaque(float f)
    {
        f = Mathf.Clamp(f, 0, 1);
        colorData.a = f;
        color = colorData;
    }
    void SetScale(float f)
    {
        scale = GetComponent<RectTransform>().localScale;
        scale.x = f;
        scale.y = f;
        scale.z = f;
        GetComponent<RectTransform>().localScale = scale;
    }
    void Start()
    {
        if (GetComponent<Text>() != null)
            isText = true;

        colorData = color;
        colorData.a = 0;
        color = colorData;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!anyKeyPressed)
                anyKeyPressed = true;
            else
                lifeTime = realiseTime * 2;
        }
        
        SetOpaque(1 - Mathf.Cos((lifeTime/realiseTime) * Mathf.PI) / 2 - 0.5f);
        SetScale(Mathf.Lerp(GetComponent<RectTransform>().localScale.x, endSize, Time.deltaTime / (2 * realiseTime)));

        lifeTime += Time.deltaTime;

        if (lifeTime > realiseTime && !anyKeyPressed)
            lifeTime = realiseTime;

        if (lifeTime > (realiseTime * 2))
            next.Invoke();
    }
}
