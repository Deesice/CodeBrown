using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;
using System.Text;

public class dialog : MonoBehaviour
{
    public bool stopOnExit = true;
    public bool skipOnExit = false;
    public GameObject[] persons;
    public string[] rusPhrases;
    public string[] engPhrases;
    public AudioClip[] rusSounds;
    public AudioClip[] engSounds;

    public GameObject dialogCloud;
    public int maxStrLen;
    public UnityEvent Action;
    public AudioMixer mixer;
    Vector3 offset = new Vector3(0, 5.75f, 0);

    Vector2 cloudSize;
    int curPhrase = 0;
    GameObject curCloud;
    static int slot = 0;
    int bufForCurPhrase = -1;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var i in persons)
        {
            AudioSource audio;
            if ((audio = i.GetComponent<AudioSource>()) == null)
                audio = i.AddComponent<AudioSource>();
            audio.outputAudioMixerGroup = mixer.FindMatchingGroups("Voice")[0];
            audio.playOnAwake = false;
            if (i.GetComponent<ControlSpatial>() == null)
                i.AddComponent<ControlSpatial>();

            i.GetComponent<ControlSpatial>().minDistanse = 10;
            i.GetComponent<ControlSpatial>().maxDistanse = 30;
        }

    }
    public void ResetDialog()
    {
        curPhrase = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (skipOnExit && bufForCurPhrase >= 0)
            {
                curPhrase = bufForCurPhrase;
                bufForCurPhrase = -1;
            }
            if (curPhrase < rusPhrases.Length && curCloud == null)
                NextPhrase();
        }

        if (other.gameObject.name == "Player" && rusPhrases.Length == 0)
            Action.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (skipOnExit)
            {
                bufForCurPhrase = curPhrase;
                curPhrase = rusPhrases.Length - 1;
            }
            if (stopOnExit)
            {
                StopPhrase();
                if (skipOnExit)
                    Action.Invoke();
            }
        }
    }

    public void StopPhrase()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(ResetPhrase(0));
    }
    bool IsAsianLanguage()
    {
        return GameManager.Instance.data.language == GameManager.Language.JA ||
            GameManager.Instance.data.language == GameManager.Language.KO ||
            GameManager.Instance.data.language == GameManager.Language.ZH_CN ||
            GameManager.Instance.data.language == GameManager.Language.ZH_TW;
    }

    void NextPhrase()
    {
        string curStr;
        AudioClip[] langSounds;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.data.language == GameManager.Language.RU)
            {
                curStr = rusPhrases[curPhrase];
                langSounds = rusSounds;
            }
            else
            {
                curStr = Translator.Translate(engPhrases[curPhrase], GameManager.Instance.data.language);
                langSounds = engSounds;
            }
        }
        else
        {
            curStr = rusPhrases[curPhrase];
            langSounds = rusSounds;
        }


        int curLen = maxStrLen;
        if (IsAsianLanguage())
            curLen /= 4;

        if (curStr.Length <= maxStrLen)
            curLen = curStr.Length;
        else
            curStr = ModString(curStr, ref curLen);

        

        cloudSize.y = (curStr.Length - curStr.Replace("\n", "").Length) / "\n".Length + 1;
        cloudSize.y *= (1.0f / Power(cloudSize.y, 2) + 1.0f);
        cloudSize.x = curLen * 0.4f + 1;
        if (GameManager.Instance != null)
        {
            cloudSize *= GameManager.Instance.data.fontSize / 72.0f;
            if (GameManager.Instance.data.language == GameManager.Language.KO ||
                GameManager.Instance.data.language == GameManager.Language.JA ||
                GameManager.Instance.data.language == GameManager.Language.ZH_CN ||
                GameManager.Instance.data.language == GameManager.Language.ZH_TW)
            {
                cloudSize.x *= 1.7f;
                cloudSize.y *= 1.2f;
            }
        }

        int fontSize = GameManager.Instance != null ? GameManager.Instance.data.fontSize : 72;
        curCloud = CreateCloud(cloudSize, fontSize);
        // curCloud.transform.localScale *= persons[curPhrase % persons.Length].transform.lossyScale.y;
        curCloud.transform.localScale *= GameObject.Find("Player").transform.lossyScale.y;

        if (!IsAsianLanguage())
        {

            if (GameManager.Instance.data.language == GameManager.Language.RU)
            {
                curStr = rusPhrases[curPhrase];
                langSounds = rusSounds;
            }
            else
            {
                curStr = Translator.Translate(engPhrases[curPhrase], GameManager.Instance.data.language);
                langSounds = engSounds;
            }
        }

        curCloud.transform.Find("Canvas").transform.Find("Text").GetComponent<Text>().text = curStr;

        if (persons[curPhrase % persons.Length].GetComponent<Animator>())
            persons[curPhrase % persons.Length].GetComponent<Animator>().SetFloat("isTalking", 1.0f);

        StartCoroutine(ResetPhrase(langSounds[curPhrase].length));

        persons[curPhrase % persons.Length].GetComponent<AudioSource>().clip = langSounds[curPhrase];
        persons[curPhrase % persons.Length].GetComponent<AudioSource>().Play();
    }
    IEnumerator ResetPhrase(float phraseTime = 0)
    {
        if (phraseTime != 0)
            yield return new WaitForSeconds(phraseTime);

        if (curCloud != null)
        {
            Destroy(curCloud);
            foreach (var i in persons)
                if (i.GetComponent<Animator>() && i.GetComponent<Animator>().GetFloat("isTalking") == 1)
                {
                    i.GetComponent<Animator>().SetFloat("isTalking", 0);
                    i.GetComponent<AudioSource>().Stop();
                }
            curPhrase++;
            if (GetComponent<BoxCollider>().enabled)
            {
                GetComponent<BoxCollider>().enabled = false;
                GetComponent<BoxCollider>().enabled = true;
            }
            if (curPhrase >= rusPhrases.Length)
            {
                GetComponent<BoxCollider>().enabled = false;
                if (phraseTime > 0)
                {
                    Action.Invoke();
                    ResetDialog();
                }
            }
            StopAllCoroutines();
        }
    }

    float Power(float a, int b)
    {
        var buf = a;
        for (int i = 1; i < b; i++)
            a *= buf;

        return a;
    }

    GameObject CreateCloud(Vector2 cloudSize, int fontSize = 72)
    {
        var curCloud = Instantiate(dialogCloud, persons[curPhrase % persons.Length].transform.position + offset, dialogCloud.transform.rotation);

        curCloud.GetComponent<SpriteRenderer>().size = cloudSize;

        var canvas = curCloud.transform.Find("Canvas");
        canvas.GetComponent<RectTransform>().sizeDelta = cloudSize / canvas.GetComponent<RectTransform>().localScale.x;

        var text = canvas.transform.Find("Text");
        cloudSize.y -= 34 * canvas.GetComponent<RectTransform>().localScale.x;
        //text.GetComponent<RectTransform>().sizeDelta = cloudSize / canvas.GetComponent<RectTransform>().localScale.x;
        text.GetComponent<Text>().resizeTextMaxSize = fontSize;

        curCloud.GetComponent<Follow>().player = persons[curPhrase % persons.Length].transform;
        curCloud.GetComponent<Follow>().offset = new Vector3(offset.x, offset.y, 0) - new Vector3(0,0,7) + new Vector3(0, 0, -0.01f * slot + 0.05f);
        slot++;
        slot %= 10;

        return curCloud;
    }

    string ModString(string original, ref int maxChar)
    {
        List<int> indA = new List<int>();
        List<int> indB = new List<int>();
        int lastSpace = 0;
        original += ' ';
        var array = original.ToCharArray();
        int maxDiff = 0;
        int i;

        for (i = 0; i < array.Length; i++)
            if (array[i] == ' ' || array[i] == '\n' || array[i] == '、' || array[i] == '，' || array[i] == '。')
            {
                indA.Add(i);
            }

        for (i = 0; i < indA.Count; i++)
            if (indA[i] >= lastSpace + maxChar)
            {
                indB.Add(indA[i]);
                maxDiff = Mathf.Max(maxDiff, indA[i] - lastSpace - maxChar);
                lastSpace = indA[i] + 1;
            }

        for (i = 0; i < indB.Count - 1; i++)
        {
            original = original.Insert(indB[i] + 1, "\n");
        }

        if (indB[i] != original.Length - 1)
        {
            original = original.Insert(indB[i] + 1, "\n");
        }

        original = original.Remove(original.Length - 1, 1);

        maxChar += maxDiff;

        return original.Replace("\n\n", "\n");
    }
    public void DestroyObject()
    {
        StopAllCoroutines();
        StopPhrase();
        foreach (var i in persons)
            if (i.GetComponent<Animator>().GetFloat("isTalking") == 1)
                i.GetComponent<Animator>().SetFloat("isTalking", 0);
        Destroy(curCloud);
        Destroy(gameObject);
    }

    public void DestroyComponent()
    {
        StopAllCoroutines();
        StopPhrase();
        foreach (var i in persons)
            if (i.GetComponent<Animator>().GetFloat("isTalking") == 1)
                i.GetComponent<Animator>().SetFloat("isTalking", 0);
        Destroy(curCloud);
        Destroy(this);
    }
}
