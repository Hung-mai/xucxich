using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Popup_NoAds : PopupBase {

    public override void Show() {
        base.Show();
        FirebaseManager.ins.ads_reward_offer("NoAds", "NoAds");
    }

    #region BUTTON
    public void BtnNoAds() {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowRewardedAd("NoAds", "NoAds", () => {
            DataManager.ins.gameSave.dayNoAds = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
            DataManager.ins.gameSave.secondNoAds = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1).AddDays(DataManager.ins.gameSave.dayNoAds)).TotalSeconds;
            SceneManager.ins.formHome.StartCoroutine(SceneManager.ins.formHome.CountdownNoAds());
            Close();
        });
    }

    public void BtnClose() {
        SoundManager.ins.sound_Click.PlaySound();
        Close();
    }
    #endregion
}
