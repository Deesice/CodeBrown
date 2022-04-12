using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    public int keyNumber = 0;
    public bool onLock = false;
    public UnityEvent onAllKeys;
    // Start is called before the first frame update
    void Start()
    {
        if (!onLock)
            switch (keyNumber)
            {
                case 0:
                    if (GameManager.Instance.data.firstKey)
                        gameObject.SetActive(false);
                    break;
                case 1:
                    if (GameManager.Instance.data.secondKey)
                        gameObject.SetActive(false);
                    break;
                case 2:
                    if (GameManager.Instance.data.thirdKey)
                        gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        else
            switch (keyNumber)
            {
                case 0:
                    if (!GameManager.Instance.data.firstKey)
                        gameObject.SetActive(false);
                    break;
                case 1:
                    if (!GameManager.Instance.data.secondKey)
                        gameObject.SetActive(false);
                    break;
                case 2:
                    if (!GameManager.Instance.data.thirdKey)
                        gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        CheckKeys();
    }

    public void CheckKeys()
    {
        if (GameManager.Instance.data.firstKey && GameManager.Instance.data.secondKey && GameManager.Instance.data.thirdKey)
            onAllKeys?.Invoke();
    }
    public void AddKey()
    {
        switch (keyNumber)
        {
            case 0:
                GameManager.Instance.data.firstKey = true;
                break;
            case 1:
                GameManager.Instance.data.secondKey = true;
                break;
            case 2:
                GameManager.Instance.data.thirdKey = true;
                break;
            default:
                break;
        }
    }
}
