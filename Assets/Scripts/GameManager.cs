using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReflectableClass
{
    public object this[string key]
    {
        get
        {
            return this.GetType().GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(this);
        }
        set
        {
            this.GetType().GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(this, value);
        }
    }

}

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance => _instance ? _instance : new GameObject("GameManager").AddComponent<GameManager>();
    [Serializable]
    public enum Language { EN, RU, IT, ES, FR, DE, ZH_TW, ZH_CN, PT, JA, KO, PL, NL, SV, TR, CS };
    [Serializable]
    public class Data : ReflectableClass
    {
        public Language language = Language.EN;
        public int volSFX = 10;
        public int volMusic = 10;
        public int volVoice = 10;
        public float firstTime = 0;
        public float secondTime = 0;
        public float thirdTime = 0;
        public float curTime = 0;
        public float preToiletSummaryTime = 0;
        public int totalSkill = 0;
        public int greatSkill = 0;
        public int goodSkill = 0;
        public bool[] firstGoals = { false, false, false, false, false };
        public bool[] secondGoals = { false, false, false, false, false };
        public bool[] thirdGoals = { false, false, false, false, false };
        public bool[] curGoals = { false, false, false, false, false };
        public bool firstKey = false;
        public bool secondKey = false;
        public bool thirdKey = false;
        public int fontSize = 72;
        public int birdAch = 0;
        public int skillAch = 0;
        public int stepAch = 0;
        public float totalTimeLeft = 7200;
        public int firstScore = 0;
        public int secondScore = 0;
        public int thirdScore = 0;
        public int curScore = 0;
    }
    public GameObject firstLogo;
    public GameObject message;
    [HideInInspector]
    public bool firstLaunch = false;
    public Data data = new Data();
    string path;
    AudioMixer mixer;
    string urlText;
    AnimationCurve soundsCurve;
    ushort secretKey = 0x0088;
    // Start is called before the first frame update
    private void Awake()
    {
        mixer = Resources.Load<AudioMixer>("mixer");
        gameObject.AddComponent<Translator>();
        _instance = this;
        soundsCurve = new AnimationCurve(new Keyframe[2] {
            new Keyframe(0, 0, Mathf.Tan(Mathf.Deg2Rad * 60), Mathf.Tan(Mathf.Deg2Rad * 60)),
            new Keyframe(1, 1, 0, 0)
        });
        //SteamUserStats.ResetAllStats(true);
        UnlockAchievement("00_thanks");
        DontDestroyOnLoad(this.gameObject);

#if UNITY_ANDROID && !UNITY_EDITOR
        string oldPath = Path.Combine(Application.persistentDataPath, "Save.poop");
        path = Path.Combine(Application.persistentDataPath, "savedata.poop");
#else
        string oldPath = Path.Combine(Application.dataPath, "Save.poop");
        path = Path.Combine(Application.dataPath, "savedata.poop");
#endif
        if (File.Exists(oldPath))
        {
            data = JsonUtility.FromJson<Data>(File.ReadAllText(oldPath));
            SaveData();
            File.Delete(oldPath);
        }
        if (File.Exists(path))
        {
            firstLaunch = false;
            data = JsonUtility.FromJson<Data>(EncodeDecrypt(File.ReadAllText(path), secretKey));
            firstLogo?.SetActive(true);
        }
        else
        {
            firstLaunch = true;
            switch (Application.systemLanguage)
            {
                case SystemLanguage.German:
                    data.language = Language.DE;
                    break;
                case SystemLanguage.Russian:
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Belarusian:
                    data.language = Language.RU;
                    break;
                case SystemLanguage.French:
                    data.language = Language.FR;
                    break;
                case SystemLanguage.Italian:
                    data.language = Language.IT;
                    break;
                case SystemLanguage.Korean:
                    data.language = Language.KO;
                    break;
                case SystemLanguage.Spanish:
                    data.language = Language.ES;
                    break;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    data.language = Language.ZH_CN;
                    break;
                case SystemLanguage.ChineseTraditional:
                    data.language = Language.ZH_TW;
                    break;
                case SystemLanguage.Japanese:
                    data.language = Language.JA;
                    break;
                case SystemLanguage.Portuguese:
                    data.language = Language.PT;
                    break;
                case SystemLanguage.Polish:
                    data.language = Language.PL;
                    break;
                case SystemLanguage.Swedish:
                    data.language = Language.SV;
                    break;
                case SystemLanguage.Czech:
                    data.language = Language.CS;
                    break;
                case SystemLanguage.Turkish:
                    data.language = Language.TR;
                    break;
                case SystemLanguage.Dutch:
                    data.language = Language.NL;
                    break;
                default:
                    data.language = Language.EN;
                    break;
            }
            message?.SetActive(true);
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        ResetVariable();
    }
    private void Update()
    {
        data.totalTimeLeft -= Time.unscaledDeltaTime;
        if (data.totalTimeLeft <= 0)
        {
            data.totalTimeLeft = 1000000;
            UnlockAchievementInstance("00_hour");
        }
    }
    private void Start()
    {
        CheckData();
    }
    public void ChangeLang()
    {
        int a = (int)data.language;
        a += 1;
        a %= 2;
        data.language = (Language)a;
    }

    public void NextLang()
    {
        int a = (int)data.language;
        a += 1;
        a %= 16;
        data.language = (Language)a;
    }
    public void PrevLang()
    {
        int a = (int)data.language;
        a += 15;
        a %= 16;
        data.language = (Language)a;
    }
    public void LoadSceneLatency(string name)
    {
        ResetVariable();
        StartCoroutine(loool(name));
    }
    public void LoadScene(string name)
    {
        ResetVariable();
        SceneManager.LoadScene(name);
    }
    static IEnumerator loool(string scName)
    {
        yield return new WaitForSeconds(1);
        //Debug.Log("Loading" + scName);
        if (scName == "_current")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(scName);
    }
    public void GameOver()
    {
        foreach (var i in GameObject.FindObjectsOfType<AudioSource>())
            i.Stop();
        foreach (var i in GameObject.FindObjectsOfType<Toilet>())
            i.StopAllCoroutines();
        foreach (var i in GameObject.FindObjectsOfType<dialog>())
        {
            i.GetComponent<BoxCollider>().enabled = false;
            i.StopPhrase();
        }

        UnlockAchievement("00_loser");
        AudioSystem.instance.PlaySound(4);
        var player = GameObject.Find("Player");
        foreach (var i in GameObject.FindGameObjectsWithTag("enemy"))
            if (i.GetComponent<NPCController>())
                i.GetComponent<NPCController>().GameOver(player.transform);

        player.GetComponent<PlayerController>().GameOver();
        Camera.main.GetComponent<PostProcessingAnimation>().GameOver();

        var bar = GameObject.Find("Прогресс_туалета(Clone)");
        Destroy(bar);

        var ui = GameObject.FindObjectOfType<PauseMenu>();
        ui.SetClockColor(new Color(0, 0, 0, 0));
        ui.state = PauseMenu.FlowState.GameOver;
    }

    public void Complete()
    {
        AudioSystem.instance.PlaySound(5);
        data.curScore = SaveResults();
        Camera.main.transform.Find("Curtain").gameObject.SetActive(true);
        var ui = GameObject.FindObjectOfType<PauseMenu>();
        ui.SetClockColor(new Color(0, 0, 0, 0));
        ui.state = PauseMenu.FlowState.Complete;

        ui.transform.Find("Stats").Find("Coefficient").GetComponent<Text>().text = ((int)(CalculateCoef() * 100)).ToString();
    }
    public void CheckData()
    {
        if (data.volSFX < 0)
            data.volSFX = 0;
        if (data.volSFX > 10)
            data.volSFX = 10;

        mixer.SetFloat("SFX", Mathf.Lerp(-50, 0, soundsCurve.Evaluate(data.volSFX / 10.0f)));

        if (data.volMusic < 0)
            data.volMusic = 0;
        if (data.volMusic > 10)
            data.volMusic = 10;

        mixer.SetFloat("Music", Mathf.Lerp(-50, 0, soundsCurve.Evaluate(data.volMusic / 10.0f)));

        if (data.volVoice < 0)
            data.volVoice = 0;
        if (data.volVoice > 10)
            data.volVoice = 10;

        mixer.SetFloat("Voice", Mathf.Lerp(-50, 0, soundsCurve.Evaluate(data.volVoice / 10.0f)));

        if (data.fontSize < 50)
            data.fontSize = 50;
        if (data.fontSize > 75)
            data.fontSize = 75;
    }

    void ResetVariable()
    {
        data.curTime = 0;
        data.greatSkill = 0;
        data.goodSkill = 0;
        data.totalSkill = 0;
        data.preToiletSummaryTime = 0;
        data.curScore = 0;
        for (int i = 0; i < 5; i++)
            data.curGoals[i] = false;
    }
    public float CalculateCoef()
    {
        if (data.totalSkill != 0)
            return ((float)data.greatSkill + (float)data.goodSkill*0.5f) / (float)data.totalSkill;
        else
            return 1;
    }
    int SaveResults()
    {
        data.curTime += data.greatSkill * 5 + (data.totalSkill - data.greatSkill - data.goodSkill) * (-10);
        int goalsCount = 0;
        switch(SceneManager.GetActiveScene().name)
        {
            case "Level1":
                if (data.curTime < data.firstTime || data.firstTime == 0)
                    data.firstTime = data.curTime;
                for (int i = 0; i < 5; i++)
                {
                    data.firstGoals[i] = data.firstGoals[i] || data.curGoals[i];
                    goalsCount += data.curGoals[i] ? 1 : 0;
                }
                break;
            case "Level2":
                if (data.curTime < data.secondTime || data.secondTime == 0)
                    data.secondTime = data.curTime;
                for (int i = 0; i < 5; i++)
                {
                    data.secondGoals[i] = data.secondGoals[i] || data.curGoals[i];
                    goalsCount += data.curGoals[i] ? 1 : 0;
                }
                break;
            case "Level3":
                if (data.curTime < data.thirdTime || data.secondTime == 0)
                    data.thirdTime = data.curTime;
                for (int i = 0; i < 5; i++)
                {
                    data.thirdGoals[i] = data.thirdGoals[i] || data.curGoals[i];
                    goalsCount += data.curGoals[i] ? 1 : 0;
                }
                break;
        }
        data.preToiletSummaryTime /= goalsCount;

        var curScore = (int)((150 - data.curTime) * (7.5f - data.preToiletSummaryTime) * (CalculateCoef() + 1) * 50);
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level1":
                if (curScore > data.firstScore)
                    data.firstScore = curScore;
                break;
            case "Level2":
                if (curScore > data.secondScore)
                    data.secondScore = curScore;
                break;
            case "Level3":
                if (curScore > data.thirdScore)
                    data.thirdScore = curScore;
                break;
        }

        return curScore;
    }
