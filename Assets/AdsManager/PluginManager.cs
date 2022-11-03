using System;
using System.Collections;
using UnityEngine;

public class PluginManager : MonoBehaviour
{
    [Header("-------- CDPR_CCPA --------")]
    public bool isCDPR_CCPA;

    [Header("-------- Value Remote Config --------")]
    public bool AOA_FirstOpen = false;
    public bool AOA_SessionStart = true;
    public bool AOA_SwitchApps = true;
    public long AOA_SwitchApps_Seconds = 15;
    public long Capping_Inter = 20;
    public long Level_PopupTurnOnInternet = 2;
    //public bool ShowInter_FormLV1 = false;//Đã AB test xong
    public bool OpenMinigameMommyHuggy = true;
    //public long DifficultyStart = 1;//Đã AB test xong
    public long CostReviveStart = 100;
    public long CostReviveIncrease = 100;
    public bool isShowBooster = true;
    public long timeOffer = 180;
    public long Setting_BtnHome = 0;
    public bool FreeChar_Loading = false;
    public bool TryFree_FormHome = false;
    public long NodeStart_WinAll = 6;
    public long NodeIncease_WinAll = 0;
    public long GoldOffer_Shop = 500;
    public bool PopupFail_ButtonHomeOnTop = false;
    public long Difficulty_Rank1 = 1;


    [Header("--------------- State --------")]
    bool isNoWait_RemoteConfig = false;//Đợi RemoveConfig lâu quá thì vào game luôn (Giá trị RemoveConfig trả về muộn thì vẫn sẽ nhận đc)
    bool isSkipAOA = false;//Đợi AOA lâu quá thì skip
    [HideInInspector] public bool isFirstOpen_Done = false;

    #region awake
    public static PluginManager ins;
    private void Awake()
    {
        if (ins != null)
        {
            Destroy(this.gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(this.gameObject);

#if UNITY_IOS || UNITY_IPHONE
        isIOS = true;
#endif
    }
    #endregion

    public void InitAll() {
        StartCoroutine(IEnumerator_InitAll());
    }
    public void FirstOpen() {
        StartCoroutine(IEnumerator_FirstOpen());
    }

    public IEnumerator IEnumerator_InitAll() {
        FirebaseManager.ins.Init();
        yield return new WaitForEndOfFrame();
        AppOpenAdsManager.ins.Init();
        yield return new WaitForEndOfFrame();
        if (isCDPR_CCPA) CDPR_CCPA.ins.Init();
        yield return new WaitForEndOfFrame();
        MaxManager.ins.Init();
    }

    public IEnumerator IEnumerator_FirstOpen() {
        //MaxManager.ins.ReloadBanner();
        Timer.Schedule(this, 5, () => { isNoWait_RemoteConfig = true; });
        if (isCDPR_CCPA) {
            CDPR_CCPA.ins.ShowPopup();
            yield return new WaitUntil(() => CDPR_CCPA.ins.isComplete);
        }
        yield return new WaitUntil(() => FirebaseManager.ins.is_remote_config_success || isNoWait_RemoteConfig);
        if ((AOA_FirstOpen && DataManager.ins.gameSave.totalSession == 0) || (AOA_SessionStart && DataManager.ins.gameSave.totalSession > 0)) {
            Debug.LogWarning("AOA_RemoteConfig TurnOn");
            Timer.Schedule(this, 3, () => { isSkipAOA = true; });
            yield return new WaitUntil(() => (AppOpenAdsManager.ins.IsAdAvailable || isSkipAOA));
            if (AppOpenAdsManager.ins.IsAdAvailable) {
                Debug.LogWarning("AOA_Goi ham show AOA");
                AppOpenAdsManager.ins.ShowAOA();
            }
        }
        FirebaseManager.ins.StartGame_UserProperty();
        isFirstOpen_Done = true;
    }
}
