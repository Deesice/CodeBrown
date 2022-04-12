using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float speed;
    public Transform leftBorder;
    public Transform rightBorder;
    public GameObject prefab;
    public AudioClip[] clips;    
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("offset", Random.Range(0.0f, 1.0f));

        var scale = new Vector3((Random.Range(0, 2) - 0.5f) * 3, 1.5f, 1.5f);

        transform.localScale = scale;

        speed = Random.Range(30.0f, 50.0f);

        GetComponent<AudioSource>().clip = clips[Random.Range(0,clips.Length)];
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            GameManager.AddBird();
            FlyAway();
        }
    }

    void FlyAway()
    {
        GetComponent<AudioSource>().Play();
        Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f), 0);
        direction = direction.normalized;
        if (direction.x > 0)
            transform.localScale = new Vector3(-1.5f, 1.5f, 1.5f);
        else
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        StartCoroutine(Moving(direction));
        Invoke("DestroyObject", 2);
    }
    IEnumerator Moving(Vector3 direction)
    {
        while (true)
        {
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
    }

    void DestroyObject()
    {
        var obj = Instantiate(prefab);
        var pos = new Vector3(Random.Range(leftBorder.position.x, rightBorder.position.x), Random.Range(0.5f, 1.0f), -0.1f);
        var player = GameObject.Find("Player").transform;
        while ((player.position - pos).magnitude < 40)
            pos = new Vector3(Random.Range(leftBorder.position.x, rightBorder.position.x), Random.Range(0.5f, 1.0f), -0.1f);

        obj.transform.position = pos;
        obj.GetComponent<Bird>().leftBorder = leftBorder;
        obj.GetComponent<Bird>().rightBorder = rightBorder;
        Destroy(gameObject);
    }
}