#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }
#endif
    private void OnApplicationQuit()
    {
        SaveData();
    }
    public void SaveData()
    {
        File.WriteAllText(path, EncodeDecrypt(JsonUtility.ToJson(data), secretKey));
    }

    public delegate void FloatFunc(float a);
    public delegate void BoolFunc(bool a);
    public void Invoke(FloatFunc f, float value, float time)
    {
        StartCoroutine(StartFloat(f, value, time));
    }
    IEnumerator StartFloat(FloatFunc f, float value, float time)
    {
        yield return new WaitForSeconds(time);
        f(value);
        Destroy(gameObject);
    }
    public void Invoke(BoolFunc f, bool value, float time)
    {
        StartCoroutine(StartBool(f, value, time));
    }
    IEnumerator StartBool(BoolFunc f, bool value, float time)
    {
        yield return new WaitForSeconds(time);
        f(value);
    }

    public void SetMixerSnapshot(string name)
    {
        AudioMixerSnapshot[] snap = new AudioMixerSnapshot[1];
        Debug.Log(mixer);
        snap[0] = mixer.FindSnapshot(name);
        float[] f = new float[1];
        f[0] = 1;
        mixer.TransitionToSnapshots(snap, f, 1.5f);
    }

    public void DestroyHelper(GameObject obj, float time = 0)
    {
        StartCoroutine(DestroyHelperCoroutine(obj, time));
    }
    IEnumerator DestroyHelperCoroutine(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }    
    public static string GetLocale(GameManager.Language lang)
    {
        switch (lang)
        {
            case Language.EN:
                return "en";
            case Language.RU:
                return "ru";
            case Language.CS:
                return "cs";
            case Language.DE:
                return "de";
            case Language.ES:
                return "es";
            case Language.FR:
                return "fr";
            case Language.IT:
                return "it";
            case Language.JA:
                return "ja";
            case Language.KO:
                return "ko";
            case Language.NL:
                return "nl";
            case Language.PL:
                return "pl";
            case Language.PT:
                return "pt";
            case Language.SV:
                return "sv";
            case Language.TR:
                return "tr";
            case Language.ZH_CN:
                return "zh-CN";
            case Language.ZH_TW:
                return "zh-TW";
            default:
                return "";
        }
    }

    public static void UnlockAchievement(string name)
    {
        return;
        //if (instance.GetComponent<SteamManager>().enabled && SteamUserStats.GetAchievement(name, out bool flag) && !flag)
        //{
        //    SteamUserStats.SetAchievement(name);
        //    SteamUserStats.StoreStats();
        //}
    }
    public void UnlockAchievementInstance(string name)
    {
        UnlockAchievement(name);
    }
    public static void AddBird()
    {
        if (Instance.data.birdAch < 10)
            Instance.data.birdAch++;

        if (Instance.data.birdAch == 10)
            UnlockAchievement("00_bird");
    }

    public static void AddStep()
    {
        if (Instance.data.stepAch < 1000)
            Instance.data.stepAch++;

        if (Instance.data.stepAch == 1000)
            UnlockAchievement("00_health");
    }
    public static void AddSkill()
    {
        if (Instance.data.skillAch < 10)
            Instance.data.skillAch++;

        if (Instance.data.skillAch == 10)
            UnlockAchievement("00_skill");
    }
    string EncodeDecrypt(string str, ushort secretKey)
    {
        var ch = str.ToCharArray(); //преобразуем строку в символы
        string newStr = "";      //переменная которая будет содержать зашифрованную строку
        foreach (var c in ch)  //выбираем каждый элемент из массива символов нашей строки
            newStr += TopSecret(c, secretKey);  //производим шифрование каждого отдельного элемента и сохраняем его в строку
        return newStr;
    }

    char TopSecret(char character, ushort secretKey)
    {
        character = (char)(character ^ secretKey); //Производим XOR операцию
        return character;
    }
}
