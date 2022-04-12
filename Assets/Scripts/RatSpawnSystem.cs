using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatSpawnSystem : MonoBehaviour
{
    public Sprite ratSprite;
    public float ratSpeed;
    public GameObject item; //Взять не префаб, а скопировать с объекта на сцене, предварительно его настроив
    public Transform[] holePositions;

    GameObject rat;
    int hole;
    float timeToRun = 0;
    bool enable = true;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    public void Disable()
    {
        enable = false;
    }

    void Handler(string message)
    {
        Debug.Log(message);
        StartCoroutine(ToHole());
    }

    IEnumerator ToHole()
    {
        yield return new WaitForSeconds(timeToRun);
        var startPos = rat.transform.position;
        float timeToArrival = (startPos - holePositions[hole].position).magnitude / ratSpeed;
        timeToArrival /= rat.transform.lossyScale.y;

        float curTime = Time.deltaTime;
        while (curTime < timeToArrival)
        {
            rat.transform.position = Vector3.Lerp(startPos, holePositions[hole].position, curTime/timeToArrival);
            yield return null;
            curTime += Time.deltaTime;
        }

        Destroy(rat);
        rat = null;

        if (enable == true)
            Spawn();

        timeToRun += 0.25f;
    }

    void Spawn()
    {
        var player = GameObject.Find("Player").transform;
        hole = Random.Range(0, holePositions.Length);
        var leftPos = holePositions[hole].parent.Find("leftRatPos").position;
        var rightPos = holePositions[hole].parent.Find("rightRatPos").position;
        var spawnPos = new Vector3(Random.Range(leftPos.x, rightPos.x), Random.Range(leftPos.y, rightPos.y), Random.Range(leftPos.z, rightPos.z));
        while((player.position - spawnPos).magnitude < 20)
        {
            hole = Random.Range(0, holePositions.Length);
            leftPos = holePositions[hole].parent.Find("leftRatPos").position;
            rightPos = holePositions[hole].parent.Find("rightRatPos").position;
            spawnPos = new Vector3(Random.Range(leftPos.x, rightPos.x), Random.Range(leftPos.y, rightPos.y), Random.Range(leftPos.z, rightPos.z));
        }

        rat = new GameObject();        
        rat.transform.position = spawnPos;
        rat.transform.parent = holePositions[hole].parent;
        rat.AddComponent<SpriteRenderer>().sprite = ratSprite;
        rat.AddComponent<BoxCollider>().isTrigger = true;
        rat.GetComponent<BoxCollider>().size = new Vector3(rat.GetComponent<BoxCollider>().size.x, rat.GetComponent<BoxCollider>().size.y, 2);

        var trigger = rat.AddComponent<SimpleTrigger>();
        trigger.targets = new GameObject[1];
        trigger.targets[0] = player.gameObject;
        trigger.Notify += Handler;


        float ratScale = 1.5f;
        if (rat.transform.position.x - holePositions[hole].position.x < 0)
            rat.transform.localScale = new Vector3(-ratScale, ratScale, ratScale);
        else
            rat.transform.localScale = new Vector3(ratScale, ratScale, ratScale);

        var inst = Instantiate(item);
        inst.transform.parent = rat.transform;
        inst.transform.localPosition = new Vector3(0, 0, -0.01f);
        inst.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        if (rat.transform.parent.GetComponent<RoomManager>() && rat.transform.parent.GetComponent<RoomManager>().enable == false)
            rat.transform.parent.GetComponent<RoomManager>().DisableRoom(rat.transform);

        rat.name = "Rat";
    }
}
