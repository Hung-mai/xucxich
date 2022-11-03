using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using DG.Tweening;

public class SceneManager : MonoBehaviour {
    public static SceneManager ins;
    public static FormUI formLast;
    public AsyncOperation async;
   
    [Header("--------------- Trạng thái của Scene ---------------")]
    bool isNoWait_RemoteConfig = false;//Đợi RemoveConfig lâu quá thì vào game luôn (Giá trị RemoveConfig trả về muộn thì vẫn sẽ nhận đc)
    [SerializeField] public bool isChangingForm = false; 

    [Header("--------------- Pause ---------------")]
    public bool isPause = false;
    [SerializeField] private bool isPauseF5 = false;
    [SerializeField] public bool isPauseSound = false;
    [SerializeField] private float timeBlock = 0;//Block Input theo thời gian
    [SerializeField] private bool isBlockInput;//Block Input theo sự kiện
    public GameObject obj_BlockInput;
    public GameObject obj_WaittingAds;
    public GameObject obj_RewardAdsNotAvailable;

    [Header("--------------- Popup đã đc tạo trong Scene ---------------")]
    [HideInInspector] public Popup_Rate popup_Rate;
    [HideInInspector] public Popup_EndGame popup_EndGame;
    [HideInInspector] public Popup_Settings popup_Settings;
    [HideInInspector] public Popup_TurnOnInternet popup_TurnOnInternet;
    [HideInInspector] public Popup_Revive popup_Revive;
    [HideInInspector] public Popup_SelectMiniGame popup_SelectMiniGame;
    [HideInInspector] public Popup_Skin popup_Skin;
    [HideInInspector] public Popup_Earning popup_Earning;
    [HideInInspector] public Popup_OfferFreeSkin popup_OfferFreeSkin;
    [HideInInspector] public Popup_NoAds popup_NoAds;
    [HideInInspector] public Popup_Spin popup_Spin;
    [HideInInspector] public List<PopupBase> popupList;

    [Header("--------------- UI trong Scene ---------------")]
    public Form_Loading form_Loading;
    public Form_Gameplay formGameplay;
    public Form_Home formHome;
    public Canvas formCanvas;
    public Canvas popupCanvas;
    public Canvas loadingCanvas;
    public Canvas canvas_Indicator;
    public Canvas canvas_Joystick;
    public FormBase formCurrent;

    public GameObject obj_Gold;
    public TextMeshProUGUI txt_UIMoney;
    public Coroutine coroutineTweenGem;
    [SerializeField] Action call_back_tween_gem;
    //public List<Data_Intelligent> intelligentList;
    public Image iconGold;
    public TextMeshProUGUI obj_CupTxt;
    public GameObject obj_Cup;
    public EffectGem effectGem;
    #region Main
    private void Awake() {
        if (ins != null) Destroy(ins.gameObject);
        ins = this;
    }

    protected void Start() {
        if (GameManager.ins == null) return;
        isChangingForm = false;
        if (GameManager.ins.isFormLoading_Proccess) {
            formCurrent.Show(); 
        }
        if (txt_UIMoney != null) txt_UIMoney.text = DataManager.ins.gameSave.gold + "";
        if (obj_CupTxt != null) obj_CupTxt.text = DataManager.ins.gameSave.amountWinAll.ToString();
    }

