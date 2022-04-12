using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

public class Pooling : MonoBehaviour
{
    public enum PoolType {Time, Goals, Key, Bool, Coefficient, Score, Medal };
    public PoolType poolType;
    public string dataName;
    public Sprite bronze;
    public Sprite silver;
    public Sprite gold;
    
    // Start is called before the first frame update
    void Start()
    {
        switch((int)poolType)
        {
            case 0:
                PoolTime();
                break;
            case 1:
                PoolGoals();
                break;
            case 2:
                PoolKey();
                break;
            case 3:
                PoolBool();
                break;
            case 4:
                PoolCoefficient();
                break;
            case 5:
                PoolScore();
                break;
            case 6:
                PoolMedal();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    public void PoolTime()
    {
        var f = (float)GameManager.Instance.data[dataName];
        int seconds = (int)f;
        f -= seconds;
        f *= 100;
        int milliseconds = (int)f;
        int minutes = seconds / 60;
        seconds %= 60;

        if (seconds + minutes + milliseconds == 0)
            GetComponent<Text>().text = "--/--/--";
        else
            GetComponent<Text>().text =
            (minutes < 10 ? "0" : "") +
            minutes.ToString() + ":" +
            (seconds < 10 ? "0" : "") +
            seconds.ToString() + ":" +
            (milliseconds < 10 ? "0" : "") +
            milliseconds.ToString();
    }
    public void PoolScore()
    {
        var f = (int)GameManager.Instance.data[dataName];
        GetComponent<Text>().text = f.ToString();
    }
    public void PoolMedal()
    {
        var f = (int)GameManager.Instance.data[dataName];

        GetComponent<Image>().enabled = false;
        if (f > 0)
        {
            GetComponent<Image>().enabled = true;
            GetComponent<Image>().sprite = bronze;
        }
        if (f >= 40000)
            GetComponent<Image>().sprite = silver;
        if (f >= 80000)
            GetComponent<Image>().sprite = gold;
    }

    public void PoolGoals()
    {
        var b = (bool[])GameManager.Instance.data[dataName];
        int counter = 0;
        foreach (var i in b)
            if (i)
                counter++;

        GetComponent<Text>().text = counter.ToString() + "/5";
    }
    public void PoolKey()
    {
        var b = (bool)GameManager.Instance.data[dataName];

        if (b)
            GetComponent<Image>().enabled = true;
    }
    public void PoolBool()
    {
        var yes = GameObject.Find("Canvas").transform.Find("Галочка").GetComponent<Image>().sprite;
        var no = GameObject.Find("Canvas").transform.Find("Крестик").GetComponent<Image>().sprite;
        if (GameManager.Instance.data.curGoals[int.Parse(dataName)])
            GetComponent<Image>().sprite = yes;
        else
            GetComponent<Image>().sprite = no;
    }

    public void PoolCoefficient()
    {
        GetComponent<Text>().text = Mathf.FloorToInt(GameManager.Instance.CalculateCoef() * 100).ToString();
    }
}
