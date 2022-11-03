using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Revive : PopupBase {
    public int time;
    public bool isWacthingVideoAds = false;
    private int goldOld;
    private int costRevive;
    public TextMeshProUGUI txt_Time;
    public TextMeshProUGUI txt_GoldOn;
    public TextMeshProUGUI txt_GoldOff;
    public GameObject obj_BtnReviveGold_On;
    public GameObject obj_BtnReviveGold_Off;
    public DOTweenAnimation twn_BtnRevive;
    public DOTweenAnimation twn_Countdown;
    public Image image_ButtonFree;
    public SoundObject soundCountdown;
    public Action actionRevive;

    public override void Show() {
        base.Show();
        if (Form_Gameplay.ins.timeCur < 3) {
            Form_Gameplay.ins.minigameManager.listChars [0].isAnimationDyning = false;
            SceneManager.ins.ShowPopup_EndGame(false);
            Close();
            return;
        }
        time = 3;
        txt_Time.text = time + "";
        goldOld = DataManager.ins.gameSave.gold;
        if (SceneManager.ins.obj_Gold != null) SceneManager.ins.obj_Gold.SetActive(true);
        costRevive =  Mathf.Min(300, (int)(PluginManager.ins.CostReviveStart  + Form_Gameplay.ins.amountRevive * PluginManager.ins.CostReviveIncrease));
        txt_GoldOn.text = costRevive + " $";
        txt_GoldOff.text = costRevive + " $";
        if (costRevive <= 0) {
            obj_BtnReviveGold_On.SetActive(false);
            obj_BtnReviveGold_Off.SetActive(false);
            twn_BtnRevive.DORestart();
        } else if (DataManager.ins.gameSave.gold >= costRevive) {
            obj_BtnReviveGold_On.SetActive(true);
            obj_BtnReviveGold_Off.SetActive(false);
        } else {
            obj_BtnReviveGold_On.SetActive(false);
            obj_BtnReviveGold_Off.SetActive(true);
        }

        soundCountdown.PlaySound();//bật tiếng Countdown time
        twn_Countdown.DORestart();//Ảnh loading quay hết một vòng thì tự động gọi hàm CountTime() 1 lần
        SceneManager.ins.isPause = true;
        isWacthingVideoAds = false;
        Form_Gameplay.ins.soundLast10Serconds.Pause();
        FirebaseManager.ins.ads_reward_offer("Revive", "");
    }

    //Ảnh loading quay hết một vòng thì tự động gọi hàm CountTime() 1 lần
    public void CountTime() {
        if (isWacthingVideoAds) return;//Nếu đang xem VideoAds thì bỏ qua lần đếm này
        //Check VideoAds
        if (MaxManager.ins.isRewardedVideoAvailable() || Application.isEditor) {//Nếu VideoAds sẵn sàng -> Hiển thị button sáng lên
            image_ButtonFree.color = Color.white;
        } else {//Nếu Chưa sẵn sàng -> Button Video Ads mờ đi
            image_ButtonFree.color = Color.gray;
        }
        time--;
        if (time>=0) txt_Time.text = time + "";
        if (time < 0) { //Hết thời gian thì bật Popup thua lên
            if (time == -1) LoseIt();
        } else {//Nếu chưa hết thời gian thì bật tiếng Countdown time
            soundCountdown.PlaySound();
        }
    }

    //
    private void LoseIt() {
        Close();
        Form_Gameplay.ins.minigameManager.listChars [0].isAnimationDyning = false;
        SceneManager.ins.isPause = false;
        SceneManager.ins.ShowPopup_EndGame(false);
        //MaxManager.ins.ShowInterstitial("Revive", () => {
           
        //});
    }

    #region Button
    public void Btn_ReviveByAds() {
        SoundManager.ins.sound_Click.PlaySound();
        isWacthingVideoAds = true;
        MaxManager.ins.ShowRewardedAd("Revive", "Revive_" +Form_Gameplay.ins.minigameCur.ToString(), () => {
            Form_Gameplay.ins.amountRevive++;
            Form_Gameplay.ins.soundLast10Serconds.UnPause();
            Close();
            SceneManager.ins.isPause = false;
            if (actionRevive != null) {
                actionRevive.Invoke();
                actionRevive = null;
            }
            FirebaseManager.ins.level_revive(Form_Gameplay.ins.minigameManager.minigame.ToString(), Form_Gameplay.ins.rankCur, Form_Gameplay.ins.mapCur, DataManager.ins.gameSave.levelEnded + 1, Form_Gameplay.ins.timeCur , true);
            if (SceneManager.ins.obj_Gold != null) SceneManager.ins.obj_Gold.SetActive(false);
        }, () => {
            isWacthingVideoAds = false;
        });
    }

    public void Btn_ReviveByGold() {
        SoundManager.ins.sound_Click.PlaySound();
        isWacthingVideoAds = true;
        //Trừ tiền nếu đủ -> 
        if (DataManager.ins.gameSave.gold >= costRevive) {
            Form_Gameplay.ins.amountRevive++;
            DataManager.ins.ChangeGold(-costRevive, "Revive", true, true);
            Form_Gameplay.ins.soundLast10Serconds.UnPause();
            Close();
            SceneManager.ins.isPause = false;
            if (actionRevive != null) {
                actionRevive.Invoke();
                actionRevive = null;
            }
            if (SceneManager.ins.obj_Gold != null) SceneManager.ins.obj_Gold.SetActive(false);
            FirebaseManager.ins.level_revive(Form_Gameplay.ins.minigameManager.minigame.ToString(), Form_Gameplay.ins.rankCur, Form_Gameplay.ins.mapCur, DataManager.ins.gameSave.levelEnded + 1, Form_Gameplay.ins.timeCur, false);
        }
    }

    public void Btn_LoseIt() {
        SoundManager.ins.sound_Click.PlaySound();
        LoseIt();
    }
    #endregion
}

