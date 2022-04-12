using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour
{
    [HideInInspector]
    public static AudioSystem instance;
    public AudioMixer mixer;
    public AudioClip[] clips;
    AudioSource audioSrc;
    // Start is called before the first frame update
    void Awake()
    {
        if ((audioSrc = gameObject.GetComponent<AudioSource>()) == null)
            audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSrc.ignoreListenerPause = true;

        instance = this;
    }
    public void PlaySound(int i)
    {

        audioSrc.clip = clips[i];
        audioSrc.Play();
    }

    public void PlaySound(AudioClip i, Vector3 pos)
    {
        var obj = new GameObject();
        obj.transform.position = pos;
        var a = obj.AddComponent<AudioSource>();
        obj.AddComponent<ControlSpatial>();
        a.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        a.clip = i;
        a.Play();
        StartCoroutine(DestroyObject(obj, i.length));
    }

    public void PlayClip(AudioClip i)
    {
        var obj = new GameObject();
        obj.name = i.name;
        var a = obj.AddComponent<AudioSource>();
        a.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        a.clip = i;
        a.Play();
        StartCoroutine(DestroyObject(obj, i.length));
    }

    IEnumerator DestroyObject(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }

    public void SetMixerSnapshot(string name)
    {
        GameManager.Instance.SetMixerSnapshot(name);
    }

    public void TranslateLevel()
    {
        Translator.TranslateLevel();
    }
}
