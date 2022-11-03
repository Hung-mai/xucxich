using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    [Header("--------------- Cheat & Hack --------")]
    public Minigame levelCheat = Minigame.None;
    public int rankMinigame = -1;
    public int mapMinigame = -1;
    //public bool isABTest = false;
    public bool isPlayMinigameNow = false;
    public bool isRemoveAllAds = false;
    public bool isEnableCheat = false;//Bật cheat trong Setting: No Banner, No Inter + PopupSetting: Hide UIGameplay, cheat Gold, cheat Levels 
    
    [Header("--------------- Trạng thái đặc biệt --------")]
    public bool isIOS = false;
    public bool isFormLoading_Proccess = false;
    public bool isSelectedChar = false;
    public bool isFirstOpen = true;
    public bool isHideUIGameplay = false;
    public bool isShowOfflineEarning = false;
    public Minigame_Manager minigameManager;

    [Header("--------------- Prefab của Popup --------")]
    public Popup_Rate popup_Rate;
    public Popup_EndGame popup_EndGame;
    public Popup_Settings popup_Settings;
    public Popup_TurnOnInternet popup_TurnOnInternet;
    public Popup_Revive popup_Revive;
    public Popup_SelectMiniGame popup_SelectMiniGame;
    public Popup_Skin popup_Skin;
    public Popup_Earning popup_Earning;
    public Popup_OfferFreeSkin popup_OfferFreeSkin;
    public Popup_NoAds popup_NoAds;
    public Popup_Spin popup_Spin;


    [Header("-------- List Data --------")]
    public List<Minigame> listMinigamesRandom;
    public Data_Minigame [] arrayDatasMinigame;
    public List<Data_Minigame> listDatasMinigame_Available;
    public string [] listUsernames;
    public Color [] listColorsChar;
    public Data_Skin [] arrayDataSkin_Full;
    public Data_Skin [] arrayDataSkin_Hair;
    public Data_Skin [] arrayDataSkin_Hand;
    public Data_Skin [] arrayDataSkin_Body;

    [Header("--------------- Other --------")]
    public GameObject obj_SplashScreen;

    [Header("------Time---------")]
    public int timeOffer = 999;
   

    #region Unity
    public override void Awake() {
        base.Awake();
        isIOS = (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Constants.FRAME_RATE;
        obj_SplashScreen.SetActive(true);
        //Đếm thời gian chơi
        StartCoroutine(CountTime());
    }

    IEnumerator CountTime()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);
            timeOffer++;
        }
    }


    IEnumerator Start() {
        if (isRemoveAllAds == false) {
            //UI Splash screen
            DataManager.ins.LoadData();
            yield return new WaitForSeconds(0.1f);
            PluginManager.ins.InitAll();
            yield return new WaitForSeconds(2.5f);

            //UI Loading
            obj_SplashScreen.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            isFormLoading_Proccess = true;
            SoundManager.ins.ReloadMusic();
            PluginManager.ins.FirstOpen();
            yield return new WaitUntil(() => PluginManager.ins.isFirstOpen_Done);
            yield return new WaitUntil(() => SceneManager.ins.form_Loading.fillAmountProcess > 1.25f);
            SceneManager.ins.formCurrent.Show();
        } else {
            //UI Splash screen
            DataManager.ins.LoadData();
            obj_SplashScreen.SetActive(false);
            SceneManager.ins.formCurrent.Show();
            isFormLoading_Proccess = true;
            SoundManager.ins.ReloadMusic();
        }
        yield return new WaitUntil(() => isSelectedChar);
        DataManager.ins.SaveGame();
        if (DataManager.ins.gameSave.totalSession == 0) {
            /*
            if (DataManager.ins != null && DataManager.ins.gameSave != null) {
                    DataManager.ins.gameSave.difficulty = PluginManager.ins.DifficultyStart;
            } else {
                Debug.LogError("Lỗi ko thay đổi đc DifficultyStart: Do Data chưa đc load lên");
            }*/

            if (isPlayMinigameNow) {
                DataManager.ins.gameSave.isRandomMinigameNext = false;
                SceneManager.ins.ChangeForm_Gameplay();
            } else {
                SceneManager.ins.ChangeForm_Gameplay(0);
            }
        } else {
            if (isPlayMinigameNow) {
                DataManager.ins.gameSave.isRandomMinigameNext = false;
                SceneManager.ins.ChangeForm_Gameplay();
            }
            SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
        }
    }
    /*
    void OnDestroy() {
        if (!Debug.isDebugBuild && !Application.isEditor) {
            //số level đã chơi(số level đã start -mỗi level chỉ bắn 1 lần)
            FirebaseManager.ins.OnSetProperty("level_played", DataManager.ins.gameSave.levelWin);
        }
    }*/
    #endregion

}
