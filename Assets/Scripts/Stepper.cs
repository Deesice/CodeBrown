using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Stepper : MonoBehaviour
{
    public float timeToStep;
    public AudioClip[] clips;
    public AudioMixer mixer;
    float bufTime;
    AudioSource src;
    Vector3 curPos;
    // Start is called before the first frame update
    void Start()
    {
        bufTime = timeToStep;
        if (gameObject.name != "Тролль")
            timeToStep = Random.Range(0, bufTime);
        curPos = transform.position;
        var n = new GameObject();
        n.transform.position = gameObject.transform.position;
        n.transform.SetParent(gameObject.transform);
        n.name = "Stepper";
        
        src = n.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        src.reverbZoneMix = 1;
        n.AddComponent<ControlSpatial>();

        n.GetComponent<ControlSpatial>().minDistanse = 30 - 24;
        n.GetComponent<ControlSpatial>().maxDistanse = 30 + 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != curPos)
        {
            timeToStep -= Time.deltaTime;
            curPos = transform.position;
        }

        if (timeToStep < 0)
        {
            ResetTime();
            src.Stop();
            src.clip = clips[Random.Range(0, clips.Length)];
            src.Play();

            if (gameObject.name == "Player")
                GameManager.AddStep();
        }
    }
    public void SetSpatial(float min, float max)
    {
        src.GetComponent<ControlSpatial>().minDistanse = min;
        src.GetComponent<ControlSpatial>().maxDistanse = max;
    }
    public void SetSpatial(float min)
    {
        src.GetComponent<ControlSpatial>().minDistanse = min;
        src.GetComponent<ControlSpatial>().maxDistanse = min*10;
    }
    public void ResetTime()
    {
        timeToStep = bufTime;
    }
}
