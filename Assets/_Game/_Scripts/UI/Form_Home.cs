using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Form_Home : FormBase {
    public GameObject obj_FakeLoading;
    public TMP_InputField name_InputField;
    public LevelProgressBar progressBar;
    public camFollower camFollower;

    public GameObject obj_BtnSetting;
    public GameObject obj_BtnSkin;
    public GameObject obj_BtnOfferGold;
    public TextMeshProUGUI txt_OfferGold;
    public Button btn_play;
    public Button btn_NoAds;
    public TextMeshProUGUI txt_NoAds;

    private int goldOld = 0;
    public int idCharSelect = 0;
    private int idOfferSkin2 = -1;
    private int idOfferSkin3 = -1;
    public bool isSkin2_Unlock = false;
    public bool isSkin3_Unlock = false;
    public ModelChar modelChar;
    public CharacterFormHome [] listChars;
    public Transform tran_RotateChars;
    public DataSkin_OfferHome [] listDataSkinOffer;
    public TextMeshProUGUI txt_UnlockChar2;
    public TextMeshProUGUI txt_UnlockChar3;
    public GameObject obj_BtnLeft;
    public GameObject obj_BtnRight;
    //public GameObject obj_BtnUnlock;

    public Image img_MinigameCur;
    public Sprite logo;
    public TextMeshProUGUI txt_NameMinigameCur;
    public GameObject obj_Logo;
    public GameObject obj_AllButton;

    private void Update() {
        for (int i = 0; i < listChars.Length; i++) {
            listChars [i].transform.eulerAngles = Vector3.zero;
        }
    } 
    public override void Show()
    {
        base.Show();
        obj_FakeLoading.SetActive(true);
        idCharSelect = 0;
        name_InputField.text = DataManager.ins.gameSave.username;
        progressBar.RefreshLevelProgressBar();
        txt_OfferGold.text = "+ " + PluginManager.ins.GoldOffer_Shop;

        //Nếu có 1 skin nào trong bộ đã mua thì bỏ qua bộ đó
        idOfferSkin2 = -1;
        idOfferSkin3 = -1;
        List<int> listIDOfferSkin = new List<int>();
        for (int i = 0; i < listDataSkinOffer.Length; i++) {
            bool isSkinFullNotUnlock = listDataSkinOffer[i].idSkinFull >= 0 && listDataSkinOffer[i].idSkinFull < DataManager.ins.gameSave.listSkins_Full.Length && !DataManager.ins.gameSave.listSkins_Full [listDataSkinOffer[i].idSkinFull].isUnlock;
            if (isSkinFullNotUnlock)
            {
                listIDOfferSkin.Add(i);
            }
        }

        if (listIDOfferSkin.Count > 1) {//Phải đủ 2 nhân vật thì mới Offer
            int idRandom = Random.Range(0, listIDOfferSkin.Count);
            idOfferSkin2 = listIDOfferSkin [idRandom];
            listIDOfferSkin.RemoveAt(idRandom);
            if (listDataSkinOffer [idOfferSkin2].idSkinFull >= 0 && listDataSkinOffer [idOfferSkin2].idSkinFull < DataManager.ins.gameSave.listSkins_Full.Length) {
                listChars [1].modelChar.idSkin_Hair = -1;
                listChars [1].modelChar.idSkin_Hand = -1;
                listChars [1].modelChar.idSkin_Body = -1;
                listChars [1].modelChar.idSkin_Full = listDataSkinOffer [idOfferSkin2].idSkinFull;
                listChars [1].modelChar.WearSkinByIDCur();
            }
            idRandom = Random.Range(0, listIDOfferSkin.Count);
            idOfferSkin3 = listIDOfferSkin [idRandom];
            if (listDataSkinOffer [idOfferSkin3].idSkinFull >= 0 && listDataSkinOffer [idOfferSkin3].idSkinFull < DataManager.ins.gameSave.listSkins_Full.Length) {
                listChars [2].modelChar.idSkin_Hair = -1;
                listChars [2].modelChar.idSkin_Hand = -1;
                listChars [2].modelChar.idSkin_Body = -1;
                listChars [2].modelChar.idSkin_Full = listDataSkinOffer [idOfferSkin3].idSkinFull;
                listChars [2].modelChar.WearSkinByIDCur();
            }
            FirebaseManager.ins.ads_reward_offer("UnlockSkin_Home", "");
        } else {
            obj_BtnRight.SetActive(false);
            obj_BtnLeft.SetActive(false);
            for (int i = 1; i < listChars.Length; i++) {
                listChars [i].gameObject.SetActive(false);
            }
        }
        modelChar.WearSkinByGameSave();

        if (PluginManager.ins.TryFree_FormHome) {
            txt_UnlockChar2.text = "Try Free";
            txt_UnlockChar3.text = "Try Free";
        } else {
            txt_UnlockChar2.text = "Get It";
            txt_UnlockChar3.text = "Get It";
        }


        if (DataManager.ins.gameSave.isRandomMinigameNext) {
            obj_Logo.SetActive(true);
            img_MinigameCur.gameObject.SetActive(false);
        } else {
            obj_Logo.SetActive(false);
            img_MinigameCur.gameObject.SetActive(true);
            img_MinigameCur.sprite = GameManager.ins.arrayDatasMinigame [(int)GameManager.ins.listMinigamesRandom [DataManager.ins.gameSave.idRandomMinigame]].imgThumbnail;
            txt_NameMinigameCur.text = GameManager.ins.arrayDatasMinigame [(int)GameManager.ins.listMinigamesRandom [DataManager.ins.gameSave.idRandomMinigame]].nameDisplay;
        }
        //Check thời gian NoAds
        if (DataManager.ins.gameSave.dayNoAds == (int)System.DateTime.Now.Subtract(new System.DateTime(1970, 1, 1)).TotalDays) 
            StartCoroutine(CountdownNoAds());

        if (DataManager.ins.gameSave.showSpinWinAll || (GameManager.ins.isFirstOpen && DataManager.ins.gameSave.totalSession > 0 )){
            SceneManager.ins.ShowPopup_Spin();
        } else if (DataManager.ins.gameSave.levelEnded >= PluginManager.ins.Level_PopupTurnOnInternet && Application.internetReachability == NetworkReachability.NotReachable && !Application.isEditor) {//Nếu ko có Internet sau level 5 thì ko cho chơi tiếp
            SceneManager.ins.ShowPopupTurnOnInternet();
        }
        GameManager.ins.isFirstOpen = false;
        Timer.Schedule(this, 0.5f, () => {
            MaxManager.ins.ReloadBanner();
        });
    }

    public IEnumerator CountdownNoAds() {
        int secondNoAdsRemain = DataManager.ins.gameSave.secondNoAds + Constants.SECOND_REMOVEADS - (int)System.DateTime.Now.Subtract(new System.DateTime(1970, 1, 1).AddDays(DataManager.ins.gameSave.dayNoAds)).TotalSeconds;
        if (secondNoAdsRemain >= 0) {
            btn_NoAds.interactable = false;
            txt_NoAds.text = string.Format("{0:00} : {1:00}", secondNoAdsRemain / 60, secondNoAdsRemain % 60);
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(CountdownNoAds());
        } else {
            btn_NoAds.interactable = true;
            txt_NoAds.text = "No Ads";
        }
    }

    #region Button
    public void ClickToNextSkin(bool isNext = false) {
        float timeEffect = 0.45f;
        tran_RotateChars.DOLocalRotate(isNext ? tran_RotateChars.localEulerAngles + Vector3.up*120 : tran_RotateChars.localEulerAngles + Vector3.down*120, timeEffect);
        SceneManager.ins.BlockInput(timeEffect);
        idCharSelect += isNext ? 1 : -1;
        if (idCharSelect < 0) idCharSelect = listChars.Length - 1;
        if (idCharSelect > listChars.Length - 1) idCharSelect = 0;
        Timer.Schedule(this, timeEffect, () => {
            CheckSelected();
        });
    }

    private void CheckSelected() {
        if (idCharSelect == 1) {
            if (isSkin2_Unlock) {
                DataManager.ins.gameSave.idSkin_Hair = listChars [idCharSelect].modelChar.idSkin_Hair;
                DataManager.ins.gameSave.idSkin_Hand = listChars [idCharSelect].modelChar.idSkin_Hand;
                DataManager.ins.gameSave.idSkin_Body = listChars [idCharSelect].modelChar.idSkin_Body;
                DataManager.ins.gameSave.idSkin_Full = listChars [idCharSelect].modelChar.idSkin_Full;
            }
        } else if (idCharSelect == 2) {
            if (isSkin3_Unlock) {
                DataManager.ins.gameSave.idSkin_Hair = listChars [idCharSelect].modelChar.idSkin_Hair;
                DataManager.ins.gameSave.idSkin_Hand = listChars [idCharSelect].modelChar.idSkin_Hand;
                DataManager.ins.gameSave.idSkin_Body = listChars [idCharSelect].modelChar.idSkin_Body;
                DataManager.ins.gameSave.idSkin_Full = listChars [idCharSelect].modelChar.idSkin_Full;
            }
        } else {
            DataManager.ins.gameSave.idSkin_Hair = listChars [idCharSelect].modelChar.idSkin_Hair;
            DataManager.ins.gameSave.idSkin_Hand = listChars [idCharSelect].modelChar.idSkin_Hand;
            DataManager.ins.gameSave.idSkin_Body = listChars [idCharSelect].modelChar.idSkin_Body;
            DataManager.ins.gameSave.idSkin_Full = listChars [idCharSelect].modelChar.idSkin_Full;
        }
    }

    public void BackToSkinCur() {
        if (idCharSelect == 1 && isSkin2_Unlock) {
            tran_RotateChars.localEulerAngles = Vector3.up * 120;
        } else if(idCharSelect == 2 && isSkin3_Unlock) {
            tran_RotateChars.localEulerAngles = Vector3.down * 120;
        } else {
            tran_RotateChars.localEulerAngles = Vector3.zero;
            idCharSelect = 0;
        }
        listChars[idCharSelect].modelChar.WearSkinByGameSave();
    }

    public void Btn_UnlockSkinByIDChar(int id) {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowRewardedAd("OfferSkin_Home", "SkinHome_" +  (id == 1 ? DataManager.ins.gameSave.listSkins_Full [listChars [1].modelChar.idSkin_Full].keyID : DataManager.ins.gameSave.listSkins_Full [listChars [2].modelChar.idSkin_Full].keyID), () => {
            if (listChars [id].modelChar.idSkin_Full >=0 && listChars [id].modelChar.idSkin_Full< DataManager.ins.gameSave.listSkins_Full.Length) {
                if (PluginManager.ins.TryFree_FormHome) {
                    DataManager.ins.gameSave.listSkins_Full[listChars[id].modelChar.idSkin_Full].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Full[listChars[id].modelChar.idSkin_Full].isBuy = false;
                    DataManager.ins.gameSave.listSkins_Full[listChars[id].modelChar.idSkin_Full].play1Game_Unlock1Time = true;
                } else {
                    DataManager.ins.gameSave.listSkins_Full[listChars[id].modelChar.idSkin_Full].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Full[listChars[id].modelChar.idSkin_Full].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Full[listChars[id].modelChar.idSkin_Full].play1Game_Unlock1Time = false;
                }
            }
            listChars [id].obj_BtnUnlockSkin.SetActive(false);
            listChars [id].effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [listChars [id].modelChar.idColor];
            listChars [id].effect_ChangeSkin.Play();

            if (id == 1) {
                isSkin2_Unlock = true;
                if (idCharSelect == 0) {
                    ClickToNextSkin(true);
                } else if(idCharSelect == 2) {
                    ClickToNextSkin(false);
                } else {
                    CheckSelected();
                }
                idCharSelect = 1;
            }
            if (id == 2) {
                isSkin3_Unlock = true;
                if (idCharSelect == 0) {
                    ClickToNextSkin(false);
                } else if (idCharSelect == 1) {
                    ClickToNextSkin(true);
                } else {
                    CheckSelected();
                }
                idCharSelect = 2;
            }
        }, null);
    }

    public void BtnClaimGold() {
        SoundManager.ins.sound_Click.PlaySound();
        SceneManager.ins.BlockInput(1);
        MaxManager.ins.ShowRewardedAd("Gold_Home", "Gold_Home", () => {
            goldOld = DataManager.ins.gameSave.gold;
            DataManager.ins.ChangeGold((int)PluginManager.ins.GoldOffer_Shop, "Gold_Offer_Shop", true, false);
            SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
            });
        });
    }


    public void Btn_Setting() {
        SoundManager.ins.sound_Click.PlaySound();
        SceneManager.ins?.ShowPopup_Settings();
    }
    public void Btn_Skin_Outfit()
    {
        progressBar.gameObject.SetActive(false);
        obj_BtnSetting.SetActive(false);
        obj_BtnSkin.SetActive(false);
        btn_play.gameObject.SetActive(false);
        SoundManager.ins.sound_Click.PlaySound();
        SceneManager.ins.ShowPopup_Skin();
        SceneManager.ins.popup_Skin.OnClosed = () => {
            progressBar.gameObject.SetActive(true);
            obj_BtnSetting.SetActive(true);
            obj_BtnSkin.SetActive(true);
            btn_play.gameObject.SetActive(true);
        };
    }

    public void Btn_NoAds() {
        SoundManager.ins.sound_Click.PlaySound();
        SceneManager.ins?.ShowPopup_NoAds();
    }
    public void EndEditName()
    {
        DataManager.ins.gameSave.username = name_InputField.text;
    }
    public void Btn_Play() {
        SoundManager.ins.sound_Click.PlaySound();
        SceneManager.ins.ChangeForm_Gameplay(0);
    }
    public void Btn_OpenSceneNangCup()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene_NangCup");
    }
    #endregion
}

[System.Serializable]
public class DataSkin_OfferHome {
    public int idSkinFull;
}