using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BonusCloud : MonoBehaviour
{
    public AnimationCurve curve;
    public float finalSize = 1.2f;
    public float time = 1;
    public AudioClip clip;
    Text text;
    public void Disappear(string s, Color c)
    {
        text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        if (clip)
            AudioSystem.instance.PlayClip(clip);
        text.color = c;
        text.text = s;
        StartCoroutine(Disappearing());
    }
    public void DEBUG_Disappear()
    {
        if (clip)
            AudioSystem.instance.PlayClip(clip);
        StartCoroutine(Disappearing());
    }
    IEnumerator Disappearing()
    {
        float i = 0;
        Color start = text.color;
        Color final = new Color(start.r, start.g, start.b, 0);
        while (i < 1)
        {
            transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(finalSize, finalSize, finalSize), curve.Evaluate(i));
            text.color = Color.Lerp(start, final, curve.Evaluate(i));
            i += Time.deltaTime / time;
            yield return null;
        }
        Destroy(gameObject);
    }
}
