using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.PickerWheelUI;
using TMPro;
using System;
using System.Globalization;
using DG.Tweening;
using System.Collections.Generic;

public class Popup_Spin : PopupBase
{
    public bool isWinAll = false;

    [Header("---------- list id reward -----------")]
    public List<int> listHairID     = new List<int>() { 0, 3, 22, 17, 2};
    public List<int> listWeaponID   = new List<int>() { 2, 4, 6, 15, 12};
    public List<int> listBodyID     = new List<int>() { 20 ,12, 19, 18, 16};
    public List<int> listSKinFullID = new List<int>() { 9, 19, 16, 11, 15 };

    private List<int> listHairIDToOffer = new List<int>();
    private List<int> listWeaponIDToOffer = new List<int>();
    private List<int> listBodyIDToOffer = new List<int>();
    private List<int> listSKinFullIDToOffer = new List<int>();

    public ItemSpin_Type rewardType;

    [Header("---------- objects -----------")]
    public GameObject objBtnBefore_Spin;
    public GameObject objBtnAfter_Spin;

    public GameObject btnClose_Before;
    public GameObject btnLoseIt;
   
    public TextMeshProUGUI txt_ClaimByAds;
    public Sprite sprite_Money;
    //public Text popupReward_MoneyTxt;
    //public Image imageReward;

    public PickerWheel pickerWheel;
    public WheelPiece wheelPiece;


    private int goldOld;

    public override void Show()
	{
        base.Show();
        if (SceneManager.ins.obj_Gold != null) {
            SceneManager.ins.obj_Gold.SetActive(false);
        }
        if (SceneManager.ins.obj_Cup != null) {
            SceneManager.ins.obj_Cup.SetActive(false);
        }
        OnClosed = () => {
            if (SceneManager.ins.obj_Gold != null) {
                SceneManager.ins.obj_Gold.SetActive(true);
            }
            if (SceneManager.ins.obj_Cup != null) {
                SceneManager.ins.obj_Cup.SetActive(true);
            }
        };
        btnLoseIt.SetActive(false);
        objBtnBefore_Spin.SetActive(true);
        objBtnAfter_Spin.SetActive(false);
        if (DataManager.ins.gameSave.showSpinWinAll) {
            DataManager.ins.gameSave.showSpinWinAll = false;
            btnClose_Before.SetActive(false);
        } else {
            btnClose_Before.SetActive(true);
        }

        // TODO: thiết lập chỗ này
        goldOld = DataManager.ins.gameSave.gold;
        SetupPieces();
        if (DataManager.ins.gameSave.showFistOpenSpin) {
            for (int i = 0; i < pickerWheel.wheelPieces.Length; i++) {
                pickerWheel.wheelPieces [i].Chance = 0;
            }
            pickerWheel.wheelPieces [7].Chance = 100;
        }

        // sau khi thiết lập ảnh và tỉ lệ thì ms generate ra spin
        pickerWheel.StartGenerate();
    }

