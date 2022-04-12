using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    Camera mainCamera;
    Vector3 zeroCameraPosition;
    Vector3 zeroSelfPosition;
    Vector3 zeroSelfScale;
    public float distant;
    [SerializeField] bool _updatePosition = true;
    public bool UpdatePosition { get { return _updatePosition; } set { _updatePosition = value; } }
    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = Camera.main;
        mainCamera.GetComponent<Follow>().Updated += ApplyCameraPosition;
        SetDistant(distant);
    }

    // Update is called once per frame
    void ApplyCameraPosition()
    {
        if (!_updatePosition)
            return;
        transform.position = zeroSelfPosition + Vector3.Lerp(new Vector3(0,0,0), (mainCamera.transform.position - zeroCameraPosition) * Mathf.Sign(distant), Mathf.Abs(distant));
        transform.localScale = Vector3.Lerp(zeroSelfScale, zeroSelfScale * (Mathf.Sign(distant) == 1 ? mainCamera.orthographicSize / 10.8f : 10.8f / mainCamera.orthographicSize), Mathf.Abs(distant));
    }

    public void SetDistant(float f)
    {
        distant = f;
        zeroCameraPosition = mainCamera.transform.position;
        zeroSelfPosition = transform.position;
        zeroSelfScale = transform.localScale;
    }
}
