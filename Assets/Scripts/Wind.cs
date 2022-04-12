using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public float amplitude;
    public float speed;
    Cloth cloth;
    // Start is called before the first frame update
    void Start()
    {
        cloth = GetComponent<Cloth>();
    }

    // Update is called once per frame
    void Update()
    {
        cloth.externalAcceleration = new Vector3(Mathf.Sin(Time.time * speed) * amplitude, 0, 0);
    }
}
