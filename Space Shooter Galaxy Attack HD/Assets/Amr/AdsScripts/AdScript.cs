/*using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using ShmupBaby;


public class AdScript : MonoBehaviour
{
public Button buttons;
//public Drop[] Drops;
public GameObject[] prefabsToDrop;
public string rewardedId = "ca-app-pub-3940256099942544/5224354917";
RewardedAd rewardedAd;
//public int maxAdRewardsPerButtonPerDay = 5;
//private List<int> adRewardsRemaining;

private void Start()
{
MobileAds.RaiseAdEventsOnUnityMainThread = true;
MobileAds.Initialize(initStatus => {});

}
#region Rewarded
public void LoadRewardedAd() {
if (rewardedAd!=null)
{
rewardedAd.Destroy();
rewardedAd = null;
}
var adRequest = new AdRequest();
adRequest.Keywords.Add("unity-admob-sample");
RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
{
if (error != null || ad == null)
{
print("Rewarded failed to load"+error);
return;
}
print("Rewarded ad loaded !!");
rewardedAd = ad;
RewardedAdEvents(rewardedAd);
});
}
public void ShowRewardedAd() {
if (rewardedAd != null && rewardedAd.CanShowAd())
{
rewardedAd.Show
((Reward reward) =>
{
    //GameObject prefabToDrop = prefabsToDrop(transform.position, transform.rotation);
    //Instantiate(prefabToDrop, transform.position, transform.rotation);
    LevelController.Instantiate(prefabsToDrop[UnityEngine.Random.Range(0, prefabsToDrop.Length)],transform.position, Quaternion.identity);
    
}
);
}
else {
print("Rewarded ad not ready");
}
}
public void RewardedAdEvents(RewardedAd ad)
{
// Raised when the ad is estimated to have earned money.
ad.OnAdPaid += (AdValue adValue) =>
{
Debug.Log("Rewarded ad paid {0} {1}."+
adValue.Value+
adValue.CurrencyCode);
};
// Raised when an impression is recorded for an ad.
ad.OnAdImpressionRecorded += () =>
{
Debug.Log("Rewarded ad recorded an impression.");
};
// Raised when a click is recorded for an ad.
ad.OnAdClicked += () =>
{
Debug.Log("Rewarded ad was clicked.");
};
// Raised when an ad opened full screen content.
ad.OnAdFullScreenContentOpened += () =>
{
Debug.Log("Rewarded ad full screen content opened.");
};
// Raised when the ad closed full screen content.
ad.OnAdFullScreenContentClosed += () =>
{
Debug.Log("Rewarded ad full screen content closed.");
};
// Raised when the ad failed to open full screen content.
ad.OnAdFullScreenContentFailed += (AdError error) =>
{
Debug.LogError("Rewarded ad failed to open full screen content " +
"with error : " + error);
};
}
#endregion
}*/