using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCurtain : MonoBehaviour
{
    new RectTransform transform;
    Vector3 initialPosition;
    static LoadingCurtain instance;
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
        image.enabled = true;
        instance = this;
        transform = GetComponent<RectTransform>();
    }
    public static void ShowScene()
    {
        instance.transform.position = instance.initialPosition;
        instance.StartCoroutine(instance.MovingTo(instance.initialPosition + Vector3.left * 2000, 0.5f));
        instance.GetComponent<AudioSource>().Play();
    }
    public static void HideScene()
    {
        instance.transform.position = instance.initialPosition + Vector3.right * 2000;
        instance.StartCoroutine(instance.MovingTo(instance.initialPosition, 0.5f));
    }
    void Start()
    {
        initialPosition = transform.position;
        ShowScene();
    }
    IEnumerator MovingTo(Vector3 endPosition, float time)
    {
        float i = 0;
        var startPosition = transform.position;
        yield return null;
        while (i < 1)
        {
            yield return null;
            i += Time.unscaledDeltaTime / time;
            transform.position = Vector3.Lerp(startPosition, endPosition, i);
        }
        yield return null;
    }
}
