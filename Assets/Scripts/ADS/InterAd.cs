using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class InterAd : MonoBehaviour
{
    [SerializeField] string interstitialUnitId;
    private InterstitialAd interstitialAd;

    private string testInterstitialUnitId = "ca-app-pub-3940256099942544/8691691433";
    Canvas canvas;
    static InterAd instance;
    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Init()
    {
#if UNITY_EDITOR
        interstitialUnitId = testInterstitialUnitId;
#endif
        interstitialAd = new InterstitialAd(interstitialUnitId);
        interstitialAd.OnAdClosed += Load;
        Load(null, null);
    }
    private void Load(object sender, EventArgs e)
    {
        if (canvas)
            canvas.enabled = true;
        interstitialAd.Destroy();
        AdRequest adRequest = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(adRequest);
    }
    public static void ShowAd(Canvas canvasNeedToHide)
    {
        switch (Application.systemLanguage)
        {
#if !UNITY_EDITOR
            case SystemLanguage.Russian:
                UnityAdsManager.ShowInterstitial();
                return;
#endif
            default:
                if (instance.interstitialAd.IsLoaded())
                {

                    instance.canvas = canvasNeedToHide;
                    if (instance.canvas)
                        instance.canvas.enabled = false;
                    instance.interstitialAd.Show();
                }
                return;
        }
    }
}