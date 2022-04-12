using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
{
    Image sp;

    public UnityEvent onClick;
    public UnityEvent onFirstLaunch;
    public Sprite def;
    public Sprite onEnter;
    public Sprite onDown;
    public float latency = 0.1f;
    public int value;
    void Awake()
    {
        sp = GetComponent<Image>();
    }

    void OnDisable()
    {
        sp.sprite = def;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AudioSystem.instance.PlaySound(12);
        sp.sprite = onDown;
        Invoke("Action", latency);
    }
    void Action()
    {
        onClick.Invoke();
        if (GameManager.Instance.firstLaunch)
            onFirstLaunch.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        sp.sprite = onEnter;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioSystem.instance.PlaySound(11);
        sp.sprite = onEnter;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        sp.sprite = def;
    }
    public void LoadScene(string scName)
    {
        AudioSystem.instance.PlaySound(13);
        LoadingCurtain.HideScene();

        GameManager.Instance.LoadSceneLatency(scName);
    }
    public void ChangeLang()
    {
        int a = (int)GameManager.Instance.data.language;
        a += 1;
        a %= 2;
        GameManager.Instance.data.language = (GameManager.Language)a;
    }

    public void NextLang()
    {
        GameManager.Instance.NextLang();
    }
    public void PrevLang()
    {
        GameManager.Instance.PrevLang();
    }
    public void AddValue(int category)
    {
        switch (category)
        {
            case 0:
                GameManager.Instance.data.volSFX += value;
                GameManager.Instance.CheckData();
                break;
            case 1:
                GameManager.Instance.data.volMusic += value;
                GameManager.Instance.CheckData();
                break;
            case 2:
                GameManager.Instance.data.volVoice += value;
                GameManager.Instance.CheckData();
                break;
            case 3:
                GameManager.Instance.data.fontSize += value;
                GameManager.Instance.CheckData();
                break;
            default: break;
        }
    }
    public void OpenURL(string link)
    {
        Application.OpenURL(link);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ResetFirstLaunch()
    {
        GameManager.Instance.firstLaunch = false;
    }
    public void ShowIS()
    {
        InterAd.ShowAd(null);
    }
}
