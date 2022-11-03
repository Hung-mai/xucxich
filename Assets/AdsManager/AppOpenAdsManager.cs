using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public class AppOpenAdsManager : MonoBehaviour {
    public static AppOpenAdsManager ins;

    /// <summary>
    /// Key test: AD_UNIT_ID = "ca-app-pub-3940256099942544/3419835294";
    /// </summary>

    public string AD_UNIT_ID = "";

    public string AD_UNIT_ID_IOS = "";

    private AppOpenAd ad;
    public bool isStartWaittingAd = false;
    private bool isShowingAd = false;
    private DateTime switchAppsTime;
    private DateTime timeLastAOAShow;

    //Kiểm tra việc loại bỏ show AOA
    //Nếu quá thời gian hoặc đã vào game
    private bool dropAOA;
    public bool IsAdAvailable {
        get {
            return ad != null;
        }
    }

    private void Awake() {
        if (ins != null) {
            Destroy(gameObject);
            return;
        } else {
            ins = this;
            DontDestroyOnLoad(this.gameObject);
        }
        switchAppsTime = new DateTime();
        Debug.LogWarning("AOA_Awake()");
    }

    public void Init() {
        try {
            MobileAds.Initialize(initStatus => {
                Debug.LogWarning("AOA_Initialize()");
                LoadAd();
            });
        } catch (Exception ex) {
            Debug.LogError("Lỗi Init AppOpenAdsManager:" + ex);
        }
    }

    private Coroutine i_ShowAOA;
    public void ShowAOA() {
        Debug.LogWarning("AOA_ShowAOA()");
        if (GameManager.ins.isRemoveAllAds) return;
        if (i_ShowAOA != null) StopCoroutine(i_ShowAOA);
        i_ShowAOA = StartCoroutine(ie_ShowAOA());
    }

    public void DropShowAOA() {
        if (i_ShowAOA != null) StopCoroutine(i_ShowAOA);
        dropAOA = true;
    }

    IEnumerator ie_ShowAOA() {
        Debug.LogWarning("AOA_ie_ShowAOA()");
        LoadAd();
        dropAOA = false;
        yield return new WaitUntil(() => IsAdAvailable);
        Debug.LogWarning("AOA_dropAOA" + dropAOA +"Chuẩn bị gọi ShowAdIfAvailable()");
        if (!dropAOA) ShowAdIfAvailable();
    }



    private bool _isLoading;
    private void LoadAd() {
        try {
            //đã có ads rồi hoặc đang load rồi thì thôi load
            if (IsAdAvailable || _isLoading) return;
            _isLoading = true;
            Debug.LogWarning("AOA_LoadAd()");

            AdRequest request = new AdRequest.Builder().Build();
            // Load an app open ad for portrait orientation
            AppOpenAd.LoadAd(GameManager.ins.isIOS ? AD_UNIT_ID_IOS : AD_UNIT_ID, ScreenOrientation.Portrait, request, ((appOpenAd, error) => {
                _isLoading = false;
                if (error != null) {
                    // Handle the error.
                    Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                    Invoke(nameof(LoadAd), 1f);
                    return;
                }
                // App open ad is loaded.
                ad = appOpenAd;
                Debug.LogWarning("AOA_ Load AOA thanh cong");
            }));
        } catch (System.Exception ex) {
            _isLoading = false;
            Debug.LogError("Lỗi LoadAOA:" + ex.ToString());
        }
    }

    private void ShowAdIfAvailable() {
        try {

            Debug.LogWarning("AOA_ ShowAdIfAvailable()");

            Debug.LogWarning("AOA_Thoi gian_" + !IsAdAvailable
                + "_" + GameManager.ins.isRemoveAllAds
                + "_" + isShowingAd
                + "_" + MaxManager.ins.showingAds);

            if (!IsAdAvailable
                || GameManager.ins.isRemoveAllAds
                || isShowingAd
                || MaxManager.ins.showingAds) {
                Debug.LogWarning("AOA_RemoveAll Ads hoặc đang show ads khác");
                return;
            }

            //Ko show AOA quá gần nhau
            if ((DateTime.Now - switchAppsTime).TotalSeconds < 30) {
                Debug.LogWarning("AOA_Loi thoi gian capping");
                return;
            }
            ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
            ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
            ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
            ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
            ad.OnPaidEvent += HandlePaidEvent;

            Debug.LogWarning("AOA_ ShowAdIfAvailable() thanh cong");
            if (Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.OSXEditor) {
                Debug.LogWarning("AOA_Ko show trong Editor");
                return;
            }

            isStartWaittingAd = true;
            switchAppsTime = DateTime.Now;
            timeLastAOAShow = DateTime.Now;
            ad.Show();
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi ShowAdIfAvailable:" + ex.ToString());
        }
    }

    private void OnApplicationPause(bool paused) {
        try {
            // Hiển thị AOA nếu back ra ngoài rồi vào lại > time capping
            if (paused) {
                //Lưu lại thời điểm User back ra ngoài
                Debug.LogWarning("AOA_Back ra ngoài");
                switchAppsTime = DateTime.Now;
            }
            if (!paused && PluginManager.ins.isFirstOpen_Done) {
                //Khi vào lại game
                double timeSwitchApps = (DateTime.Now - switchAppsTime).TotalSeconds;
                Debug.LogWarning("AOA_Khi vào lại game_"+!MaxManager.ins.showingAds+"_"+PluginManager.ins.AOA_SwitchApps +"_"+timeSwitchApps);
                if (!MaxManager.ins.showingAds
                    && PluginManager.ins.AOA_SwitchApps
                    && timeSwitchApps >= PluginManager.ins.AOA_SwitchApps_Seconds) {
                    Debug.LogWarning("AOA_Đủ điều kiện show SwitchApps");
                    ShowAOA();
                } else if (MaxManager.ins != null) {
                    MaxManager.ins.showingAds = false;//Vừa xem Ads xong
                    MaxManager.ins.isStartWaitting_Inter = false;
                    MaxManager.ins.isStartWaitting_Reward = false;
                    isStartWaittingAd  = false;
                    Debug.LogWarning("AOA_Vừa xem Ads xong");
                }
            }
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi OnApplicationPause_AOA:" + ex.ToString());
        }
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args) {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdsManager no longer has another ad to show.
        isStartWaittingAd = false;
        isShowingAd = false;
        ad = null;
        LoadAd();
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args) {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdsManager no longer has another ad to show.
        isStartWaittingAd = false;
        isShowingAd = false;
        ad = null;
        LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args) {
        isStartWaittingAd = false;
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args) {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args) {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.AdValue.CurrencyCode, args.AdValue.Value);
    }
}