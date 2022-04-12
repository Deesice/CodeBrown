using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FaggotSign : MonoBehaviour
{
    public Text simpleText;
    public Text nickName;
    public float time = 2;
    float i = 0;
    Color startColor;
    Color finalColor;
    // Start is called before the first frame update
    string GetMyName()
    {
        return "";
    }
    void Start()
    {
        nickName.text = GetMyName();
        startColor = simpleText.color;
        finalColor = new Color(startColor.r, startColor.g, startColor.b, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (i <= 1)
        {
            simpleText.color = Color.Lerp(startColor, finalColor, i);
            nickName.color = Color.Lerp(startColor, finalColor, i);
            i += Time.deltaTime/time;
        }
    }
}
