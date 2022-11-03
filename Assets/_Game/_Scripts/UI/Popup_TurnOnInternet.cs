using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Popup_TurnOnInternet : PopupBase
{
    //public bool isCheckingInternet = false;

    public override void Show()
    {
        base.Show();
        SceneManager.ins.isPause = SceneManager.ins.isPauseSound = true;
    }

    public override void Close(bool showUIForm = true) {
        base.Close(showUIForm);
    }
    /*
    public IEnumerator CheckInternet() {
        UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();
        isCheckingInternet = false;

        Debug.LogError("Internet: " + (request.error != null));
        if (request.error != null && Application.internetReachability != NetworkReachability.NotReachable) {
            SceneManager.ins.isPause = SceneManager.ins.isPauseSound = false;
            Close();
        } else {
            if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false)
                SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
        }
    }*/

    #region BUTTON
    public void BtnOK() {
        SoundManager.ins.sound_Click.PlaySound();
        /*if (!isCheckingInternet) {
            isCheckingInternet = true;
            StartCoroutine(CheckInternet());
        }*/
        if(Application.internetReachability != NetworkReachability.NotReachable) {
            SceneManager.ins.isPause = SceneManager.ins.isPauseSound = false;
            Close();
            if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == true)
                SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(false);
        } else {
            if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false)
                SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
        }
    }
    #endregion
}