    void Update() {
        if (async != null) return;
        //Bật phần xoay xoay khi đợi Ads 
        if(AppOpenAdsManager.ins == null || MaxManager.ins == null) return;
        obj_WaittingAds.SetActive(AppOpenAdsManager.ins.isStartWaittingAd || MaxManager.ins.isStartWaitting_Inter || MaxManager.ins.isStartWaitting_Reward);
        //Rung nhẹ mỗi khi nhấn vào màn hình
        //if (Input.GetMouseButtonDown(0) && DataManager.ins != null && DataManager.ins.gameSave.isVibrate) MoreMountains.NiceVibrations.MMVibrationManager.Vibrate();
        //Pause game
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5)) isPauseF5 = !isPauseF5;
#endif
        if (GameManager.ins != null) { 
            Time.timeScale = (isPause || isPauseF5) ? 0 : 1;
            AudioListener.pause = (isPauseSound);
        }
        //User bấm nút Back trên điện thoại
        /*if (Input.GetKeyDown(KeyCode.Escape) && !obj_BlockInput.activeSelf)
        {
            //Đóng các Popup lại
            PopupBase[] listPopup =  popupCanvas.GetComponentsInChildren<PopupBase>();
            if (listPopup.Length > 0)
            {
                //Nếu popup cuối cùng (Ở trên cùng) có thể tắt đc -> Tắt popup
                if (listPopup[listPopup.Length - 1].isCloseByEscape)
                {
                    listPopup[listPopup.Length - 1].Close();
                }
            }
        }*/
    }
    #endregion


    #region Block Input
    public void BlockInput(bool isBlock) {//BlockInput theo sự kiện
        isBlockInput = isBlock;
        if ((timeBlock <= 0 && !isBlock) || isBlock) obj_BlockInput.SetActive(isBlock);
    }

    public void BlockInput(float second) {//BlockInput theo thời gian
        obj_BlockInput.SetActive(true);
        if (second > timeBlock) timeBlock = second;
        StartCoroutine(CountTimeBlockInput());
    }

    private IEnumerator CountTimeBlockInput() {
        yield return new WaitForSeconds(0.5f);
        timeBlock -= 0.5f;
        if (timeBlock <= 0) {
            timeBlock = 0;
            if (!isBlockInput) obj_BlockInput.SetActive(false);//Nếu ko block theo even thì mới mở Block
        } else {
            StartCoroutine(CountTimeBlockInput());//Tiếp tục đếm thời gian Block Input
        }
    }
    #endregion

    #region Tween Money
    private IEnumerator TweenGem(int gem, Action _call_back_tween_gem = null) {
        if (_call_back_tween_gem != null) {
            this.call_back_tween_gem = _call_back_tween_gem;
        }
        if (txt_UIMoney != null) {
            if (gem > 0) {
                int tweenGem = int.Parse(txt_UIMoney.text);
                int amount = (gem <= 50 ? 2 : gem < 100 ? 4 : gem < 200 ? 6 : gem < 300 ? 8 : gem < 400 ? 8 : gem < 500 ? 10 : gem < 800 ? 20 : gem < 1600 ? 40 : gem / 30);
                while (DataManager.ins.gameSave.gold > tweenGem) {
                    tweenGem += amount;
                    txt_UIMoney.text = tweenGem.ToString();
                    txt_UIMoney.color = new Color(0.5f, 1, 0.5f);

                    yield return new WaitForSeconds(0.02f);
                }
                txt_UIMoney.text = DataManager.ins.gameSave.gold.ToString();
            } else {
                int tweenGem = int.Parse(txt_UIMoney.text);
                int amount = (gem > -30 ? -2 : gem > -100 ? -5 : gem > -200 ? -10 : gem > -800 ? -20 : gem > -1600 ? -40 : gem / 20);
                if (DataManager.ins.gameSave.gold < tweenGem) {
                    while (DataManager.ins.gameSave.gold < tweenGem) {
                        tweenGem += amount;
                        txt_UIMoney.text = tweenGem.ToString();
                        txt_UIMoney.color = new Color(1,0.5f, 0.5f);

                        yield return new WaitForSeconds(0.02f);
                    }
                } else {
                    while (DataManager.ins.gameSave.gold > tweenGem) {
                        tweenGem -= amount;
                        txt_UIMoney.text = tweenGem.ToString();
                        txt_UIMoney.color =  new Color(0.5f, 1, 0.5f);

                        yield return new WaitForSeconds(0.02f);
                    }
                }

                txt_UIMoney.text = DataManager.ins.gameSave.gold.ToString();
            }
            txt_UIMoney.color = Color.white;

        }

        yield return new WaitForSeconds(0.3f);


        if (this.call_back_tween_gem != null) {
            this.call_back_tween_gem();
            this.call_back_tween_gem = null;
        }
        coroutineTweenGem = null;
    }


    public void OpenTweenGem(TextMeshProUGUI txt_gem,int currentGold = 0,Action _call_back_tween_gem = null) {
        txt_UIMoney = txt_gem;
        if (coroutineTweenGem != null) {
            StopCoroutine(coroutineTweenGem);
            txt_UIMoney.text = currentGold.ToString();
        }

        coroutineTweenGem = StartCoroutine(TweenGem(DataManager.ins.gameSave.gold - int.Parse(txt_UIMoney.text), _call_back_tween_gem));
    }

    #endregion

    #region Form
    //Chuyển sang 1 Form mới
    public void ChangeForm(string formNext, float time = 0) {
        if (isChangingForm) return;
        isChangingForm = true;
        formLast = formCurrent.idForm;
        AppOpenAdsManager.ins.DropShowAOA();
        BlockInput(time + 0.5f);
        Timer.Schedule(this, time, () => { 
            async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(formNext); 
        });
    }

    public void ChangeForm_Gameplay(float time = 0) {
        if (isChangingForm) return;
        isChangingForm = true;
        formLast = formCurrent.idForm;

        BlockInput(time + 0.5f);
        Minigame minigameNext = GameManager.ins.levelCheat;
        if (GameManager.ins.levelCheat != Minigame.None) {
            Timer.Schedule(this, time, () => {
                async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)minigameNext + "_" + minigameNext.ToString());
            });
        } else if(DataManager.ins.gameSave.isRandomMinigameNext || DataManager.ins.gameSave.idRandomMinigame == -1) {
            Timer.Schedule(this, time, () => {
                DataManager.ins.gameSave.isRandomMinigameNext = false;
                 minigameNext = NextMinigame();
                if (PluginManager.ins.OpenMinigameMommyHuggy == false) {
                    if (minigameNext == Minigame.GhostChaser || minigameNext == Minigame.OnTheCuttingBoard) {
                        minigameNext = NextMinigame();
                    }
                    if (minigameNext == Minigame.GhostChaser || minigameNext == Minigame.OnTheCuttingBoard) {
                        minigameNext = NextMinigame();
                    }
                }
                int rankCur = GameManager.ins.rankMinigame >= 0 ? GameManager.ins.rankMinigame : Mathf.CeilToInt(DataManager.ins.gameSave.difficulty - 0.01f);
                rankCur = Mathf.Clamp(rankCur, 0, Constants.MAX_RANK_EACH_MINIGAME - 1);
                int mapCur = GameManager.ins.mapMinigame >= 0 ? GameManager.ins.mapMinigame : DataManager.ins.gameSave.list_Minigames[(int)minigameNext].idMapPlayed_ByRank[rankCur];
                mapCur = Mathf.Clamp(mapCur, 0, Mathf.Max(0, GameManager.ins.arrayDatasMinigame[(int)minigameNext].rankMinigames[rankCur].prefab_Maps.Length - 1));
                FirebaseManager.ins.check_point(DataManager.ins.gameSave.levelEnded + 1, minigameNext.ToString(), rankCur, mapCur);
                ShowPopup_SelectMiniGame();
                popup_SelectMiniGame.Active(GameManager.ins.arrayDatasMinigame [(int)minigameNext], () => {
                    async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)minigameNext + "_" + minigameNext.ToString());
                });
            });
        } else {
            minigameNext =GameManager.ins.listMinigamesRandom [DataManager.ins.gameSave.idRandomMinigame];
            Timer.Schedule(this, time, () => {
                async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)minigameNext + "_" + minigameNext.ToString());
            });
        }
    }

    private Minigame NextMinigame() {
        DataManager.ins.gameSave.idRandomMinigame++;
        if (DataManager.ins.gameSave.idRandomMinigame >= GameManager.ins.listMinigamesRandom.Count)
            DataManager.ins.gameSave.idRandomMinigame = 0;
        return GameManager.ins.listMinigamesRandom [DataManager.ins.gameSave.idRandomMinigame];
    }
    
    #endregion

    #region Popup

    public void ShowPopup_Rate() {
        if (popup_Rate == null) {
            popup_Rate = Instantiate(GameManager.ins.popup_Rate, popupCanvas.transform);
        }
        popup_Rate.transform.localScale = Vector3.one;
        popup_Rate.transform.SetAsLastSibling();
        popup_Rate.gameObject.SetActive(false);
        popup_Rate.Show();
    }

    public void ShowPopup_EndGame(bool isWin = false, float timeDelay = 0) {
        if (popup_EndGame == null) {
            popup_EndGame = Instantiate(GameManager.ins.popup_EndGame, popupCanvas.transform);
        } else if (popup_EndGame.gameObject.activeSelf) {
            return;
        }
        popup_EndGame.isWin = isWin;
        popup_EndGame.transform.localScale = Vector3.one;
        popup_EndGame.transform.SetAsLastSibling();
        popup_EndGame.gameObject.SetActive(false);
        Timer.Schedule(this, timeDelay, () => {
            popup_EndGame.Show();
        });
    }

    public void ShowPopup_Settings() {
        if (popup_Settings == null) {
            popup_Settings = Instantiate(GameManager.ins.popup_Settings, popupCanvas.transform);
        }

        popup_Settings.transform.localScale = Vector3.one;
        popup_Settings.transform.SetAsLastSibling();
        popup_Settings.gameObject.SetActive(false);
        popup_Settings.Show();

    }

    public void ShowPopupTurnOnInternet() {
        if (popup_TurnOnInternet == null) {
            popup_TurnOnInternet = Instantiate(GameManager.ins.popup_TurnOnInternet, popupCanvas.transform);
        }
        popup_TurnOnInternet.transform.localScale = Vector3.one;
        popup_TurnOnInternet.transform.SetAsLastSibling();
        popup_TurnOnInternet.gameObject.SetActive(false);
        popup_TurnOnInternet.Show();
    }

    public void ShowPopup_Revive(Action actionRevive = null) {
        if (popup_Revive == null) {
            popup_Revive = Instantiate(GameManager.ins.popup_Revive, popupCanvas.transform);
        }
        popup_Revive.actionRevive = actionRevive;
        popup_Revive.transform.localScale = Vector3.one;
        popup_Revive.transform.SetAsLastSibling();
        popup_Revive.gameObject.SetActive(false);
        popup_Revive.Show();
    }
    public void ShowPopup_SelectMiniGame()
    {
        if (popup_SelectMiniGame == null)
        {
            popup_SelectMiniGame = Instantiate(GameManager.ins.popup_SelectMiniGame, popupCanvas.transform);
        }

        popup_SelectMiniGame.transform.localScale = Vector3.one;
        popup_SelectMiniGame.transform.SetAsLastSibling();
        popup_SelectMiniGame.gameObject.SetActive(false);
        popup_SelectMiniGame.Show();
    }
    public void ShowPopup_Skin()
    {
        if (popup_Skin == null)
        {
            popup_Skin = Instantiate(GameManager.ins.popup_Skin, popupCanvas.transform);
        }

        popup_Skin.transform.localScale = Vector3.one;
        popup_Skin.transform.SetAsLastSibling();
        popup_Skin.gameObject.SetActive(false);
        popup_Skin.Show();
    }

    public void ShowPopup_Earning() {
        if (popup_Earning == null) {
            popup_Earning = Instantiate(GameManager.ins.popup_Earning, popupCanvas.transform);
        }

        popup_Earning.transform.localScale = Vector3.one;
        popup_Earning.transform.SetAsLastSibling();
        popup_Earning.gameObject.SetActive(false);
        popup_Earning.Show();
    }
    public void ShowPopup_OfferFreeSkin()
    {
        if (popup_OfferFreeSkin == null)
        {
            popup_OfferFreeSkin = Instantiate(GameManager.ins.popup_OfferFreeSkin, popupCanvas.transform);
        }

        popup_OfferFreeSkin.transform.localScale = Vector3.one;
        popup_OfferFreeSkin.transform.SetAsLastSibling();
        popup_OfferFreeSkin.gameObject.SetActive(false);
        popup_OfferFreeSkin.Show();
    }

    public void ShowPopup_NoAds() {
        if (popup_NoAds == null) {
            popup_NoAds = Instantiate(GameManager.ins.popup_NoAds, popupCanvas.transform);
        }

        popup_NoAds.transform.localScale = Vector3.one;
        popup_NoAds.transform.SetAsLastSibling();
        popup_NoAds.gameObject.SetActive(false);
        popup_NoAds.Show();
    }

    public void ShowPopup_Spin() {
        if (popup_Spin == null) {
            popup_Spin = Instantiate(GameManager.ins.popup_Spin, popupCanvas.transform);
        }

        popup_Spin.transform.localScale = Vector3.one;
        popup_Spin.transform.SetAsLastSibling();
        popup_Spin.gameObject.SetActive(false);
        popup_Spin.Show();
    }
    #endregion
    public bool IsOpenPopUp() {
        //check xem dang mo UI nao
        if (popupList.Count < 0) return false;
        for (int i = 0; i < popupList.Count; i++) {
            if (popupList [i].gameObject.activeSelf) {
                return true;
            }
        }
        return false;
    }
}

