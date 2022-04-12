using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juggle : MonoBehaviour
{
    public float speed = 3;
    public float amplitude = 0.3f;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * speed) * amplitude/2, 0);
    }
}
