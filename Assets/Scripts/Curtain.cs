using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour
{
    public float speed;
    public GameObject plane_L;
    public GameObject plane_R;
    Vector3 initialLocalL;
    Vector3 initialLocalR;
    public Camera cam;
    // Start is called before the first frame update
    void Awake()
    {
        initialLocalL = plane_L.transform.localPosition;
        initialLocalR = plane_R.transform.localPosition;
    }

    private void OnEnable()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var dir = new Vector3(speed * Time.deltaTime, 0, 0);
        plane_L.transform.localPosition += dir;
        plane_R.transform.localPosition -= dir;

        if (plane_L.transform.localPosition.x >= -4.5f || plane_R.transform.localPosition.x <= 4.5f)
            speed = 0;
    }
}
