using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Safe : MonoBehaviour
{
    public GameObject outline;
    public float blinkTime = 0.5f;
    public UnityEvent success;
    public AudioClip[] buttons;
    public AudioClip successSound;
    Text text;
    string str = "";
    bool isInside = false;
    // Start is called before the first frame update
    ScreenKey[] screenKeys;
    void Start()
    {
        screenKeys = GetComponentsInChildren<ScreenKey>();
        text = transform.Find("Canvas").Find("Text").GetComponent<Text>();
        GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("Canvas").gameObject.SetActive(false);
        outline.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(CursorBlink());

        gameObject.AddComponent<AudioSource>();
        gameObject.AddComponent<ControlSpatial>().minDistanse = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInside)
        {
            var input = -1;
            foreach (var s in screenKeys)
                if (s.pressed)
                {
                    s.pressed = false;
                    input = s.inputValue;
                }

            if (input == -1)
                return;
            GetComponent<AudioSource>().PlayOneShot(buttons[Random.Range(0,buttons.Length)]);
            str += input.ToString();
            Check("592");
        }
    }

    void Check(string rightAnswer)
    {
        if (str == rightAnswer)
        {
            GetComponent<AudioSource>().PlayOneShot(successSound);
            success.Invoke();
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            transform.Find("Canvas").gameObject.SetActive(false);
            outline.GetComponent<SpriteRenderer>().enabled = false;
            isInside = false;
            return;
        }
        if (str.Length == 3)
        {
            StopAllCoroutines();
            text.text = str;
            isInside = false;
            AudioSystem.instance.PlaySound(2);
            outline.GetComponent<SpriteRenderer>().color = new Color(1,0,0,1);
            Invoke("Reset", 0.75f);
        }
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(CursorBlink());
        outline.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        str = "";
    }

    int ReadNumeric(string str)
    {
        if (str == "0")
            return 0;
        if (str == "1")
            return 1;
        if (str == "2")
            return 2;
        if (str == "3")
            return 3;
        if (str == "4")
            return 4;
        if (str == "5")
            return 5;
        if (str == "6")
            return 6;
        if (str == "7")
            return 7;
        if (str == "8")
            return 8;
        if (str == "9")
            return 9;
        return 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (IsInvoking())
            {
                CancelInvoke();
                Reset();
            }
            GetComponent<SpriteRenderer>().enabled = true;
            transform.Find("Canvas").gameObject.SetActive(true);
            outline.GetComponent<SpriteRenderer>().enabled = true;
            isInside = true;

            foreach (var s in screenKeys)
            {
                s.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            GetComponent<SpriteRenderer>().enabled = false;
            transform.Find("Canvas").gameObject.SetActive(false);
            outline.GetComponent<SpriteRenderer>().enabled = false;
            isInside = false;

            foreach (var s in screenKeys)
            {
                s.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator CursorBlink()
    {
        float time;
        while (true)
        {
            time = blinkTime;
            while (time > 0)
            {
                text.text = str + "|";
                yield return null;
                time -= Time.deltaTime;
            }
            time = blinkTime;
            while (time > 0)
            {
                text.text = str;
                yield return null;
                time -= Time.deltaTime;
            }
        }
    }
}
