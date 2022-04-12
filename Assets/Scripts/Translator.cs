using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Translator : MonoBehaviour
{
    public static Translator instance;
    string path;
    [Serializable]
    public struct Item
    {
        public int key;
        public char[] value;
    }
    [Serializable]
    public class LocaleFile
    {
        public Item[] items;
        public LocaleFile(int c)
        {
            items = new Item[c];
        }
    }
    Dictionary<int, string> library;
    bool translatingNow;
    bool error = false;
    // Start is called before the first frame update
    void Awake()
    {

        var textAsset = Resources.Load<TextAsset>("localization");
        if (textAsset)
        {
            Debug.Log("Loading from Resources/ file");
            library = ToDictionary(JsonUtility.FromJson<LocaleFile>(textAsset.text));
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "Locale.poop");
#else
            path = Path.Combine(Application.dataPath, "Locale.poop");
#endif
            if (File.Exists(path))
                library = ToDictionary(JsonUtility.FromJson<LocaleFile>(File.ReadAllText(path)));
            else
                library = new Dictionary<int, string>();
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        translatingNow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (error)
        {
            Debug.Log("[][][]ERROR[][][]");
        }        
    }

    public static string Translate(string input, GameManager.Language lang, string message = null)
    {
        string from = "en";
        string to = GameManager.GetLocale(lang);
        var hash = (input + to).GetHashCode();        
        if (to == "en")
        {
            instance.library.Remove(hash);
            instance.library.Add(hash, input);
            if (message != null)
                Debug.Log(message);
            TranslatePhrase(input);            
            return input;
        }
        if (instance.library.TryGetValue(hash, out string value))
        {
            //if (message != null)
            //    Debug.Log(message);
            return value;
        }
        string url = String.Format
        ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
         from, to, Uri.EscapeUriString(input));

        instance.library.Add(hash, "");
        instance.StartCoroutine(instance.SimpleGetRequest(url, hash, message));

        return "Your request not in base, try later";
    }

    IEnumerator SimpleGetRequest(string url, int hash, string message)
    {
        while (translatingNow)
            yield return null;

        translatingNow = true;
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            error = true;
            Debug.Log(www.error.ToUpper());
            library.Remove(hash);
            translatingNow = false;
            yield break;
        }

        //while (jsonData == null)
        var Info = JSON.Parse(www.downloadHandler.text);

        // Extract just the first array element (This is the only data we are interested in)
        var translationItems = Info[0];

        // Translation Data
        string translation = "";

        // Loop through the collection extracting the translated objects
        foreach (var item in translationItems)
        {
            // Save its value (translated text)
            translation += string.Format(" {0}", Convert.ToString(item.Value[0].Value));
        }

        // Remove first blank character
        if (translation.Length > 1) { translation = translation.Substring(1); };

        // Return translation
        library.Remove(hash);
        library.Add(hash, translation);

        if (message != null)
            Debug.Log(message);
        translatingNow = false;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopAllCoroutines();
            ClearDictionary(library);
            File.WriteAllText(path, JsonUtility.ToJson(ToLocaleFile(library)));
        }
    }
#endif
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
        ClearDictionary(library);
        if (!string.IsNullOrEmpty(path))
            File.WriteAllText(path, JsonUtility.ToJson(ToLocaleFile(library)));
    }
    public void SaveData()
    {
        StopAllCoroutines();
        ClearDictionary(library);
        if (!string.IsNullOrEmpty(path))
            File.WriteAllText(path, JsonUtility.ToJson(ToLocaleFile(library)));
    }

    LocaleFile ToLocaleFile(Dictionary<int,string> source)
    {
        if (source == null)
        {
            Debug.Log("Dictionary is null");
            return null;
        }
        Debug.Log("Strings will be serialized: " + source.Count);
        var L = new LocaleFile(source.Count);
        var count = 0;
        foreach (var i in source)
        {
            L.items[count].key = i.Key;
            L.items[count].value = i.Value.ToCharArray();
            count++;
        }
        return L;
    }
    Dictionary<int, string> ToDictionary(LocaleFile source)
    {
        var D = new Dictionary<int, string>();
        for (int i = 0; i < source.items.Length; i++)
        {
            D.Add(source.items[i].key, FromCharArray(source.items[i].value));
        }

        return D;
    }
    string FromCharArray(char[] array)
    {
        string s = "";
        foreach (var i in array)
            s += i;

        return s;
    }

    public static void TranslateLevel()
    {
        var dialogs = instance.FindDialogs();
        int count = 0;
        Debug.Log("Найдено " + dialogs.Count + " диалогов");
        foreach (var i in dialogs)
            count += i.engPhrases.Length;
        count *= 14;
        int cur = 0;
        for (int i = 2; i < 16; i++)
        {
            for (int j = 0; j < dialogs.Count; j++)
            {
                for (int k = 0; k < dialogs[j].engPhrases.Length; k++)
                {
                    ++cur;
                    Translate(dialogs[j].engPhrases[k], (GameManager.Language)i, "Translating... (" + cur + "/" + count + ")");
                }
            }
        }
    }

    static void TranslatePhrase(string input)
    {
        for (int i = 2; i < 16; i++)
        {
            Translate(input, (GameManager.Language)i, input.Replace("\n", " ").Substring(0, Mathf.Min(10, input.Replace("\n", " ").Length)) + "... is translating (" + (i - 1) + "/" + 14 + ")");
        }
    }

    void ClearDictionary(Dictionary<int, string> D)
    {
        List<int> keys = new List<int>();
        foreach (var i in D)
        {
            if (i.Value == "")
                keys.Add(i.Key);
        }

        foreach (var i in keys)
            D.Remove(i);
    }

    List<dialog> FindDialogs(Transform trans = null)
    {
        
        if (trans == null)
        {
            List<dialog> Lm = new List<dialog>();
            foreach (var i in SceneManager.GetActiveScene().GetRootGameObjects())
                foreach (var j in FindDialogs(i.transform))
                    Lm.Add(j);

            return Lm;
        }

        List<dialog> L = new List<dialog>();
        for (int i = 0; i < trans.childCount; i++)
        {
            foreach (var j in FindDialogs(trans.GetChild(i)))
                L.Add(j);
        }

        dialog d;
        if ((d = trans.GetComponent<dialog>()) != null)
            L.Add(d);

        return L;
    }
}
