using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableLanguage : MonoBehaviour
{
    public enum DataType { Image, Text, Int, Sound}
    public DataType dataType;
    public string category;
    public Sprite rus;
    public Sprite eng;
    public string strRus;
    public string strEng;
    public string strIt;
    public string strEs;
    public string strFr;
    public string strDe;
    public string strZh_TW;
    public string strZh_CN;
    public string strPt;
    public string strJa;
    public string strKo;
    public string strPl;
    public string strNl;
    public string strSv;
    public string strTr;
    public string strCs;
    public AudioClip rusSound;
    public AudioClip engSound;

    void OnEnable()
    {
        switch ((int)dataType)
        {
            case 0:
                LocalizeSprite();
                break;
            case 1:
                LocalizeText();
                break;
            case 2:
                GetComponent<Text>().text = ((int)GameManager.Instance.data[category]).ToString();
                break;
            case 3:
                LocalizeSound();
                break;
            default:
                break;
        }
    }

    void LocalizeSprite()
    {
        if (GameManager.Instance.data.language == GameManager.Language.RU)
            GetComponent<Image>().sprite = rus;
        else
            GetComponent<Image>().sprite = eng;
    }

    void LocalizeText()
    {
        if (strIt == "")
        {
            switch (GameManager.Instance.data.language)
            {
                case GameManager.Language.RU:
                    GetComponent<Text>().text = strRus.Replace("\\n", "\n");
                    break;
                default:
                    GetComponent<Text>().text = Translator.Translate(strEng.Replace("\\n", "\n"), GameManager.Instance.data.language);
                    break;
            }
        }
        else
        {
            switch (GameManager.Instance.data.language)
            {
                case GameManager.Language.CS:
                    GetComponent<Text>().text = strCs.Replace("\\n", "\n");
                    break;
                case GameManager.Language.DE:
                    GetComponent<Text>().text = strDe.Replace("\\n", "\n");
                    break;
                case GameManager.Language.EN:
                    GetComponent<Text>().text = strEng.Replace("\\n", "\n");
                    break;
                case GameManager.Language.ES:
                    GetComponent<Text>().text = strEs.Replace("\\n", "\n");
                    break;
                case GameManager.Language.FR:
                    GetComponent<Text>().text = strFr.Replace("\\n", "\n");
                    break;
                case GameManager.Language.IT:
                    GetComponent<Text>().text = strIt.Replace("\\n", "\n");
                    break;
                case GameManager.Language.JA:
                    GetComponent<Text>().text = strJa.Replace("\\n", "\n");
                    break;
                case GameManager.Language.KO:
                    GetComponent<Text>().text = strKo.Replace("\\n", "\n");
                    break;
                case GameManager.Language.NL:
                    GetComponent<Text>().text = strNl.Replace("\\n", "\n");
                    break;
                case GameManager.Language.PL:
                    GetComponent<Text>().text = strPl.Replace("\\n", "\n");
                    break;
                case GameManager.Language.PT:
                    GetComponent<Text>().text = strPt.Replace("\\n", "\n");
                    break;
                case GameManager.Language.RU:
                    GetComponent<Text>().text = strRus.Replace("\\n", "\n");
                    break;
                case GameManager.Language.SV:
                    GetComponent<Text>().text = strSv.Replace("\\n", "\n");
                    break;
                case GameManager.Language.TR:
                    GetComponent<Text>().text = strTr.Replace("\\n", "\n");
                    break;
                case GameManager.Language.ZH_CN:
                    GetComponent<Text>().text = strZh_CN.Replace("\\n", "\n");
                    break;
                case GameManager.Language.ZH_TW:
                    GetComponent<Text>().text = strZh_TW.Replace("\\n", "\n");
                    break;
            }
        }
    }
    void LocalizeSound()
    {
        if (GameManager.Instance.data.language == GameManager.Language.RU)
            GetComponent<AudioSource>().clip = rusSound;
        else
            GetComponent<AudioSource>().clip = engSound;
    }
}
