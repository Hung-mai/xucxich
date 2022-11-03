using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Popup_Earning : PopupBase {
    public TextMeshProUGUI txt_Gold_Collect;
    public TextMeshProUGUI txt_Gold_X3;

    private int goldOld; 
    public int goldEarning = 250;
    public int goldEarningAdsReward = 1000;

    public override void Show() {
        base.Show();
        GameManager.ins.isShowOfflineEarning = false;
        goldOld = DataManager.ins.gameSave.gold;
        FirebaseManager.ins.ads_reward_offer("Offline Earning", "Offline Earning");
        txt_Gold_X3.text = "+" + goldEarningAdsReward;
        txt_Gold_Collect.text = goldEarning + "";
    }

    #region BUTTON
    public void BtnCollect() {
        SoundManager.ins.sound_Click.PlaySound();
        SceneManager.ins.BlockInput(true);
        SceneManager.ins.effectGem.ShowEffect(goldEarning, Camera.main.WorldToScreenPoint(txt_Gold_Collect.transform.position), SceneManager.ins.iconGold.transform,
                () => {
                    DataManager.ins.gameSave.timeOfflineEarning = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
                    DataManager.ins.ChangeGold(goldEarning, "OfflineEarning", true, false);
                    SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
                        SceneManager.ins.BlockInput(false);
                        Close();
                    });
                });
    }

    public void BtnX3Reward() {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowRewardedAd("OfflineEarning", "OfflineEarning", () => {
            SceneManager.ins.BlockInput(true);
            SceneManager.ins.effectGem.ShowEffect(goldEarningAdsReward, Camera.main.WorldToScreenPoint(txt_Gold_X3.transform.position), SceneManager.ins.iconGold.transform,
                    () => {
                        DataManager.ins.gameSave.timeOfflineEarning = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
                        DataManager.ins.ChangeGold(goldEarningAdsReward, "X3OfflineEarning", true, false);
                        SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
                            SceneManager.ins.BlockInput(false);
                            Close();
                        });
                    });
        });
    }
    #endregion
}
