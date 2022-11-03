using System.Collections;
using Firebase.Analytics;
using UnityEngine;
using System;
using System.Globalization;
/// <summary>
/// Document Link: https://docs.google.com/spreadsheets/d/1PUjPCuHoE5pRhD8Up4vCrQWRktgS_MFFADkgZppNdEw/edit#gid=0
/// </summary>
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager ins = null;

    //Check khởi tạo firebase
    private bool fireBaseReady = false;//Firebase đã Init thành công
    private bool firebaseIniting = false;//Firebase đang Init

    public bool is_remote_config_done = false;//Quá trình RemoteConfig đã xong
    public bool is_remote_config_success = false;//RemoteConfig thành công

    #region FIREBASE SETUP
    void Awake()
    {
        if (ins != null)
        {
            Destroy(this.gameObject);
            return;
        }
        ins = this;
    }


    public void Init() {
        StartCoroutine(ie_Init());
    }

    private IEnumerator ie_Init()
    {
        firebaseIniting = true;
        CheckFireBase();
        yield return new WaitUntil(() => !firebaseIniting);
        if (fireBaseReady) {
            //Khởi tạo remote config
            fetch((bool is_fetch_result) => { });
        } else {
            Debug.LogError("Ko khởi tạo đc Firebase");
        }
    }

    private void CheckFireBase()
    {
        try
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                firebaseIniting = false;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    fireBaseReady = true;
                }
                else
                {
                    Debug.LogError(System.String.Format("Lỗi dependencies của Firebase: {0}", dependencyStatus));
                }
            });
        }
        catch (System.Exception ex)
        {
            firebaseIniting = false;
            Debug.LogError("Lỗi CheckFireBase:" + ex.ToString());
        }
    }
    #endregion

    #region USER_PROPERTIES
    public void StartGame_UserProperty() {
        try {
            if (!Debug.isDebugBuild && !Application.isEditor) {
                //(Retention: Ngày cuối cùng User mở lại game. Ngày đầu tiên User chơi tính là Day 0)
                if (DataManager.ins.gameSave.totalDays <= 7) {//Đối vs HyperCasual, chỉ bắn UserProperties của Firebase đến Day 7
                    if (!DataManager.ins.gameSave.firebase_retent_type[DataManager.ins.gameSave.totalDays]) {
                        OnSetProperty("retent_type", DataManager.ins.gameSave.totalDays);
                        DataManager.ins.gameSave.firebase_retent_type[DataManager.ins.gameSave.totalDays] = true;
                    }
                    OnSetProperty("days_played", DataManager.ins.gameSave.daysPlayed);
                    OnSetProperty("level_played", DataManager.ins.gameSave.levelPass);//số level đã chơi(số level đã start -mỗi level chỉ bắn 1 lần)
                    OnSetProperty("total_currency", DataManager.ins.gameSave.gold);//tổng số currency đã nhận được
                    OnSetProperty("start_minigame", DataManager.ins.gameSave.levelStart);//số lần vào chơi minigame (Chơi lại vẫn bắn)
                }
                if (DataManager.ins.gameSave.totalSession == 0) {
                    //Bắn lên khi mở game ở session đầu tiên (Chỉ bắn 1 lần mỗi User) 
                    first_open();
                }
                if (DataManager.ins.gameSave.daysPlayed == 0) {
                    //số level người chơi đã chơi ở ngày đầu tiên
                    OnSetProperty("level_played_1stday", DataManager.ins.gameSave.levelPass);
                }
            }
        } catch (System.Exception ex) {
            Debug.LogError("Lỗi StartGame_UserProperty:" + ex.ToString());
        }
    }

    //Thông tin của User này sẽ biến động liên tục và đc gắn vào mỗi event khi bắn lên
    public void OnSetProperty(string key, object value)
    {
        try
        {
            FirebaseAnalytics.SetUserProperty(key.ToString(), value.ToString());
        }
        catch
        {
            Debug.LogError("Lỗi OnSetProperty: " + key);
        }
    }
    #endregion

    #region Events
   //Bắn lên khi mở game ở session đầu tiên (Chỉ bắn 1 lần mỗi User) 
    public void first_open() {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("first_open", new Parameter [] { });
        }
    }

    //Bắn lên khi người chơi start level x (chỉ bắn 1 lần mỗi level)
    public void check_point(int totalLevel, string name_game_cur, int rank, int map) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            if (totalLevel < 10) {
                FirebaseAnalytics.LogEvent("checkpoint_0" + totalLevel, new Parameter []
                    {
                        new Parameter("name_game_cur", name_game_cur),
                        new Parameter("rank", rank + ""),
                        new Parameter("map", map + ""),
                    });
            } else if (totalLevel <= 20) {
                FirebaseAnalytics.LogEvent("checkpoint_" + totalLevel, new Parameter []
                    {
                        new Parameter("name_game_cur", name_game_cur),
                        new Parameter("rank", rank + ""),
                        new Parameter("map", map + ""),
                    });
            }
        }
    }
    //Bắn lên khi người chơi end level x (chỉ bắn 1 lần mỗi level)
    public void checkpoint_endgame(int totalLevel, string name_game_cur, int rank, int map, bool isWin) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            if (totalLevel < 10) {
                FirebaseAnalytics.LogEvent("checkpoint_endgame_0" + totalLevel, new Parameter []
                    {
                         new Parameter("name_game_cur", name_game_cur),
                          new Parameter("rank", rank + ""),
                        new Parameter("map", map + ""),
                         new Parameter("is_win", isWin ? "win" : "lose")
                    });
            } else if (totalLevel <= 20) {
                FirebaseAnalytics.LogEvent("checkpoint_endgame_" + totalLevel, new Parameter []
                    {
                        new Parameter("name_game_cur", name_game_cur),
                         new Parameter("rank", rank + ""),
                        new Parameter("map", map + ""),
                         new Parameter("is_win", isWin ? "win" : "lose")
                    });
            }
        }
    }

    //bắn lên khi người chơi bắt đầu 1 level
    public void level_start(string minigame, int rank, int map, int totalLevel, int current_money) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("level_start", new Parameter []
{               
                new Parameter("name_game_cur", minigame),
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                new Parameter("current_money", current_money + ""),
});
            FirebaseAnalytics.LogEvent("level_start_" + minigame, new Parameter []
{
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                new Parameter("current_money", current_money + ""),
});
        }
    }

    //bắn lên khi người chơi hồi sinh bằng AdsReward trong 1 level
    public void level_revive(string minigame, int rank, int map, int totalLevel, int seconds_remaining, bool isRewardAds) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("level_complete", new Parameter []
{
                new Parameter("name_game_cur", minigame),
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                new Parameter("seconds_remaining", seconds_remaining + ""),
                new Parameter("reward_Ads", isRewardAds + ""),
});
            FirebaseAnalytics.LogEvent("level_revive_" + minigame, new Parameter []
{
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                 new Parameter("seconds_remaining", seconds_remaining + ""),
                  new Parameter("reward_Ads", isRewardAds + ""),
});
        }
    }

    //bắn lên khi người chơi hoàn thành 1 level
    public void level_complete(string minigame, int rank, int map, int totalLevel, int real_time_played) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("level_complete", new Parameter []
{
                new Parameter("name_game_cur", minigame),
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                new Parameter("real_time_played", real_time_played + ""),
});
            FirebaseAnalytics.LogEvent("level_complete_" + minigame, new Parameter []
{
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                new Parameter("real_time_played", real_time_played + ""),
});

        }
    }

    //bắn lên khi người chơi thua 1 level
    public void level_fail(string minigame, int rank, int map, int totalLevel, int failcount) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("level_fail", new Parameter []
{
                new Parameter("name_game_cur", minigame),
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                new Parameter("failcount", failcount + ""),
});
            FirebaseAnalytics.LogEvent("level_fail_" + minigame, new Parameter []
{
                new Parameter("rank", rank + ""),
                new Parameter("map", map + ""),
                new Parameter("level", totalLevel + ""),
                new Parameter("failcount", failcount + ""),
});
        }
    }

    //Loại tiền tệ kiếm được
    //[virtual_currency_name]: tên loại tiền tệ (gold, gem ...)
    //[amount]: số lượng
    //[source]: nguồn kiếm được (collect in game, view reward, buy with $ ...)
    public void earn_virtual_currency(string virtual_currency_name, int amount, string source)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("earn_virtual_currency", new Parameter[]
            {
                new Parameter("virtual_currency_name", virtual_currency_name),
                new Parameter("value", amount),
                new Parameter("source", source)
               });
        }
    }

    //Loại tiền tệ tiêu thụ
    //[virtual_currency_name]: tên loại tiền tệ (gold, gem ...)
    //[amount]: số lượng
    //[source]: nguồn tiêu thụ (mua skin, hồi sinh, ...)
    public void spend_virtual_currency(string virtual_currency_name, int amount, string item_name)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("spend_virtual_currency", new Parameter[]
            {
                new Parameter("virtual_currency_name", virtual_currency_name),
                new Parameter("value", amount),
                new Parameter("item_name", item_name)
               });
        }
    }

    #region Inter ADS
    /// <summary>
    /// Khi click vào trigger inter (Đã đạt đủ capping và logic trong game.)
    /// Nhưng ko check đã load đc ads chưa.
    /// </summary>
    public void ads_inter_click() {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_inter_click", new Parameter [] { });
        }
    }

    /// <summary>
    /// bắn lên khi quảng cáo inter được load
    /// </summary>
    public void ads_inter_load()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_load", new Parameter[] { });
        }
    }

    /// <summary>
    /// bắn lên khi quảng cáo inter bắt đầu đc gọi
    /// </summary>
    public void ads_inter_start_show()
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_start_show", new Parameter[] { });
        }
    }

    /// <summary>
    ///bắn lên khi quảng cáo inter đã đc hiển thị
    /// </summary>
    public void ads_inter_displayed() {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_inter_displayed", new Parameter [] { });
        }
    }

    /// <summary>
    /// bắn lên khi có lỗi inter
    /// </summary>
    public void ads_inter_fail(string errormsg)
    {
        if (!Debug.isDebugBuild && !Application.isEditor)
        {
            FirebaseAnalytics.LogEvent("ads_inter_fail", new Parameter[]
            {
                new Parameter("errormsg", errormsg)
            });
        }
    }
    #endregion

    #region Reward ADS
    /// <summary>
    /// bắn lên khi hiển thị offer reward cho người chơi
    /// </summary>
    public void ads_reward_offer(string placement, string reward) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_reward_offer", new Parameter []
            {
                new Parameter("placement", placement),
                new Parameter("reward", reward)
            });
        }
    }

    /// <summary>
    ///bắn lên khi người chơi click vào button quảng cáo
    /// </summary>
    public void ads_reward_click(string placement, string reward) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_reward_click", new Parameter []
            {
                new Parameter("placement", placement),
                new Parameter("reward", reward)
            });
        }
    }

    /// <summary>
    /// bắn lên khi quảng cáo bắt đầu đc gọi
    /// </summary>
    public void ads_reward_start_show(string placement, string reward) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_reward_start_show", new Parameter []
            {
                new Parameter("placement", placement),
                new Parameter("reward", reward),
            });
        }
    }

    /// <summary>
    /// bắn lên khi quảng cáo đã đc hiển thị
    /// </summary>
    public void ads_reward_displayed(string placement, string reward) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_reward_displayed", new Parameter []
            {
                new Parameter("placement", placement),
                new Parameter("reward", reward),
            });
        }
    }

    /// <summary>
    /// bắn lên khi có lỗi hiển thị quảng cáo
    /// </summary>
    public void ads_reward_fail(string placement, string reward, string errormsg) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_reward_fail", new Parameter []
            {
                new Parameter("placement", placement),
                new Parameter("reward", reward),
                new Parameter("errormsg", errormsg)
            });
        }
    }

    /// <summary>
    ///bắn lên khi người chơi hoàn thành xem ads reward
    /// </summary>
    public void ads_reward_complete(string placement, string reward) {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            FirebaseAnalytics.LogEvent("ads_reward_complete", new Parameter []
            {
                new Parameter("placement", placement),
                new Parameter("reward", reward),
            });
        }
    }
    #endregion

    #region ADS RevenuePain
    /// <summary>
    /// Send theo event của Max Manager (Log doanh thu từ mỗi quảng cáo)
    /// </summary>
    /// <param name="data"></param>
    public void ADS_RevenuePain(ImpressionData data)
    {
        try {
            if (!Debug.isDebugBuild && !Application.isEditor) {
                Parameter [] AdParameters = {
                 new Parameter("ad_platform", "applovin"),
                 new Parameter("ad_source", data.NetworkName),
                 new Parameter("ad_unit_name", data.AdUnitIdentifier),
                 new Parameter("currency", "USD"),
                 new Parameter("value", data.Revenue),
                 new Parameter("placement", data.Placement),
                 new Parameter("country_code", data.CountryCode),
                 new Parameter("ad_format", data.AdFormat),
                };
                FirebaseAnalytics.LogEvent("ad_impression", AdParameters);
            }
        } catch (Exception e) {
            Debug.LogError("Lỗi ADS_RevenuePain:" + e.ToString());
        }
        try {
            if (!Debug.isDebugBuild && !Application.isEditor) {
                Parameter [] AdParameters = {
                 new Parameter("ad_platform", "applovin"),
                 new Parameter("ad_source", data.NetworkName),
                 new Parameter("ad_unit_name", data.AdUnitIdentifier),
                 new Parameter("currency", "USD"),
                 new Parameter("value", data.Revenue),
                 new Parameter("placement", data.Placement),
                 new Parameter("country_code", data.CountryCode),
                 new Parameter("ad_format", data.AdFormat),
                };
                FirebaseAnalytics.LogEvent("ad_impression_abi", AdParameters);
            }
        } catch (Exception e) {
            Debug.LogError("Lỗi ad_impression_abi:" + e.ToString());
        }
    }
    #endregion

    #endregion

    #region Remote Config
    /// <summary>
    /// Setup Remote config
    /// </summary>
    /// <param name="completionHandler"></param>
    public void fetch(Action<bool> completionHandler)
    {
        try
        {
            Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
            var settings = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ConfigSettings;
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(settings);

            var fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(new TimeSpan(0));

            fetchTask.ContinueWith(task =>
            {
                is_remote_config_done = true;
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogWarning("fetchTask Firebase Fail");
                    is_remote_config_success = false;
                    completionHandler(false);
                }
                else
                {
                    Debug.LogWarning("fetchTask Firebase Commplete");
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    RefrectProperties();

                    completionHandler(true);
                }
            });
        }
        catch (Exception ex)
        {
            is_remote_config_done = true;
            Debug.Log("Lỗi fetch:" + ex.ToString());
        }
    }

    /// <summary>
    /// Dữ liệu remote config
    /// </summary>
    private void RefrectProperties()
    {
        try
        {
            //AOA Setting Params
            //GetValue_RemoteConfig("AOA_FirstOpen", out PluginManager.ins.AOA_FirstOpen);
            //GetValue_RemoteConfig("AOA_SessionStart", out PluginManager.ins.AOA_SessionStart);
            //GetValue_RemoteConfig("AOA_SwitchApps", out PluginManager.ins.AOA_SwitchApps);
            //Bật Inter theo điều kiện
            //GetValue_RemoteConfig("ShowInter_FormLV1", out PluginManager.ins.ShowInter_FormLV1);
            //Đặt là true thì mới có thể vào minigame Mommy va Huggy. Đặt là false thì ko thể vào đc để GG ko bắt bản quyền)
            //GetValue_RemoteConfig("OpenMinigameMommyHuggy", out PluginManager.ins.OpenMinigameMommyHuggy);
            //GetValue_RemoteConfig("isShowBooster", out PluginManager.ins.isShowBooster);
            //GetValue_RemoteConfig("FreeChar_Loading", out PluginManager.ins.FreeChar_Loading);
            //GetValue_RemoteConfig("TryFree_FormHome", out PluginManager.ins.TryFree_FormHome);
            //GetValue_RemoteConfig("PopupFail_ButtonHomeOnTop", out PluginManager.ins.PopupFail_ButtonHomeOnTop);
            
            //Remote Config kiểu hiện Inter
            //GetValue_RemoteConfig("ShowInter_ByWin", out PluginManager.ins.ShowInter_ByWin);
            //GetValue_RemoteConfig("CostReviveStart", out PluginManager.ins.CostReviveStart);
            //GetValue_RemoteConfig("CostReviveIncrease", out PluginManager.ins.CostReviveIncrease);
            //
            //GetValue_RemoteConfig("Setting_BtnHome", out PluginManager.ins.Setting_BtnHome);
            //GetValue_RemoteConfig("timeOffer", out PluginManager.ins.timeOffer);
            //GetValue_RemoteConfig("Difficulty_Rank1", out PluginManager.ins.Difficulty_Rank1);
            //GetValue_RemoteConfig("NodeStart_WinAll", out PluginManager.ins.NodeStart_WinAll);
            //GetValue_RemoteConfig("NodeIncease_WinAll", out PluginManager.ins.NodeIncease_WinAll);
            //GetValue_RemoteConfig("GoldOffer_Shop", out PluginManager.ins.GoldOffer_Shop);
            //Remote Config độ khó bắt đầu
            //GetValue_RemoteConfig("DifficultyStart", out PluginManager.ins.DifficultyStart);
            //Capping Inter
            //GetValue_RemoteConfig("Capping_Inter", out PluginManager.ins.Capping_Inter);
            //Level bắt user phải bật Internet
            //GetValue_RemoteConfig("Level_PopupTurnOnInternet", out PluginManager.ins.Level_PopupTurnOnInternet);

            //GetValue_RemoteConfig("AOA_SwitchApps_Seconds", out PluginManager.ins.AOA_SwitchApps_Seconds);

            is_remote_config_success = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi RefrectProperties: " + ex.Message);
        }
    }

    private void GetValue_RemoteConfig(string key, out bool value) {
        value =  Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
        Debug.LogWarning("RemoteConfig: " + key +" = " + value);
    }

    private void GetValue_RemoteConfig(string key, out long value) {
        value =  Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
        Debug.LogWarning("RemoteConfig: " + key +" = " + value);
    }

    #endregion
}
