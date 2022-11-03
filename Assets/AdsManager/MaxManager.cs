using AppsFlyerSDK;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaxManager : MonoBehaviour {
    public static MaxManager ins;

    private const string MaxSdkKey = "ZoNyqu_piUmpl33-qkoIfRp6MTZGW9M5xk1mb1ZIWK6FN9EBu0TXSHeprC3LMPQI7S3kTc1-x7DJGSV8S-gvFJ";

    [Header("Android ID")]
    private const string InterstitialAdUnitId = "a2a2bd9857af684e";
    private const string RewardedAdUnitId = "6e6e8e67c75e2793";
    private const string BannerAdUnitId = "69f7a6651a67773d";

    [Header("IOS ID")]
    private const string InterstitialAdUnitId_IOS = "6ae41ea9b7e09758";
    private const string RewardedAdUnitId_IOS = "bfd498e74dbe3bbd";
    private const string BannerAdUnitId_IOS = "3b4eebf78a972bb1";

    [Header("Capping Time")]
    public double timeWatchAds = 30;
    public double timeWatchAdsInter = 30;

    [Header("Action Event")]
    Action OnRewardAds_Finish, OnRewardAds_Fail;
    Action OnInter_Finish;

    [Header("Status")]
    [HideInInspector] public bool isBannerShowing;
    [HideInInspector] public bool isMRecShowing;
    [HideInInspector] public bool isVideoLoaded;

    public bool recieveReward = false;
    public bool showingAds = false;
    public bool isStartWaitting_Inter = false;//Bắt đầu đợi show Inter 
    public bool isStartWaitting_Reward = false;//Bắt đầu đợi show Reward
    public bool isInterLastSuccess = false;//Cái Inter cuối cùng show thành công hay ko?
    public bool isIOS = false;

    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;

    #region SETUP
    private void Awake() {
        if (ins != null) {
            Destroy(this.gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(this.gameObject);

#if UNITY_IOS || UNITY_IPHONE
        isIOS = true;
#endif
    }

    public void Init() {
        try {
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration => {
                // AppLovin SDK is initialized, configure and start loading ads.
                Debug.Log("MAX SDK Initialized");
                InitializeInterstitialAds();
                InitializeRewardedAds();
                InitializeBannerAds();
                //MaxSdk.ShowMediationDebugger();
            };
            MaxSdk.SetUserId(AppsFlyer.getAppsFlyerId());
            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi Init MaxManager:" + ex);
        }
    }

    private void FixedUpdate() {
        timeWatchAds += Time.deltaTime;
        timeWatchAdsInter += Time.deltaTime;
    }
    #endregion

    #region Interstitial Ad Methods
    private void InitializeInterstitialAds() {
        try {
            // Attach callbacks

            //Load inter ads thành công
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;

            //Load inter ads fail
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;

            //Hiển thị fail
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;

            //Ấn nút tắt ads
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;

            //Click vào ads
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;

            //Giá trị của ads
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

            //Ads hiển thị thành công
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;

            // Load the first interstitial
            LoadInterstitial();
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi InitializeInterstitialAds:" + ex);
        }
    }

    public void LoadInterstitial() {
        try {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            //Cái này mới thêm, cần test lại đã
            if (IsLoadInterstitial()) return;
            MaxSdk.LoadInterstitial(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId);
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi LoadInterstitial:" + ex);
        }
    }

    public void ShowInterstitial(string placement = "", Action OnFinish = null, bool isSkip = false) {
        try {
            isStartWaitting_Inter = true;
            isInterLastSuccess = false;
            bool isFail_CappingTimeInter = timeWatchAds < PluginManager.ins.Capping_Inter || timeWatchAdsInter <PluginManager.ins.Capping_Inter;
            Debug.LogWarning("Ko đủ ĐK show Inter" + Application.isEditor + "_"+ DataManager.ins.gameSave.isNoAds+"_"+ GameManager.ins.isRemoveAllAds +"_" + isFail_CappingTimeInter +"_"+ isSkip);
            if (Application.isEditor
                || DataManager.ins.gameSave.isNoAds
                || GameManager.ins.isRemoveAllAds
                || isFail_CappingTimeInter
                || isSkip) {
                isStartWaitting_Inter = false;
                isInterLastSuccess = true;
                OnFinish?.Invoke();
                return;
            }

            OnInter_Finish = OnFinish;
            FirebaseManager.ins.ads_inter_click();
            if (MaxSdk.IsInterstitialReady(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId)) {
                //Log Event
                AppsflyerEventRegister.af_inters_ad_eligible();
                showingAds = true;
                timeWatchAdsInter = 0;
                DataManager.ins.gameSave.amountWin_AfterShowInter = 0;
                DataManager.ins.gameSave.amountLose_AfterShowInter = 0;
                FirebaseManager.ins.ads_inter_start_show();
                MaxSdk.ShowInterstitial(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId);
            } else {
                Debug.LogError("Lỗi chưa load đc Inter");
                isStartWaitting_Inter = false;
                isInterLastSuccess = false;
                LoadInterstitial();
                OnInter_Finish?.Invoke();
            }
        } catch (Exception e) {
            Debug.LogError("Lỗi Inter: " + e);
            isStartWaitting_Inter = false;
            isInterLastSuccess = false;
            OnInter_Finish?.Invoke();
        }
    }

    public bool IsLoadInterstitial() {
        return MaxSdk.IsInterstitialReady(isIOS ? InterstitialAdUnitId_IOS : InterstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Reset retry attempt
        interstitialRetryAttempt = 0;
        Debug.Log("Interstitial loaded");

        //Log Event
        FirebaseManager.ins.ads_inter_load();
        AppsflyerEventRegister.af_inters_api_called();
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, interstitialRetryAttempt));
        Invoke("LoadInterstitial", (float)retryDelay);
        Debug.Log("Interstitial failed to load with error code: " + errorInfo.ToString());

        //Log Event
        FirebaseManager.ins.ads_inter_fail("Load Fail: " + errorInfo.ToString());
    }

    private void OnInterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) {
        // Interstitial ad failed to display. We recommend loading the next ad
        //DebugCustom.Log("Interstitial failed to display with error code: " + errorCode);
        isStartWaitting_Inter = false;
        isInterLastSuccess = false;
        LoadInterstitial();
        OnInter_Finish?.Invoke();
        Debug.Log("Interstitial failed to display with error code: " + errorInfo.ToString());

        //Log Event
        FirebaseManager.ins.ads_inter_fail("Display Fail: " + errorInfo.ToString());
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Interstitial ad is hidden. Pre-load the next ad
        isStartWaitting_Inter = false;
        isInterLastSuccess = true;
        timeWatchAdsInter = 0;
        LoadInterstitial();
        OnInter_Finish?.Invoke();
        Debug.Log("Interstitial dismissed");
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Reset retry attempt
        isStartWaitting_Inter = false;
        isInterLastSuccess = true;
        interstitialRetryAttempt = 0;

    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Reset retry attempt
        isStartWaitting_Inter = false;
        isInterLastSuccess = true;
        interstitialRetryAttempt = 0;
        Debug.Log("Interstitial Displayed");

        //Log Event
        FirebaseManager.ins.ads_inter_displayed();
        AppsflyerEventRegister.af_inters_displayed();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Interstitial revenue paid");

        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "interstitial";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.ins.ADS_RevenuePain(data);
    }

    #endregion

    #region Rewarded Ad Methods
    private string _rewardPlacement;
    private string _reward;
    private void InitializeRewardedAds() {
        try {
            // Attach callbacks
            //Reward ads load thành công
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;

            //Reward ads Load thất bại
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;

            //Reward ads show thất bại
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;

            //Reward ads Show thành công
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;

            //Click vào Reward ads 
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;

            //Tắt ads
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;

            //Phần thưởng có thể nhận được (nên dùng event tắt ads)
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            //Doanh thu
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

            // Load the first RewardedAd
            LoadRewardedAd();
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi InitializeRewardedAds:" + ex);
        }
    }

    public bool isRewardedVideoAvailable() {
        return MaxSdk.IsRewardedAdReady(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId);
    }

    public void LoadRewardedAd() {
        try {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            //Có sẵn r thì ko cần load
            //Phòng trường hợp load liên tục
            if (isRewardedVideoAvailable()) return;

            isVideoLoaded = false;
            MaxSdk.LoadRewardedAd(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId);
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi LoadRewardedAd:" + ex);
        }
    }

    public bool ShowRewardedAd(string placement = "", string reward = "", Action OnFinish = null, Action OnFail = null) {
        _rewardPlacement = placement;
        _reward = reward;
        try {
            isStartWaitting_Reward = true;
            OnRewardAds_Finish = OnFinish;
            OnRewardAds_Finish += () => { FirebaseManager.ins.ads_reward_complete(placement, reward); };
            OnRewardAds_Fail = OnFail;
            recieveReward = false;
            if (Application.isEditor || GameManager.ins.isRemoveAllAds) {
                isStartWaitting_Reward = false;
                OnRewardAds_Finish?.Invoke();
                return true;
            }
            FirebaseManager.ins.ads_reward_click(_rewardPlacement, reward);
            if (MaxSdk.IsRewardedAdReady(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId)) {
                //Event Log
                AppsflyerEventRegister.af_rewarded_ad_eligible();

                showingAds = true;
                Dictionary<string, string> value = new Dictionary<string, string>();
                FirebaseManager.ins.ads_reward_start_show(_rewardPlacement, reward);
                MaxSdk.ShowRewardedAd(isIOS ? RewardedAdUnitId_IOS : RewardedAdUnitId);
                return true;
            } else {
                Debug.LogError("Lỗi chưa load đc Video Ads");
                isStartWaitting_Reward = false;
                showingAds = false;
                LoadRewardedAd();
                OnRewardAds_Fail?.Invoke();
                if(SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false) 
                    SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
                return false;
            }
        } catch (Exception e) {
            Debug.LogError("Lỗi VideoAds: " + e);
            isStartWaitting_Reward = false;
            showingAds = false;
            OnRewardAds_Fail?.Invoke();
            if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false)
                SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
            return false;
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Debug.Log("Rewarded ad loaded");
        isVideoLoaded = true;
        // Reset retry attempt
        rewardedRetryAttempt = 0;

        //Log Event
        AppsflyerEventRegister.af_rewarded_api_called();
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) {
        isVideoLoaded = false;
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, rewardedRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
        Debug.Log("Rewarded ad failed to load with error code: " + errorInfo.ToString());

        //Event Log
        FirebaseManager.ins.ads_reward_fail(_rewardPlacement, _reward, "Load Fail: " + errorInfo.ToString());
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad failed to display. We recommend loading the next ad
        isStartWaitting_Reward = false;
        LoadRewardedAd();
        OnRewardAds_Fail?.Invoke();
        OnRewardAds_Fail = null;
        showingAds = false;
        Debug.Log("Rewarded ad failed to display with error code: " + errorInfo.ToString());

        //Event Log
        FirebaseManager.ins.ads_reward_fail(_rewardPlacement, _reward , "Display Fail: " + errorInfo.ToString());
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        recieveReward = false;
        isStartWaitting_Reward = false;
        Debug.Log("Rewarded ad displayed");

        //Event Log
        FirebaseManager.ins.ads_reward_displayed(_rewardPlacement, _reward);
        AppsflyerEventRegister.af_rewarded_displayed();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        isStartWaitting_Reward = false;
        Debug.Log("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        isStartWaitting_Reward = false;
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        if (recieveReward) {
            recieveReward = false;
            timeWatchAds = 0;
            OnRewardAds_Finish?.Invoke();
            OnRewardAds_Finish = null;
        } else {
            OnRewardAds_Fail?.Invoke();
            OnRewardAds_Fail = null;
            timeWatchAds = 0;
        }
        showingAds = false;
        Debug.Log("Rewarded ad dismissed");
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad was displayed and user should receive the reward
        recieveReward = true;
        Debug.Log("Rewarded ad received reward");

        //Event Log
        AppsflyerEventRegister.af_rewarded_ad_completed();
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad revenue paid. Use this callback to track user revenue.


        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "video_reward";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.ins.ADS_RevenuePain(data);
        Debug.Log("Rewarded ad revenue paid");
    }

    #endregion

    #region Banner Ad Methods
    private void InitializeBannerAds() {
        try {
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
            MaxSdk.CreateBanner(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional.
            MaxSdk.SetBannerBackgroundColor(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId, Color.black);
        } catch (Exception ex) {
            Debug.LogError("Lỗi InitializeBannerAds:" + ex);
        }

    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        // Rewarded ad revenue paid. Use this callback to track user revenue.


        // Ad revenue
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        var data = new ImpressionData();
        data.AdFormat = "Banner";
        data.AdUnitIdentifier = adUnitIdentifier;
        data.CountryCode = countryCode;
        data.NetworkName = networkName;
        data.Placement = placement;
        data.Revenue = revenue;

        FirebaseManager.ins.ADS_RevenuePain(data);
        Debug.Log("Banner ad revenue paid");
    }


    public void ReloadBanner() {
        if (DataManager.ins.gameSave.isNoAds || GameManager.ins.isRemoveAllAds) {
            if (isBannerShowing) HideBanner();
        } else {
            if (!isBannerShowing) ShowBanner();
        }
    }

    public void ShowBanner()
    {
        try {
            if(Application.isEditor) return;
            if (DataManager.ins.gameSave.isNoAds || GameManager.ins.isRemoveAllAds) return;
            MaxSdk.ShowBanner(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId);
            isBannerShowing = true;
        } catch (Exception ex) {
            Debug.LogError("Lỗi ShowBanner:" + ex);
        }

    }

    public void HideBanner()
    {
        try {
            MaxSdk.HideBanner(isIOS ? BannerAdUnitId_IOS : BannerAdUnitId);
            isBannerShowing = false;
        } catch (Exception ex) {
            Debug.LogError("Lỗi HideBanner:" + ex);
        }
    }
    #endregion
}

[System.Serializable]
public class ImpressionData
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;
}