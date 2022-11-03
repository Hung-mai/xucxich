using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Form_Gameplay : FormBase {
    public static Form_Gameplay ins;
    public int levelCur = 0;
    public Minigame minigameCur = 0;
    public int rankCur = 0;
    public int mapCur = 0;
    public int timeCur = 0;
    public int amountRevive = 0;
    public bool isClickBooster = false;
    [Header("-------------Object---------")]
    public GameObject fakeLoading;
    [HideInInspector] public int timePlayedLevel  = 0;

    [Header("-------------UI-------------")]
    public GameObject Obj_Time;
    public TextMeshProUGUI txt_Time;
    public TextMeshProUGUI txt_NameMinigame;
    public TextMeshProUGUI txt_Title;
    public GameObject obj_Title;
    public GameObject obj_TimeUp;
    public GameObject obj_BtnSettings;
    public Transform tran_BtnBooster;
    public GameObject obj_BtnStartGame;
    public TouchRotateSingle touchRotateSingle;

    public GameObject confetiFx;
    public SoundObject soundLast10Serconds;//Hiệu ứng âm thanh khi còn 10 giây cuối
    public GameObject obj_Tutorial;
    public Player player;
    public Indicator indicatorPlayer;
    public Minigame_Manager minigameManager;

    private void Awake() {
        if (GameManager.ins == null){
            SceneManager.ins.ChangeForm(FormUI.Form_Loading.ToString());
            return;
        }
        if (ins != null)  Destroy(ins.gameObject);
        ins = this;
        rankCur = GameManager.ins.rankMinigame >=0 ? GameManager.ins.rankMinigame :  Mathf.CeilToInt(DataManager.ins.gameSave.difficulty - 0.01f);
        rankCur = Mathf.Clamp(rankCur, 0, Constants.MAX_RANK_EACH_MINIGAME - 1);
        mapCur = GameManager.ins.mapMinigame >= 0 ? GameManager.ins.mapMinigame : DataManager.ins.gameSave.list_Minigames [(int)minigameCur].idMapPlayed_ByRank [rankCur];
        mapCur = Mathf.Clamp(mapCur, 0, Mathf.Max(0, GameManager.ins.arrayDatasMinigame[(int)minigameCur].rankMinigames[rankCur].prefab_Maps.Length - 1));

        amountRevive = 0;
        if (minigameCur == Minigame.OnTheCuttingBoard) {
            timeCur =  20 ;
        } else if (minigameCur == Minigame.GhostChaser) {
            timeCur = 25;
        } else {
            timeCur = Constants.TIME_GAMEPLAY;
        }
        Obj_Time.SetActive(false);
        fakeLoading.SetActive(true);
    }

    public override void Show()
    {
        base.Show();
        if (GameManager.ins == null || DataManager.ins == null) return;
        DataManager.ins.gameSave.levelStart++;
               obj_Tutorial.SetActive(false);
        tran_BtnBooster.gameObject.SetActive(false);
        obj_BtnSettings.gameObject.SetActive(false);
        obj_BtnStartGame.SetActive(false);
        SceneManager.ins.txt_UIMoney.text = DataManager.ins.gameSave.gold + "";
        if(SceneManager.ins.obj_CupTxt != null)SceneManager.ins.obj_CupTxt.text = DataManager.ins.gameSave.amountWinAll.ToString();
        //UI Level
        levelCur = DataManager.ins.gameSave.level;
        txt_NameMinigame.text = "Level " + (DataManager.ins.gameSave.level + 1);
        txt_Title.text = "" + GameManager.ins.arrayDatasMinigame[(int)minigameCur].txt_Tutorial;
        //Ản UI
        if (SceneManager.ins.formCurrent != null) SceneManager.ins.formCurrent.GetComponent<CanvasGroup>().alpha = GameManager.ins.isHideUIGameplay ? 0 : 1;
        if (SceneManager.ins.loadingCanvas != null) SceneManager.ins.loadingCanvas.GetComponent<CanvasGroup>().alpha = GameManager.ins.isHideUIGameplay ? 0 : 1;
        if (SceneManager.ins.canvas_Joystick != null) SceneManager.ins.canvas_Joystick.GetComponent<CanvasGroup>().alpha = GameManager.ins.isHideUIGameplay ? 0 : 1;

        if (DataManager.ins.gameSave.levelEnded >= PluginManager.ins.Level_PopupTurnOnInternet && Application.internetReachability == NetworkReachability.NotReachable && !Application.isEditor)
        {//Nếu ko có Internet sau level 3 thì ko cho chơi tiếp
            SceneManager.ins.ShowPopupTurnOnInternet();
        }
        FirebaseManager.ins.OnSetProperty("start_minigame", DataManager.ins.gameSave.levelStart);//số lần vào chơi minigame (Chơi lại vẫn bắn)
        FirebaseManager.ins.level_start(minigameCur.ToString(), rankCur, mapCur, DataManager.ins.gameSave.levelEnded + 1, DataManager.ins.gameSave.gold);
        //Load map của minigame tương ứng
        if (minigameManager == null) {
            minigameManager = Instantiate(GameManager.ins.arrayDatasMinigame [(int)minigameCur].rankMinigames [rankCur].prefab_Maps [mapCur]);
        }
        minigameManager.listChars.Add(player);
        minigameManager.listIndicators.Add(indicatorPlayer);
        Timer.Schedule(this, 0.5f, () => {
            MaxManager.ins.ReloadBanner();
        });
    }

    public void UIIntroGame() {
        Timer.Schedule(this, 0.5f, () => {
            if(PluginManager.ins.isShowBooster && DataManager.ins.gameSave.levelEnded >= 1 && minigameManager.minigame != Minigame.HitAndRun ) tran_BtnBooster.gameObject.SetActive(true);
            //tran_BtnBooster.localPosition = Vector3.zero;
            obj_BtnSettings.gameObject.SetActive(true);
            obj_BtnStartGame.SetActive(true);
        });
        obj_Tutorial.gameObject.SetActive(true);
        if (SceneManager.ins.canvas_Joystick != null) SceneManager.ins.canvas_Joystick.gameObject.SetActive(true);
        //StartCoroutine(Countdown321());
    }
    /*
    public IEnumerator Countdown321() {
        yield return new WaitForSeconds(0.65f);
        obj_Title.SetActive(true);
        tutorial.gameObject.SetActive(true);
        if(SceneManager.ins.canvas_Joystick != null) SceneManager.ins.canvas_Joystick.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        soundFirst3Serconds.PlaySound();
        yield return new WaitForSeconds(1);
        txt_321.text = "2";
        yield return new WaitForSeconds(1);
        txt_321.text = "1";
        yield return new WaitForSeconds(1);
        txt_321.text = "GO!";
        yield return new WaitForSeconds(0.5f);
        obj_Title.SetActive(false);
        minigameManager.StartGame();
    }*/

    public void UIStartGame() {
        obj_Title.SetActive(false);
        if (timeCur >= 0) {
            txt_Time.text = "" + timeCur;
            Obj_Time.SetActive(true);
            StartCoroutine(CountdownGameplay());
        }
    }

    public IEnumerator CountdownGameplay() {
        timeCur--;
        if (timeCur == 10 && SceneManager.ins.popup_EndGame == null) {//Phát âm thanh đếm ngược 10s cuối
            soundLast10Serconds.PlaySound();
        }
        if (timeCur <= 10) { 
            txt_Time.DOColor(timeCur % 2 == 0 ? new Color(1,0.5f,0.5f) : Color.white, 1);; 
        }
        if (timeCur >= 0) {
            txt_Time.text = "" + timeCur;
            yield return new WaitForSeconds(0.9f);
            StartCoroutine(CountdownGameplay());
        }else {
            if (SceneManager.ins.popup_EndGame == null) {
                obj_TimeUp.SetActive(true);
                Timer.Schedule(this, 2.5f, () => {
                    obj_TimeUp.SetActive(false);
                });
            }
            minigameManager.EndGame();
        }
    }


    private float timeClick = 0;
    private void Update() {
        if (timeClick >= 0) timeClick-= Time.deltaTime;
        if (Input.GetMouseButtonDown(0)) {
            timeClick = 0.2f;
        }
    }

    #region Button
    public void Btn_Setting() {
        if (timeClick < 0) return;
            if (SceneManager.ins.popup_Settings == null || SceneManager.ins.popup_Settings.gameObject.activeSelf == false) {
                SoundManager.ins.sound_Click.PlaySound();
                SceneManager.ins?.ShowPopup_Settings();
            }
        
    }

    public void Btn_StartGame() {
        if (Input.GetMouseButtonDown(0) && !minigameManager.isGameplay_Start) {
            SoundManager.ins.sound_Click.PlaySound();
            obj_BtnStartGame.SetActive(false);
            touchRotateSingle.dragging = true;
            touchRotateSingle.mousePos = Input.mousePosition;
            minigameManager.StartGame();
            Timer.Schedule(this, 5f, () => {
                if (PluginManager.ins.isShowBooster ) tran_BtnBooster.gameObject.SetActive(false);
            });
        }
    }

    public void Btn_Booster() {
        if (timeClick < 0) return;
        if ( !isClickBooster) {
            isClickBooster = true;
            SoundManager.ins.sound_Click.PlaySound();
            SceneManager.ins.BlockInput(0.5f);
            MaxManager.ins.ShowRewardedAd("Booster", "Booster_" + Form_Gameplay.ins.minigameCur.ToString(), () => {
                isClickBooster = false;
                minigameManager.listChars [0].isBooster = true;
                minigameManager.listChars [0].timeRunToWin *= 0.75f;
                minigameManager.listChars [0].modelChar.effect_TrailBooster.startColor = GameManager.ins.listColorsChar [minigameManager.listChars [0].modelChar.idColor];
                minigameManager.listChars [0].modelChar.effect_TrailBooster.gameObject.SetActive(true);
                minigameManager.listChars [0].effect_SpeedUp.startColor = GameManager.ins.listColorsChar [minigameManager.listChars [0].modelChar.idColor];
                minigameManager.listChars [0].effect_SpeedUp.Play();
                for (int i = 0; i < minigameManager.listChars[0].modelChar.obj_shoes.Length; i++) {
                    minigameManager.listChars [0].modelChar.obj_shoes [i].SetActive(true);
                }
                /*
                for (int i = 1; i < minigameManager.listChars.Count; i++) {
                    minigameManager.listChars[i].speedBack *= 2f;
                }*/
                tran_BtnBooster.gameObject.SetActive(false);
            }, null);
        }
    }
    #endregion
}
