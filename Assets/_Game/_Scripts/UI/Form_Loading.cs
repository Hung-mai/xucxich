using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Form_Loading : FormBase {
    private float speedLoading = 0.33f;
    public float fillAmountProcess = 0;
    public TextMeshProUGUI txt_Version;
    public int idSkinFull_1;
    public int idSkinFull_2;
    [Header("--------------- RemoteConfig True --------")]
    public int idSkinBody_1;
    public int idSkinBody_2;
    public Image img_ProccessLoading;
    public GameObject obj_ProccessLoading;
    public DOTweenAnimation twn_Logo;

    public GameObject obj_IconAds_Char1;
    public GameObject obj_IconAds_Char2;
    public GameObject obj_LoadingAds_Char1;
    public GameObject obj_LoadingAds_Char2;
    public GameObject obj_BtnAds_Char1;
    public GameObject obj_BtnAds_Char2;

    [Header("--------------- Select Skin --------")]
    public int idCharSelect = 0;
    public bool isSkin2Unlock = false;
    public bool isSkin3Unlock = false;
    public GameObject obj_Char1;
    public GameObject obj_Char2;
    public GameObject obj_Char3;
    public GameObject obj_SelectChar1;
    public GameObject obj_SelectChar2;
    public GameObject obj_SelectChar3;

    public ModelChar [] listModelChar;
    public DOTweenAnimation twn_SelectCharacter;
    private void Awake() {
        fillAmountProcess =0;
        img_ProccessLoading.fillAmount = fillAmountProcess;
        txt_Version.text ="v" + Application.version;
    }

    public override void Show() {
        base.Show();
        if (DataManager.ins.gameSave.totalSession == 0) {
            twn_Logo.DOPlayForward();
            twn_SelectCharacter.DOPlayForward();
            obj_ProccessLoading.SetActive(false);
            obj_IconAds_Char1.SetActive(false);
            obj_IconAds_Char2.SetActive(false);
            obj_LoadingAds_Char1.SetActive(true);
            obj_LoadingAds_Char2.SetActive(true);
           
                listModelChar[1].idSkin_Full = idSkinFull_1;
                listModelChar[2].idSkin_Full = idSkinFull_2;
                FirebaseManager.ins.ads_reward_offer("UnlockSkinFull_FirstOpen", "");
            
            for (int i = 0; i < listModelChar.Length; i++) {
                listModelChar [i].WearSkinByIDCur();
            }
            StartCoroutine(WaitVideoAds());
            Timer.Schedule(this, 0.8f, () => {
                obj_Char1.SetActive(true);
                obj_Char2.SetActive(true);
                obj_Char3.SetActive(true);
            });
        } else {
            GameManager.ins.isSelectedChar = true;
        }
    }



    private void Update() {
        if (GameManager.ins.isFormLoading_Proccess) {
            // if(img_ProccessLoading.rectTransform.sizeDelta.x < 773) img_ProccessLoading.rectTransform.sizeDelta = new Vector2(img_ProccessLoading.rectTransform.sizeDelta.x + Time.deltaTime/5f *773, img_ProccessLoading.rectTransform.sizeDelta.y);
            fillAmountProcess += speedLoading * Time.deltaTime;
            img_ProccessLoading.fillAmount = fillAmountProcess;
        }
    }

    IEnumerator WaitVideoAds() {
        yield return new WaitUntil(() => (MaxManager.ins.isRewardedVideoAvailable()));
        obj_IconAds_Char1.SetActive(true);
        obj_IconAds_Char2.SetActive(true);
        obj_LoadingAds_Char1.SetActive(false);
        obj_LoadingAds_Char2.SetActive(false);
    }

    public void Btn_ClickToChar(int idChar) {
        SoundManager.ins.sound_Click.PlaySound();
        if (idChar == 0) {
            idCharSelect = 0;
            obj_SelectChar1.SetActive(true);
            obj_SelectChar2.SetActive(false);
            obj_SelectChar3.SetActive(false);
        } else if (idChar == 1) {
            if (isSkin2Unlock) {//Skin này đã Unlock lần sau chọn vào ko hiện VideoAds nữa
                idCharSelect = 1;
                obj_SelectChar1.SetActive(false);
                obj_SelectChar2.SetActive(true);
                obj_SelectChar3.SetActive(false);
            } else {//Nếu chưa unlock thì xem Video Ads nếu xem fail thì vẫn chọn nhưng khi bấm Select sẽ phải xem VideoAds
                
                    MaxManager.ins.ShowRewardedAd("SelectCharacter_FirstOpen", "Loading_Skin2", () => {
                        Debug.LogWarning("Success SelectCharacter_FirstOpen Skin 2");
                        DataManager.ins.gameSave.listSkins_Full[idSkinFull_1].isBuy = true;
                        DataManager.ins.gameSave.listSkins_Full[idSkinFull_1].isUnlock = true;
                        DataManager.ins.gameSave.listSkins_Full[idSkinFull_1].play1Game_Unlock1Time = false;
                        obj_BtnAds_Char1.SetActive(false);
                        isSkin2Unlock = true;
                        idCharSelect = 1;
                        obj_SelectChar1.SetActive(false);
                        obj_SelectChar2.SetActive(true);
                        obj_SelectChar3.SetActive(false);
                    }, () => {
                        Debug.LogWarning("Fail SelectCharacter_FirstOpen Skin 2");
                        if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false)
                            SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
                    });
                
            }
        } else if (idChar == 2) {
            if (isSkin3Unlock) {//Skin này đã Unlock lần sau chọn vào ko hiện VideoAds nữa
                idCharSelect = 2;
                obj_SelectChar1.SetActive(false);
                obj_SelectChar2.SetActive(false);
                obj_SelectChar3.SetActive(true);
            } else {//Nếu chưa unlock thì xem Video Ads nếu xem fail thì vẫn chọn nhưng khi bấm Select sẽ phải xem VideoAds
               
                    MaxManager.ins.ShowRewardedAd("SelectCharacter_FirstOpen", "Loading_Skin3", () => {
                        Debug.LogWarning("Success SelectCharacter_FirstOpen Skin 3");
                        DataManager.ins.gameSave.listSkins_Full[idSkinFull_2].isBuy = true;
                        DataManager.ins.gameSave.listSkins_Full[idSkinFull_2].isUnlock = true;
                        DataManager.ins.gameSave.listSkins_Full[idSkinFull_2].play1Game_Unlock1Time = false;
                        obj_BtnAds_Char2.SetActive(false);
                        isSkin3Unlock = true;
                        idCharSelect = 2;
                        obj_SelectChar1.SetActive(false);
                        obj_SelectChar2.SetActive(false);
                        obj_SelectChar3.SetActive(true);
                    }, () => {
                        Debug.LogWarning("Fail SelectCharacter_FirstOpen Skin 3");
                        if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false)
                            SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
                    });
                
            }
        }

    }

    private void DoneSelectedChar() {
        obj_Char1.SetActive(false);
        obj_Char2.SetActive(false);
        obj_Char3.SetActive(false);
        GameManager.ins.isSelectedChar = true;
    }

    public void Btn_Play() {
        SoundManager.ins.sound_Click.PlaySound();
        //Save skin đó lại
        if (idCharSelect == 0) {
            DataManager.ins.gameSave.idSkin_Full = -1;
            DoneSelectedChar();
        } else if (idCharSelect == 1) {
            if (isSkin2Unlock) {//Skin này đã Unlock lần sau chọn vào ko hiện VideoAds nữa
                DataManager.ins.gameSave.idSkin_Full = idSkinFull_1;
                DataManager.ins.gameSave.idSkin_Body = -1;
                DoneSelectedChar();
            } else {//Nếu chưa unlock thì xem Video Ads nếu xem fail thì vẫn chọn nhưng khi bấm Select sẽ phải xem VideoAds
                MaxManager.ins.ShowRewardedAd("SelectCharacter_FirstOpen", "Loading_Skin2", () => {
                    Debug.LogWarning("Success SelectCharacter_FirstOpen Skin 2");
                    DataManager.ins.gameSave.idSkin_Full = idSkinFull_1;
                    DataManager.ins.gameSave.idSkin_Body = -1;
                    DataManager.ins.gameSave.listSkins_Full [idSkinFull_1].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Full [idSkinFull_1].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Full [idSkinFull_1].play1Game_Unlock1Time = false;
                    obj_BtnAds_Char1.SetActive(false);
                    isSkin2Unlock = true;
                    DoneSelectedChar();
                }, () => {
                    Debug.LogWarning("Fail SelectCharacter_FirstOpen Skin 2");
                    if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false)
                        SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
                });
            }
        } else if (idCharSelect == 2) {
            if (isSkin3Unlock) {//Skin này đã Unlock lần sau chọn vào ko hiện VideoAds nữa
                DataManager.ins.gameSave.idSkin_Full = idSkinFull_2;
                DataManager.ins.gameSave.idSkin_Body = -1;
                DoneSelectedChar();
            } else {//Nếu chưa unlock thì xem Video Ads nếu xem fail thì vẫn chọn nhưng khi bấm Select sẽ phải xem VideoAds
                MaxManager.ins.ShowRewardedAd("SelectCharacter_FirstOpen", "Loading_Skin3", () => {
                    Debug.LogWarning("Success SelectCharacter_FirstOpen Skin 3");
                    DataManager.ins.gameSave.idSkin_Full = idSkinFull_2;
                    DataManager.ins.gameSave.idSkin_Body = -1;
                    DataManager.ins.gameSave.listSkins_Full [idSkinFull_2].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Full [idSkinFull_2].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Full [idSkinFull_2].play1Game_Unlock1Time = false;
                    obj_BtnAds_Char2.SetActive(false);
                    isSkin3Unlock = true;
                    DoneSelectedChar();
                }, () => {
                    Debug.LogWarning("Fail SelectCharacter_FirstOpen Skin 3");
                    if (SceneManager.ins.obj_RewardAdsNotAvailable != null && SceneManager.ins.obj_RewardAdsNotAvailable.activeSelf == false)
                        SceneManager.ins.obj_RewardAdsNotAvailable.SetActive(true);
                });
            }
        }
    }
}