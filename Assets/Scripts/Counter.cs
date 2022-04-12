using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Counter : MonoBehaviour
{
    public int count = 2;
    public UnityEvent Actions;
    public void AddValue(int i)
    {
        count += i;
        if (count == 0)
            Actions.Invoke();
    }
}
