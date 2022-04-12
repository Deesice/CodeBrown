using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public AnimationCurve curve;
    public Transform[] positions;
    [HideInInspector]
    public int curPos;
    public float speed = 1;
    public 
    // Start is called before the first frame update
    void Start()
    {
        SetPositionIndex(0);
    }

    public void SetPositionIndex(int i)
    {
        curPos = i;
        StopAllCoroutines();
        if (gameObject.activeInHierarchy)
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        var start = transform.position;
        var startScale = transform.localScale;
        var startAngle = transform.rotation;
        for (float i = 0; i < 1; i += Time.deltaTime / speed)
        {
            transform.position = Vector3.Lerp(start, positions[curPos].position, curve.Evaluate(i));
            transform.localScale = Vector3.Lerp(startScale, positions[curPos].localScale, curve.Evaluate(i));
            transform.rotation = Quaternion.Lerp(startAngle, positions[curPos].rotation, curve.Evaluate(i));
            yield return null;
        }
        transform.position = positions[curPos].position;
        transform.localScale = positions[curPos].localScale;
        transform.rotation = positions[curPos].rotation;
    }

    public void Teleport(Transform dest)
    {
        StopAllCoroutines();
        transform.localScale = dest.localScale;
        transform.position = dest.position;
        transform.rotation = dest.rotation;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Move());
    }
}
