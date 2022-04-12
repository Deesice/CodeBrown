using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float timeLeft;
    public UnityEvent Actions;
    [HideInInspector]
    public float startTime;
    [HideInInspector]
    public bool freeze = false;
    public bool unscaledTime = false;
    PauseMenu ui;
    // Start is called before the first frame update
    void Start()
    {
        startTime = timeLeft;
        if (gameObject.name == "Player")
            ui = GameObject.FindObjectOfType<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > startTime)
            timeLeft = startTime;

        if (!freeze)
            if (unscaledTime)
                timeLeft -= Time.unscaledDeltaTime;
            else
                timeLeft -= Time.deltaTime;

        if (ui)
        {
            ui.SetClock(timeLeft / startTime);

            if (timeLeft / startTime < 0.25f)
                GetComponent<Animator>().SetBool("alarm", true);
            else
                GetComponent<Animator>().SetBool("alarm", false);
        }

        if (timeLeft < 0)
        {
            timeLeft = 0;
            freeze = true;
            Actions.Invoke();
            enabled = false;
        }
    }
}
