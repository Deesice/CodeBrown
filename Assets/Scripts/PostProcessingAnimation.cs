using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingAnimation : MonoBehaviour
{
    Animation anim;
    public Transform wasted;
    Color bufColor;
    float grain;
    //[SerializeField] PostProcessVolume mainVolume;
    [SerializeField] PostProcessVolume grainVolume;
    PostProcessLayer ppLayer;
    void Start()
    {
        ppLayer = GetComponent<PostProcessLayer>();
        ppLayer.enabled = false;
        anim = GetComponent<Animation>();
        if (!grainVolume)
        {
            enabled = false;
        }
    }
    public void GameOver()
    {
        ppLayer.enabled = true;
        anim.Play();
        StartCoroutine(SizeAnimation());
        Invoke("WastedSign", 2.25f);
    }
    void Update()
    {
        grainVolume.weight = (float)grain / 100;
    }

    IEnumerator SizeAnimation()
    {
        var cam = GetComponent<Camera>();
        var origSize = cam.orthographicSize;

        for (float i = 0; i <= 1; i += Time.deltaTime / 2.25f)
        {
            cam.orthographicSize = Mathf.Lerp(origSize*0.9f, origSize, i);
            yield return null;
        }
        cam.orthographicSize = origSize * 0.8f;
    }

    void WastedSign()
    {
        var blackBox = new GameObject();
        CreateQuadFrom(blackBox, GetComponent<Camera>().orthographicSize * GetComponent<Camera>().aspect * 4, GetComponent<Camera>().orthographicSize * 0.5f);
        blackBox.transform.position = transform.position + new Vector3(-GetComponent<Camera>().orthographicSize * GetComponent<Camera>().aspect, -GetComponent<Camera>().orthographicSize * 0.25f, GetComponent<Camera>().nearClipPlane + 0.1f);

        //var wasted = GameObject.Find("Canvas").transform.Find("Wasted");

        wasted.gameObject.SetActive(true);

        Invoke("WastedSignHelper", 4);
    }

    void WastedSignHelper()
    {
        //var wasted = GameObject.Find("Canvas").transform.Find("Wasted");
        wasted.Find("ReloadB (1)").gameObject.SetActive(true);
        wasted.Find("MenuB (1)").gameObject.SetActive(true);
    }
    void CreateQuadFrom(GameObject quad, float width = 1, float height = 1)
    {
        MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = quad.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;
    }

    //public void SetGrain(float f)
    //{
    //    grain = f;
    //}
    public void AddGrain(float f)
    {
        grain += f;
        ppLayer.enabled = grain > 0;
    }

    public void SetBlackBackground()
    {
        bufColor = GetComponent<Camera>().backgroundColor;
        GetComponent<Camera>().backgroundColor = Color.black;
    }
    public void ReturnBackground()
    {
        GetComponent<Camera>().backgroundColor = bufColor;
    }
}
