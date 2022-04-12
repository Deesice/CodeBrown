using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : MonoBehaviour, IUnityAdsListener
{
    double minGapISTime = 0;
    DateTime currentUnityAdGapTime;
    static UnityAdsManager instance;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    bool testMode = true;
#else
    bool testMode = false;
#endif
    [SerializeField] string gameId;
    [SerializeField] string interId;
    [SerializeField] string rewardId;
    public static event System.Action Rewarded;
    private void Awake()
    {
#if UNITY_EDITOR
        DestroyImmediate(this);
        return;
#endif
        if (instance || Application.systemLanguage != SystemLanguage.Russian)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        //minGapISTime = double.Parse(ExtensionMethods.GetContentByURL("https://deesice.github.io/unityiscooldown.txt"));
        currentUnityAdGapTime = DateTime.Now;
    }
    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        Advertisement.AddListener(this);
    }
    public static void ShowInterstitial()
    {
        if (Advertisement.IsReady()
            && (DateTime.Now - instance.currentUnityAdGapTime).TotalSeconds >= instance.minGapISTime)
        {
            instance.currentUnityAdGapTime = DateTime.Now;
            Advertisement.Show(instance.interId);
        }
    }
    public static void ShowRewarded()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show(instance.rewardId);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Time.timeScale = 0;
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        Time.timeScale = 1;
        if (placementId.Equals(rewardId) && showResult == ShowResult.Finished)
        {
            Rewarded?.Invoke();
        }
    }    
}
