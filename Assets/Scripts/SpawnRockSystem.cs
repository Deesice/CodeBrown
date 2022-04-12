using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRockSystem : MonoBehaviour
{
    public Sprite[] sprites;
    public Transform[] points;
    public bool isActive { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
            SpawnObject();
    }

    void SpawnObject()
    {
        if (points.Length < 2)
            return;

        int sector = Random.Range(0, points.Length - 1);
        Vector3 spawnPosition = Vector3.Lerp(points[sector].position, points[sector + 1].position, Random.Range(0.0f, 1.0f));

        var rock = new GameObject();
        rock.AddComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        rock.transform.position = spawnPosition;
        rock.AddComponent<Rigidbody>().AddTorque(new Vector3(0,0,Random.Range(-10.0f, 10.0f)), ForceMode.Impulse);
        StartCoroutine(DestroyObject(rock, 5));
    }
    IEnumerator DestroyObject(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
}
