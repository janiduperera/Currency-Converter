using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdController : Singleton<AdController>
{
#if UNITY_IOS
    private string m_GameID = "3401617";
#elif UNITY_ANDROID
    private string m_GameID = "3401616";
#endif

    private string m_BannerAdPlacementID = "BannerAd";
    public bool IsTestMode;
    // Start is called before the first frame update
    void Start()
    {
        Advertisement.Initialize(m_GameID, IsTestMode);
        StartCoroutine(ShowBannerWhenReady());
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(m_BannerAdPlacementID))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(m_BannerAdPlacementID);
    }
}
