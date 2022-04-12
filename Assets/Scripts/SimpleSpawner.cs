using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    public Vector3 offset;
    public GameObject prefab;

    public void Spawn()
    {
        var obj = Instantiate(prefab);
        obj.transform.position = transform.position + offset;

        foreach (var i in obj.transform.GetComponentsInChildren<SimpleTrigger>())
            i.targets[0] = transform.GetComponentsInChildren<SimpleTrigger>()[0].targets[0];
    }
}
