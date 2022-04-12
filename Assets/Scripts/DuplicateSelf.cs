using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateSelf : MonoBehaviour
{
    public Vector3 offset;
    public void Duplicate()
    {
        var obj = Instantiate(gameObject);
        obj.transform.position = transform.position + offset;
        
    }
}
