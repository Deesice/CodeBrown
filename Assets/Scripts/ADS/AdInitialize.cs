using UnityEngine;
using GoogleMobileAds.Api;

public class AdInitialize : MonoBehaviour
{
    private void Start()
    {
#if !UNITY_EDITOR
        if (Application.systemLanguage != SystemLanguage.Russian)
#endif
            MobileAds.Initialize(InitializationHandler);
    }
    void InitializationHandler(InitializationStatus status)
    {
        foreach (var i in FindObjectsOfType<InterAd>())
            i.Init();
    }
}