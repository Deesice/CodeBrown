using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Soup : MonoBehaviour
{
    public int randomizeSince = 0;
    public Sprite[] sprites;
    public UnityEvent OnSuccess;
    public UnityEvent onFailConstant;
    public UnityEvent onFailRandom;    
    List<int> rightSequence = new List<int>();
    int cur = 0;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        for (; i < randomizeSince; i++)
        {
            rightSequence.Add(i);
        }
        for (; i < sprites.Length; i++)
        {
            int a = Random.Range(randomizeSince, sprites.Length);
            while (rightSequence.Contains(a))
                a = Random.Range(randomizeSince, sprites.Length);
            rightSequence.Add(a);
        }
        Print();
    }

    void Print()
    {
        string s = "";

        foreach (var i in rightSequence)
            s += i.ToString() + " ";

        //Debug.Log(s);
    }

    public void Check(SpriteRenderer item)
    {
        //OnSuccess.Invoke();
        if (item.sprite == sprites[rightSequence[cur]])
        {
            cur++;
            Debug.Log("RIGHT");
            if (cur == sprites.Length)
                OnSuccess.Invoke();
        }
        else
        {
            cur = 0;
            Debug.Log("WRONG");
            onFailConstant.Invoke();
            RandomOnFail();
        }
    }

    void RandomOnFail()
    {
        var index = Random.Range(0, onFailRandom.GetPersistentEventCount());
        var obj = onFailRandom.GetPersistentTarget(index);
        var str = onFailRandom.GetPersistentMethodName(index);

        Debug.Log(str);

        if (str == "SetActive")
            (obj as GameObject).SetActive(true);

        if (str == "set_enabled")
            (obj as BoxCollider).enabled = true;
    }

}
