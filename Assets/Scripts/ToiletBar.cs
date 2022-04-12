using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletBar : MonoBehaviour
{
    GameObject player;
    Transform bar;
    Transform cursor;
    [HideInInspector]
    public Vector3 backup;
    [HideInInspector]
    public BoxCollider zone;
    public Vector3 offset;
    public float speed;
    public float sense;
    public float border = 2.35f;
    public float poopEfficiency = 1.0f;

    public AudioClip[] poopSounds;
    string cur;
    [HideInInspector]
    public float timeFromEnable;
    float lastMouseTime;
    int tapId;

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.Find("Шкала_1");
        cursor = transform.Find("Шкала_2");
        player = GameObject.Find("Player");
        transform.localScale = player.transform.lossyScale;
        transform.position = offset * transform.lossyScale.y + player.transform.position;
        player.GetComponent<PlayerController>().freeze = true;

        StartCoroutine(PlayPoopSounds());
        timeFromEnable = Time.time;
    }

    IEnumerator PlayPoopSounds()
    {
        var src = player.GetComponent<AudioSystem>();
        AudioClip clip;
        while (true)
        {
            clip = poopSounds[Random.Range(0, poopSounds.Length)];
            src.PlayClip(clip);
            cur = clip.name;
            yield return new WaitForSeconds(clip.length);
            cur = "";
        }
    }
    bool IsPlayerTryToInteract()
    {
        if (PlayerController.ExistTouch(TouchPhase.Began, out var touch) && Time.timeScale > 0)
        {
            lastMouseTime = Time.unscaledTime;
            tapId = touch.fingerId;
        }
        if (PlayerController.ExistTouch(TouchPhase.Ended, out touch) && tapId == touch.fingerId && Time.timeScale > 0 && Time.unscaledTime - lastMouseTime < 0.2f)
        {
            return true;
        }

        return Input.GetButtonDown("Jump");
    }
    void Update()
    {
        if (IsPlayerTryToInteract() && player.GetComponent<PlayerController>().curPoopValue > 0)
            cursor.position += new Vector3(0, sense * transform.lossyScale.y, 0);

        cursor.position += new Vector3(0, Time.deltaTime * speed * transform.lossyScale.y, 0);
        if (cursor.localPosition.y > border)
            cursor.localPosition = new Vector3(0, border, -0.01f);
        else if (cursor.localPosition.y < -border)
        {            
            player.GetComponent<Animator>().SetBool("poop", false);
            player.GetComponent<Timer>().freeze = false;
            player.transform.position = backup;
            Camera.main.GetComponent<Follow>().player = player.transform;
            zone.GetComponent<SimpleTrigger>().timeFromToilet = Time.time;
            player.GetComponent<PlayerController>().freeze = false;

            foreach (var i in FindObjectsOfType<AudioSource>())
                if (i.gameObject.name == cur)
                    Destroy(i.gameObject);

            Destroy(gameObject);
        }

        player.GetComponent<Animator>().SetFloat("speed", (cursor.localPosition.y + border)/(2*border));

        if (cursor.localPosition.y > border / 2)
            AddValue(poopEfficiency * -2);
        else if (cursor.localPosition.y > 0)
            AddValue(poopEfficiency * -1.5f);
        else
            AddValue(poopEfficiency * -1);

    }

    void AddValue(float value)
    {
        player.GetComponent<PlayerController>().curPoopValue += value * Time.deltaTime;
    }
}