    private void SetupPieces()
    {

        #region hair
        //Tìm xem còn hairr để Offer ko?
        listHairIDToOffer= new List<int>();
        for (int i = 0; i < listHairID.Count; i++) {
            if (listHairID [i] >= 0 && listHairID [i] < DataManager.ins.gameSave.listSkins_Hair.Length) {
                if (!DataManager.ins.gameSave.listSkins_Hair [listHairID [i]].isBuy  && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Hair [listHairID [i]].HideOnIOS)) {
                    listHairIDToOffer.Add(listHairID [i]);
                }
            }
        }
        //Nếu các SkinHand cố định đã Offer hết thì Offer những cái chưa mua còn lại
        if (listHairIDToOffer.Count <= 0) {
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Hair.Length; i++) {
                if (!DataManager.ins.gameSave.listSkins_Hair [i].isBuy && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Hair [i].HideOnIOS)) {
                    listHairIDToOffer.Add(i);
                }
            }
        }

        if (listHairIDToOffer.Count > 0) {
            pickerWheel.wheelPieces[1].Icon = GameManager.ins.arrayDataSkin_Hair [listHairIDToOffer [0]].icon;
            pickerWheel.wheelPieces[1].id = listHairIDToOffer [0];
        } else {
            pickerWheel.wheelPieces[1].Icon = sprite_Money;
        }

        #endregion

        #region hand
        //Tìm xem còn SkinHand để Offer ko?
        listWeaponIDToOffer= new List<int>();
        for (int i = 0; i < listWeaponID.Count; i++) {
            if (listWeaponID [i] >= 0 && listWeaponID [i] < DataManager.ins.gameSave.listSkins_Hand.Length) {
                if (!DataManager.ins.gameSave.listSkins_Hand [listWeaponID [i]].isBuy  && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Hand [listWeaponID [i]].HideOnIOS)) {
                    listWeaponIDToOffer.Add(listWeaponID [i]);
                }
            }
        }
        //Nếu các SkinHand cố định đã Offer hết thì Offer những cái chưa mua còn lại
        if (listWeaponIDToOffer.Count <= 0) {
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Hand.Length; i++) {
                if (!DataManager.ins.gameSave.listSkins_Hand [i].isBuy && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Hand [i].HideOnIOS)) {
                    listWeaponIDToOffer.Add(i);
                }
            }
        }

        if (listWeaponIDToOffer.Count > 0) {
            pickerWheel.wheelPieces[3].Icon = GameManager.ins.arrayDataSkin_Hand [listWeaponIDToOffer [0]].icon;
            pickerWheel.wheelPieces[3].id = listWeaponIDToOffer [0];
        } else {
            pickerWheel.wheelPieces[3].Icon = sprite_Money;
        }
        #endregion

        #region body
        //Tìm xem còn SkinHand để Offer ko?
        listBodyIDToOffer= new List<int>();
        for (int i = 0; i < listBodyID.Count; i++) {
            if (listBodyID [i] >= 0 && listBodyID [i] < DataManager.ins.gameSave.listSkins_Body.Length) {
                if (!DataManager.ins.gameSave.listSkins_Body [listBodyID [i]].isBuy  && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Body [listBodyID [i]].HideOnIOS)) {
                    listBodyIDToOffer.Add(listBodyID [i]);
                }
            }
        }
        //Nếu các SkinblistSkins_Body cố định đã Offer hết thì Offer những cái chưa mua còn lại
        if (listBodyIDToOffer.Count <= 0) {
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Body.Length; i++) {
                if (!DataManager.ins.gameSave.listSkins_Body [i].isBuy && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Body [i].HideOnIOS)) {
                    listBodyIDToOffer.Add(i);
                }
            }
        }

        if (listBodyIDToOffer.Count > 0) {
            pickerWheel.wheelPieces[5].Icon = GameManager.ins.arrayDataSkin_Body [listBodyIDToOffer [0]].icon;
            pickerWheel.wheelPieces[5].id = listBodyIDToOffer [0];
        } else {
            pickerWheel.wheelPieces[5].Icon = sprite_Money;
        }
        #endregion

        #region full skin
        //Tìm xem còn SkinHand để Offer ko?
        listSKinFullIDToOffer= new List<int>();
        for (int i = 0; i < listSKinFullID.Count; i++) {
            if (listSKinFullID [i] >= 0 && listSKinFullID [i] < DataManager.ins.gameSave.listSkins_Full.Length) {
                if (!DataManager.ins.gameSave.listSkins_Full [listSKinFullID [i]].isBuy  && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Full [listSKinFullID [i]].HideOnIOS)) {
                    listSKinFullIDToOffer.Add(listSKinFullID [i]);
                }
            }
        }
        //Nếu các SkinFull cố định đã Offer hết thì Offer những cái chưa mua còn lại
        if (listSKinFullIDToOffer.Count <= 0) {
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Full.Length; i++) {
                if (!DataManager.ins.gameSave.listSkins_Full [i].isBuy && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Full [i].HideOnIOS)) {
                    listSKinFullIDToOffer.Add(i);
                }
            }
        }

        if (listSKinFullIDToOffer.Count > 0) {
            pickerWheel.wheelPieces[7].Icon = GameManager.ins.arrayDataSkin_Full [listSKinFullIDToOffer [0]].icon;
            pickerWheel.wheelPieces[7].id = listSKinFullIDToOffer [0];
        } else {
            pickerWheel.wheelPieces[7].Icon = sprite_Money;
        }
    #endregion
    }
    
    public void Btn_Spin() {
        SoundManager.ins.sound_Click.PlaySound();
        DataManager.ins.gameSave.showFistOpenSpin = false;
        SceneManager.ins.BlockInput(true);
        objBtnBefore_Spin.SetActive(false);
        objBtnAfter_Spin.SetActive(false);
        pickerWheel.OnSpinEnd(wheelPieceSpin => {
            Debug.LogWarning("Phần thưởng Spin:" + wheelPieceSpin.itemType.ToString());
            wheelPiece = wheelPieceSpin;
            objBtnBefore_Spin.SetActive(false);
            objBtnAfter_Spin.SetActive(true);
            if (wheelPiece.itemType == ItemSpin_Type.Money) {
                txt_ClaimByAds.text = "Claim " +wheelPiece.Amount +"$";
            } else {
                txt_ClaimByAds.text = "Claim Skin";
            }
            SceneManager.ins.BlockInput(false);
            Timer.Schedule(this, 1.5f, () => {
                btnLoseIt.SetActive(true);
            });
        });
        pickerWheel.Spin();
    }

    public void Btn_ClaimByAds() {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowRewardedAd("Spin_ads", "Spin_ads", () => {
            switch (wheelPiece.itemType) {
                case ItemSpin_Type.None:
                    Debug.LogError("Lỗi chưa setup Item spin");
                    break;
                case ItemSpin_Type.Money:
                    DataManager.ins.ChangeGold(wheelPiece.Amount, "Spin", true, false);
                    SceneManager.ins.BlockInput(true);
                    SceneManager.ins.effectGem.ShowEffect(wheelPiece.Amount, Camera.main.WorldToScreenPoint(txt_ClaimByAds.transform.position), SceneManager.ins.iconGold.transform,
                            () => {
                                if (SceneManager.ins.obj_Gold != null) {
                                    SceneManager.ins.obj_Gold.SetActive(true);
                                }
                                SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
                                    SceneManager.ins.BlockInput(false);
                                    goldOld = DataManager.ins.gameSave.gold;
                                    Close();
                                    //SetupPieces();
                                    //pickerWheel.StartGenerate();
                                });
                            });
                    break;
                case ItemSpin_Type.Skin_Hair:
                    DataManager.ins.gameSave.idSkin_Full = -1;
                    DataManager.ins.gameSave.idSkin_Hair = wheelPiece.id;
                    DataManager.ins.gameSave.listSkins_Hair [wheelPiece.id].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Hair [wheelPiece.id].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Hair [wheelPiece.id].play1Game_Unlock1Time = false;
                    Close();
                    break;
                case ItemSpin_Type.Skin_Hand:
                    DataManager.ins.gameSave.idSkin_Full = -1;
                    DataManager.ins.gameSave.idSkin_Hand = wheelPiece.id;
                    DataManager.ins.gameSave.listSkins_Hand [wheelPiece.id].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Hand [wheelPiece.id].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Hand [wheelPiece.id].play1Game_Unlock1Time = false;
                    Close();
                    break;
                case ItemSpin_Type.Skin_Body:
                    DataManager.ins.gameSave.idSkin_Full = -1;
                    DataManager.ins.gameSave.idSkin_Body = wheelPiece.id;
                    DataManager.ins.gameSave.listSkins_Body [wheelPiece.id].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Body [wheelPiece.id].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Body [wheelPiece.id].play1Game_Unlock1Time = false;
                    Close();
                    break;
                case ItemSpin_Type.Skin_Full:
                    DataManager.ins.gameSave.idSkin_Full = wheelPiece.id;
                    DataManager.ins.gameSave.listSkins_Full [wheelPiece.id].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Full [wheelPiece.id].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Full [wheelPiece.id].play1Game_Unlock1Time = false;
                    Close();
                    break;
                default:
                    Debug.LogError("Lỗi loại item spin");
                    break;
            }
            if (SceneManager.ins.formHome != null) SceneManager.ins.formHome.BackToSkinCur();
            //SetupPieces();
            //pickerWheel.StartGenerate();
        });

    }

    public void Btn_Close()
    {
        SoundManager.ins.sound_Click.PlaySound();
        Timer.Schedule(this, 0.1f, () => {
            Close();
        });
    }
} 

public enum ItemSpin_Type {//Ko nên xóa mà nên thêm 
    None,
    Money,
    Skin_Hair,
    Skin_Hand,
    Skin_Body,
    Skin_Full,
}
