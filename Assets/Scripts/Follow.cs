using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float distortionAmplitude = 0;
    public int distortionSoftness = 1;
    Vector3 origOffset;
    public float inertion;
    public float topBorder = 1000;
    IEnumerator curCoroutine;
    public event System.Action Updated;
    // Start is called before the first frame update
    Camera cam;
    bool isCam;
    void Start()
    {
        transform.position = player.position + offset * player.lossyScale.y;
        origOffset = offset;
        cam = GetComponent<Camera>();
        isCam = cam;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isCam)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 10.8f * player.lossyScale.y, Time.deltaTime * inertion);
        }
        if (inertion < 10)
        {
            transform.position = Vector3.Lerp(transform.position, player.position + offset * player.lossyScale.y, Time.deltaTime * inertion);
        }
        else
        {
                transform.position = player.position + player.lossyScale.y * offset;
        }
        if (transform.position.y > topBorder)
            transform.position += new Vector3(0, topBorder - transform.position.y, 0);

        Updated?.Invoke();
    }

    public void TopBorder(float t)
    {
        topBorder = t;
    }

    public void Distortion(bool b)
    {
        if (b && curCoroutine == null)
        {
            curCoroutine = JuggleOffset();
            StartCoroutine(curCoroutine);
        }
        else if (!b && curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
            curCoroutine = null;
            offset = origOffset;
        }
    }
    IEnumerator JuggleOffset()
    {
        while(true)
        {
            offset = new Vector3(origOffset.x + Random.Range(-distortionAmplitude, distortionAmplitude) * 2.5f / inertion,
                origOffset.y + Random.Range(-distortionAmplitude, distortionAmplitude) * 2.5f / inertion,
                origOffset.z);
            for (int i = 0; i < distortionSoftness; i++)
                yield return null;
        }
    }

    public void SetTarget(Transform t)
    {
        player = t;
    }

    public void SetInertion(float f)
    {
        inertion = f;
    }

    public void DistortionByTime(float time)
    {
        Distortion(true);
        Invoke("StopDistortion", time);
    }

    void StopDistortion()
    {
        Distortion(false);
    }
}
