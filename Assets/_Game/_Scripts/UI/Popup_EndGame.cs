using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Popup_EndGame : PopupBase {
    public bool isWin = false;
    int goldClaim = 0;
    int amountNode = 0;
    public LevelProgressBar progressBar;
    
    [Header("------------- Completed ------------")]
    public GameObject WinPanel;
    public GameObject obj_Case1_NoOffer;
    public GameObject obj_Case2_OfferSkin;
    public TextMeshProUGUI txt_progress;
    public TextMeshProUGUI txt_MultiMoney;
    public TextMeshProUGUI txt_MultiMoneyClaimAll;
    public GameObject obj_BtnMultiMoney;
    public GameObject obj_BtnClaimAll;
    public GameObject obj_Btn100IsEnough;
    public GameObject obj_Btn100IsEnough_skin;
    public RectTransform rect_BGOfferSkin;
    public DOTweenAnimation twn_IconSkin;
    public SoundObject soundWin;
    public GameObject obj_Fail_BtnHomeOnMid;
    public GameObject obj_Fail_BtnHomeOnTop;

    public float rotateSpdLight = 5f;


    [Header("------------- Game Over ------------")]
    public GameObject LosePanel;
    public SoundObject soundLose;

    private int currentGold;

    [Header("------------ref-----------")]
    public Popup_EndGame_SkinOffer skinOffer_No;

    [HideInInspector]public float currentProcess;
    [SerializeField]private int processPerLevel = 50;

    public override void Show() {
        currentGold = DataManager.ins.gameSave.gold;
        amountNode = (int)Mathf.Clamp(PluginManager.ins.NodeStart_WinAll + DataManager.ins.gameSave.amountWinAll * PluginManager.ins.NodeIncease_WinAll, 0, 8);
        int amountMap = GameManager.ins.arrayDatasMinigame [(int)Form_Gameplay.ins.minigameCur].rankMinigames [Form_Gameplay.ins.rankCur].prefab_Maps.Length;
        FirebaseManager.ins.checkpoint_endgame(DataManager.ins.gameSave.levelEnded + 1, Form_Gameplay.ins.minigameCur.ToString(), Form_Gameplay.ins.rankCur, Form_Gameplay.ins.mapCur, isWin);
        DataManager.ins.gameSave.list_Minigames [(int)Form_Gameplay.ins.minigameCur].idMapPlayed_ByRank [Form_Gameplay.ins.rankCur] = Mathf.Clamp(DataManager.ins.gameSave.list_Minigames [(int)Form_Gameplay.ins.minigameCur].idMapPlayed_ByRank [Form_Gameplay.ins.rankCur]  + 1, 0, Mathf.Max(0, amountMap - 1));
        DataManager.ins.gameSave.levelEnded++;
        SceneManager.ins.obj_Gold.SetActive(true);
        SceneManager.ins.obj_CupTxt.text = DataManager.ins.gameSave.amountWinAll.ToString(); SceneManager.ins.obj_Cup.SetActive(true);
        Form_Gameplay.ins.soundLast10Serconds.Pause();
        //Nếu skin nào dùng 1 lần thì hủy Unlock
        if (DataManager.ins.gameSave.idSkin_Full >= 0 && DataManager.ins.gameSave.listSkins_Full[DataManager.ins.gameSave.idSkin_Full].play1Game_Unlock1Time) {
            if (DataManager.ins.gameSave.listSkins_Full [DataManager.ins.gameSave.idSkin_Full].isBuy) {
                DataManager.ins.gameSave.listSkins_Full [DataManager.ins.gameSave.idSkin_Full].play1Game_Unlock1Time = false;
            } else {
                DataManager.ins.gameSave.listSkins_Full [DataManager.ins.gameSave.idSkin_Full].isUnlock = false;
                DataManager.ins.gameSave.listSkins_Full [DataManager.ins.gameSave.idSkin_Full].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.idSkin_Full = -1;
                DataManager.ins.gameSave.idSkin_Hair = -1;
                DataManager.ins.gameSave.idSkin_Hand = -1;
                DataManager.ins.gameSave.idSkin_Body = 0;
            }
        }
        if (DataManager.ins.gameSave.idSkin_Hair >= 0 && DataManager.ins.gameSave.listSkins_Hair[DataManager.ins.gameSave.idSkin_Hair].play1Game_Unlock1Time) {
            if (DataManager.ins.gameSave.listSkins_Hair [DataManager.ins.gameSave.idSkin_Hair].isBuy) {
                DataManager.ins.gameSave.listSkins_Hair [DataManager.ins.gameSave.idSkin_Hair].play1Game_Unlock1Time = false;
            } else {
                DataManager.ins.gameSave.listSkins_Hair [DataManager.ins.gameSave.idSkin_Hair].isUnlock = false;
                DataManager.ins.gameSave.listSkins_Hair [DataManager.ins.gameSave.idSkin_Hair].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.idSkin_Hair = -1;
            }
        }
        if (DataManager.ins.gameSave.idSkin_Hand >= 0 && DataManager.ins.gameSave.listSkins_Hand[DataManager.ins.gameSave.idSkin_Hand].play1Game_Unlock1Time) {
            if (DataManager.ins.gameSave.listSkins_Hand [DataManager.ins.gameSave.idSkin_Hand].isBuy) {
                DataManager.ins.gameSave.listSkins_Hand [DataManager.ins.gameSave.idSkin_Hand].play1Game_Unlock1Time = false;
            } else {
                DataManager.ins.gameSave.listSkins_Hand [DataManager.ins.gameSave.idSkin_Hand].isUnlock = false;
                DataManager.ins.gameSave.listSkins_Hand [DataManager.ins.gameSave.idSkin_Hand].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.idSkin_Hand = -1;
            }
        }
        if (DataManager.ins.gameSave.idSkin_Body > 0 && DataManager.ins.gameSave.listSkins_Body[DataManager.ins.gameSave.idSkin_Body].play1Game_Unlock1Time) {
            if (DataManager.ins.gameSave.listSkins_Body [DataManager.ins.gameSave.idSkin_Body].isBuy) {
                DataManager.ins.gameSave.listSkins_Body [DataManager.ins.gameSave.idSkin_Body].play1Game_Unlock1Time = false;
            } else {
                DataManager.ins.gameSave.listSkins_Body [DataManager.ins.gameSave.idSkin_Body].isUnlock = false;
                DataManager.ins.gameSave.listSkins_Body [DataManager.ins.gameSave.idSkin_Body].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.idSkin_Body = 0;
            }
        }
        AppsflyerEventRegister.af_achievement_unlocked(DataManager.ins.gameSave.levelEnded, Form_Gameplay.ins.minigameCur.ToString(), isWin);
        if (isWin) {
            goldClaim = 100;
            progressBar.RefreshLevelProgressBar(1);
            DataManager.ins.gameSave.level++;
            DataManager.ins.gameSave.levelPass++;
            DataManager.ins.gameSave.amountWin_AfterShowInter++;
            DataManager.ins.gameSave.isRandomMinigameNext = true;
            DataManager.ins.gameSave.amountFailInLevelCur = 0;
            FirebaseManager.ins.OnSetProperty("level_played", DataManager.ins.gameSave.levelPass);//số level đã chơi(số level đã start -mỗi level chỉ bắn 1 lần)
            if (DataManager.ins.gameSave.daysPlayed == 0) FirebaseManager.ins.OnSetProperty("level_played_1stday", DataManager.ins.gameSave.levelPass);
            FirebaseManager.ins.level_complete(Form_Gameplay.ins.minigameCur.ToString(), Form_Gameplay.ins.rankCur, Form_Gameplay.ins.mapCur, DataManager.ins.gameSave.levelEnded, (int)Time.timeSinceLevelLoad);
            if (DataManager.ins.gameSave.level >= amountNode) { 
                DataManager.ins.gameSave.level = 0;
                DataManager.ins.gameSave.amountWinAll++;
                DataManager.ins.gameSave.showSpinWinAll = true;
                DataManager.ins.gameSave.difficulty = Mathf.Clamp(Mathf.CeilToInt(DataManager.ins.gameSave.difficulty)  + 1, 0, Constants.MAX_RANK_EACH_MINIGAME - 1);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_NangCup");
                return;
            } else {
                base.Show();
                WinPanel.SetActive(true);
                LosePanel.SetActive(false);
                obj_Case1_NoOffer.SetActive(false);
                obj_Case2_OfferSkin.SetActive(false);
                if (DataManager.ins.gameSave.progressSkin >= 100) {
                    if (DataManager.ins.gameSave.currentSkinUnlock.keyID.Contains("Hair")) skinOffer_No.skin = Popup_EndGame_SkinOffer.Skin.hand;
                    else if (DataManager.ins.gameSave.currentSkinUnlock.keyID.Contains("hand")) skinOffer_No.skin = Popup_EndGame_SkinOffer.Skin.hair;
                    DataManager.ins.gameSave.progressSkin = 0;
                    DataManager.ins.gameSave.currentSkinUnlock = new Skin_Save("");
                }
                currentProcess = DataManager.ins.gameSave.progressSkin;
                DataManager.ins.gameSave.progressSkin += processPerLevel;
                skinOffer_No.OnProgressComplete = ShowButtonOffer;
                skinOffer_No.ShowOffer();
                SceneManager.ins.BlockInput(0.5f);
                Timer.Schedule(this, 0.5f, () => {
                    MaxManager.ins.ShowInterstitial("Home_EndGame", () => {
                        soundWin.PlaySound();
                        skinOffer_No.CallRunProcess();
                        if (DataManager.ins.gameSave.levelEnded >= PluginManager.ins.Level_PopupTurnOnInternet && Application.internetReachability == NetworkReachability.NotReachable && !Application.isEditor) {//Nếu ko có Internet sau level 5 thì ko cho chơi tiếp
                            SceneManager.ins.ShowPopupTurnOnInternet();
                        }
                    });
                });
            }
        } else {
            base.Show();
            WinPanel.SetActive(false);
            LosePanel.SetActive(true);
            obj_Fail_BtnHomeOnTop.SetActive(PluginManager.ins.PopupFail_ButtonHomeOnTop);
            obj_Fail_BtnHomeOnMid.SetActive(!PluginManager.ins.PopupFail_ButtonHomeOnTop);
            DataManager.ins.gameSave.amountLose_AfterShowInter++;
            DataManager.ins.gameSave.difficulty = Mathf.Clamp(DataManager.ins.gameSave.difficulty - 2f/ amountNode + 0.002f, 0, Constants.MAX_RANK_EACH_MINIGAME - 1);
            DataManager.ins.gameSave.amountFailInLevelCur++;
            progressBar.RefreshLevelProgressBar(0);
            goldClaim = 0;
            DataManager.ins.gameSave.level = 0;
            DataManager.ins.gameSave.isRandomMinigameNext = true;
            FirebaseManager.ins.level_fail(Form_Gameplay.ins.minigameCur.ToString(), Form_Gameplay.ins.rankCur, Form_Gameplay.ins.mapCur, DataManager.ins.gameSave.levelEnded, DataManager.ins.gameSave.amountFailInLevelCur);
            FirebaseManager.ins.ads_reward_offer("Restart_PopupLose", "");
            SceneManager.ins.BlockInput(0.5f);
            Timer.Schedule(this, 0.5f, () => {
                MaxManager.ins.ShowInterstitial("Home_EndGame", () => {
                    soundLose.PlaySound();
                    if (DataManager.ins.gameSave.levelEnded >= PluginManager.ins.Level_PopupTurnOnInternet && Application.internetReachability == NetworkReachability.NotReachable && !Application.isEditor) {//Nếu ko có Internet sau level 5 thì ko cho chơi tiếp
                        SceneManager.ins.ShowPopupTurnOnInternet();
                    } else if (DataManager.ins.gameSave.progressSkin < 100 && GameManager.ins.timeOffer > PluginManager.ins.timeOffer) {
                        twn_IconSkin.gameObject.SetActive(false);
                        SceneManager.ins.ShowPopup_OfferFreeSkin();
                        SceneManager.ins.popup_OfferFreeSkin.OnClosed +=() => { twn_IconSkin.gameObject.SetActive(true); };
                    }
                });
            });
        }
    }
    public void ShowButtonOffer()
    {
        if (DataManager.ins.gameSave.progressSkin >= 100)
        {
            FirebaseManager.ins.ads_reward_offer("ClaimAll_PopupWin", "");
            skinOffer_No.IncreaseDataCountTimeOffer();
            obj_Case1_NoOffer.SetActive(false);
            obj_Case2_OfferSkin.SetActive(false);
            txt_progress.gameObject.SetActive(false);
            obj_BtnClaimAll.SetActive(false);
            obj_Btn100IsEnough_skin.SetActive(false);
            txt_MultiMoneyClaimAll.text = ""+ goldClaim *3;
            twn_IconSkin.DOPlay();
            Timer.Schedule(this, 0.5f, () => {
                rect_BGOfferSkin.DOSizeDelta(new Vector2(rect_BGOfferSkin.sizeDelta.x, 920), 0.8f);
            });
            Timer.Schedule(this, 1.4f, () => {
                obj_Case2_OfferSkin.SetActive(true);
                obj_BtnClaimAll.SetActive(true);
            });
            Timer.Schedule(this, 3.0f, () => {
                obj_Btn100IsEnough_skin.SetActive(true);
            });
            //skinOffer_No.imgBgLight.transform.DOScale(1.25f, 1f).SetEase(Ease.OutCirc);
            //BgLight_Tween.duration = rotateSpdLight;
        }
        else
        {
            FirebaseManager.ins.ads_reward_offer("MultiReward_PopupWin", "");
            Timer.Schedule(this, 0.5f, () => {
                obj_Case1_NoOffer.SetActive(true);
                obj_Case2_OfferSkin.SetActive(false);
                obj_Btn100IsEnough.SetActive(false);
                txt_MultiMoney.text = "+"+ goldClaim *3;
            });
            Timer.Schedule(this, 2.0f, () => {
                obj_Btn100IsEnough.gameObject.SetActive(true);
            });
        }
        
    }
    #region Button
    public void Btn_Home() {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowInterstitial("Home_EndGame", () => {
            SceneManager.ins.BlockInput(true);
            SceneManager.ins.effectGem.ShowEffect(goldClaim, Camera.main.WorldToScreenPoint(obj_Btn100IsEnough_skin.transform.position), SceneManager.ins.iconGold.transform,
                () => {
                    DataManager.ins.ChangeGold(goldClaim, "Home_EndGame", true, false);
                    SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                        SceneManager.ins.BlockInput(false);
                       SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
                    });
                });
        }, MaxManager.ins.isInterLastSuccess);
    }
    public void Btn_Replay()
    {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowRewardedAd("Replay_EndGame", "Replay_" + Form_Gameplay.ins.minigameCur.ToString(), () => {
            DataManager.ins.gameSave.level = Form_Gameplay.ins.levelCur;
            DataManager.ins.gameSave.isRandomMinigameNext = false;
            SceneManager.ins.ChangeForm_Gameplay(0);
        }, null);
    }
    public void Btn_Skip()
    {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowRewardedAd("Skip_EndGame","Skip", ()=> {
            DataManager.ins.gameSave.amountFailInLevelCur = 0;
            DataManager.ins.gameSave.levelPass++;
            FirebaseManager.ins.OnSetProperty("level_played", DataManager.ins.gameSave.levelPass);//số level đã chơi(số level đã start -mỗi level chỉ bắn 1 lần)
            if (DataManager.ins.gameSave.daysPlayed == 0) FirebaseManager.ins.OnSetProperty("level_played_1stday", DataManager.ins.gameSave.levelPass);
            DataManager.ins.gameSave.level = Form_Gameplay.ins.levelCur + 1;
            if (DataManager.ins.gameSave.level >= amountNode) {
                DataManager.ins.gameSave.level = 0;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_NangCup");
                return;
            }
            SceneManager.ins.ChangeForm_Gameplay(0);
        },null);
    }
    public void Btn_MultiReward() {
        MaxManager.ins.ShowRewardedAd("MultiReward_EndGame", "MultiReward_EndGame", () => {
            SceneManager.ins.BlockInput(true);
            SceneManager.ins.effectGem.ShowEffect(goldClaim * 3, Camera.main.WorldToScreenPoint(obj_BtnMultiMoney.transform.position), SceneManager.ins.iconGold.transform,
                () => {
                    DataManager.ins.ChangeGold(goldClaim * 3, "MultiReward_EndGame", true, false);
                    SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                        SceneManager.ins.BlockInput(false);
                        SceneManager.ins.ChangeForm_Gameplay(0);
                    });
                });
        });
    }
    public void Btn_ClaimAll()
    {
        MaxManager.ins.ShowRewardedAd("ClaimAll_EndGame", "ClaimAll_EndGame", () => {
            SceneManager.ins.BlockInput(true);
            skinOffer_No.GetSkin();
            SceneManager.ins.effectGem.ShowEffect(goldClaim * 3, Camera.main.WorldToScreenPoint(obj_BtnMultiMoney.transform.position), SceneManager.ins.iconGold.transform,
                () => {
                    DataManager.ins.ChangeGold(goldClaim * 3, "ClaimAll_EndGame", true, false);
                    SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                        SceneManager.ins.BlockInput(false);
                        SceneManager.ins.ChangeForm_Gameplay(0);
                    });
                });
        });
    }
    public void Btn_Next() {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowInterstitial("Continue_EndGame", () => {
            //SceneManager.ins.ChangeForm_Gameplay(1, true);
            //ResetSaveSkinOffer();
            SceneManager.ins.BlockInput(true);
            //skinOffer_No.GetSkin();
            SceneManager.ins.effectGem.ShowEffect(goldClaim, Camera.main.WorldToScreenPoint(obj_Btn100IsEnough_skin.transform.position), SceneManager.ins.iconGold.transform,
                () => {
                    DataManager.ins.ChangeGold(goldClaim, "Continue_EndGame", true, false);
                    SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                        SceneManager.ins.BlockInput(false);
                        SceneManager.ins.ChangeForm_Gameplay(0);
                    });
                });
        }, MaxManager.ins.isInterLastSuccess);
    }

    public void Btn_StartOver() {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowInterstitial("StartOver_EndGame", () => {
            SceneManager.ins.ChangeForm_Gameplay(0);
        }, MaxManager.ins.isInterLastSuccess);
    }
    #endregion
}

