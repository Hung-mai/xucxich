using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Skin : PopupBase {
    private bool isInit = false;
    private bool isShowed = false;
    public int tab = 0;//0 là chọn tóc, 1, vũ khí, 2 là chọn body, 3 là full bộ

    public ModelChar modelChar;
    public ParticleSystem effect_ChangeSkin;
    public Card_Skin card_SkinBase;//Prefab
    public Sprite sprite_Unlock;
    public Sprite sprite_Lock;
    
    public Transform tran_ListCardSkins_Hair;
    public Transform tran_ListCardSkins_Hand;
    public Transform tran_ListCardSkins_Body;
    public Transform tran_ListCardSkins_Full;
  
    public GameObject obj_ListCardSkins_Hair;
    public GameObject obj_ListCardSkins_Hand;
    public GameObject obj_ListCardSkins_Body;
    public GameObject obj_ListCardSkins_Full;

    public List<Card_Skin> listSkins_Hair;
    public List<Card_Skin> listSkins_Hand;
    public List<Card_Skin> listSkins_Body;
    public List<Card_Skin> listSkins_Full;

    public ScrollRect scroll_TapHair;
    public ScrollRect scroll_TapHand;
    public ScrollRect scroll_TapBody;
    public ScrollRect scroll_TapFull;

    public GameObject obj_TapHair_On;
    public GameObject obj_TapHand_On;
    public GameObject obj_TapBody_On;
    public GameObject obj_TapFull_On;
    
    public GameObject obj_TapHair_Off;
    public GameObject obj_TapHand_Off;
    public GameObject obj_TapBody_Off;
    public GameObject obj_TapFull_Off;

    public GameObject obj_BtnSkinLock;
    public GameObject obj_BtnSkinUnlock;
    public GameObject obj_OneTime;
    public TextMeshProUGUI txt_Cost;
    public Image img_BtnEquipp;
    public Sprite sprite_Equip;
    public Sprite sprite_UnEquip;
    public Color color_Equip;
    public Color color_UnEquip;
    public TextMeshProUGUI txt_Equipp;

    private bool dragging = false;
    private float mousePos_StartX;
    private Vector3 eulerAngle;
    private Vector3 eulerAngle_ModelCharOrigin;

    private int goldOld;

    public override void Show() {
        base.Show();
        goldOld = DataManager.ins.gameSave.gold;
        tab = 0;
        isShowed = false;
        if (SceneManager.ins.formHome != null &&  SceneManager.ins.formHome.obj_BtnOfferGold != null) SceneManager.ins.formHome.obj_BtnOfferGold.SetActive(true);
        if (!isInit) { 
            eulerAngle_ModelCharOrigin = modelChar.transform.eulerAngles;
        } else {
            modelChar.transform.eulerAngles = eulerAngle_ModelCharOrigin;
        }
        if (!isInit) listSkins_Hair = new List<Card_Skin>();
        if (!isInit) listSkins_Hand = new List<Card_Skin>();
        if (!isInit) listSkins_Body = new List<Card_Skin>();
        if (!isInit) listSkins_Full = new List<Card_Skin>();
        modelChar.WearSkinByGameSave();
        //Instance các Card_Skin và Truyền thông tin vào các Card_Skin Hair
        for (int i = 0; i < GameManager.ins.arrayDataSkin_Hair.Length; i++) {
            if (!isInit) {
                Card_Skin card = Instantiate(card_SkinBase, tran_ListCardSkins_Hair);
                card.popup_Skin = this;
                listSkins_Hair.Add(card);
            }
            listSkins_Hair [i].idCard = i;
            listSkins_Hair [i].image_BG.sprite = DataManager.ins.gameSave.listSkins_Hair [i].isUnlock ? sprite_Unlock : sprite_Lock;
            listSkins_Hair [i].image_Avatar.sprite = GameManager.ins.arrayDataSkin_Hair [i].icon;
            listSkins_Hair [i].image_Avatar.color = DataManager.ins.gameSave.listSkins_Hair [i].isUnlock ?new Color(255,255,255,255) : new Color(0, 0, 0, 0.3f);
            listSkins_Hair [i].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Hair [i].isUnlock);
            listSkins_Hair [i].obj_Selecting.SetActive( modelChar.idSkin_Hair == i);
            listSkins_Hair [i].obj_Wearing.SetActive(DataManager.ins.gameSave.idSkin_Hair == i);
            listSkins_Hair [i].obj_VIP.SetActive(GameManager.ins.arrayDataSkin_Hair [i].isVip);
            if (GameManager.ins.arrayDataSkin_Hair [i].isVip) listSkins_Hair [i].transform.SetAsLastSibling();
        }

        for (int i = 0; i < GameManager.ins.arrayDataSkin_Hand.Length; i++) {
            if (!isInit) {
                Card_Skin card = Instantiate(card_SkinBase, tran_ListCardSkins_Hand);
                card.popup_Skin = this;
                listSkins_Hand.Add(card);
            }
            listSkins_Hand [i].idCard = i;
            listSkins_Hand [i].image_BG.sprite = DataManager.ins.gameSave.listSkins_Hand [i].isUnlock ? sprite_Unlock : sprite_Lock;
            listSkins_Hand [i].image_Avatar.sprite = GameManager.ins.arrayDataSkin_Hand [i].icon;
            listSkins_Hand [i].image_Avatar.color = DataManager.ins.gameSave.listSkins_Hand [i].isUnlock ? new Color(255, 255, 255, 255) : new Color(0, 0, 0, 0.3f);
            listSkins_Hand [i].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Hand [i].isUnlock);
            listSkins_Hand [i].obj_Selecting.SetActive( modelChar.idSkin_Hand == i);
            listSkins_Hand [i].obj_Wearing.SetActive(DataManager.ins.gameSave.idSkin_Hand == i);
            listSkins_Hand [i].obj_VIP.SetActive(GameManager.ins.arrayDataSkin_Hand [i].isVip);
            if (GameManager.ins.arrayDataSkin_Hand [i].isVip) listSkins_Hand [i].transform.SetAsLastSibling();
        }

        //Instance các Card_Skin và Truyền thông tin vào các Card_Skin Body
        for (int i = 0; i < GameManager.ins.arrayDataSkin_Body.Length; i++) {
            if (!isInit) {
                Card_Skin card = Instantiate(card_SkinBase, tran_ListCardSkins_Body);
                card.popup_Skin = this;
                listSkins_Body.Add(card);
            }
            listSkins_Body [i].idCard = i;
            listSkins_Body [i].image_BG.sprite = DataManager.ins.gameSave.listSkins_Body [i].isUnlock ? sprite_Unlock : sprite_Lock;
            listSkins_Body [i].image_Avatar.sprite = GameManager.ins.arrayDataSkin_Body [i].icon;
            listSkins_Body [i].image_Avatar.color = DataManager.ins.gameSave.listSkins_Body [i].isUnlock ?new Color(255,255,255,255) : new Color(0, 0, 0, 0.3f);
            listSkins_Body [i].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Body [i].isUnlock);
            listSkins_Body [i].obj_Selecting.SetActive( modelChar.idSkin_Body == i);
            listSkins_Body [i].obj_Wearing.SetActive(DataManager.ins.gameSave.idSkin_Body == i);
            listSkins_Body [i].obj_VIP.SetActive(GameManager.ins.arrayDataSkin_Body [i].isVip);
            if (GameManager.ins.arrayDataSkin_Body [i].isVip) listSkins_Body [i].transform.SetAsLastSibling();
        }

        for (int i = 0; i < GameManager.ins.arrayDataSkin_Full.Length; i++) {
            if (!isInit) {
                Card_Skin card = Instantiate(card_SkinBase, tran_ListCardSkins_Full);
                card.popup_Skin = this;
                listSkins_Full.Add(card);
            }
            listSkins_Full [i].idCard = i;
            listSkins_Full [i].image_BG.sprite = DataManager.ins.gameSave.listSkins_Full [i].isUnlock ? sprite_Unlock : sprite_Lock;
            listSkins_Full [i].image_Avatar.sprite = GameManager.ins.arrayDataSkin_Full [i].icon;
            listSkins_Full [i].image_Avatar.color = DataManager.ins.gameSave.listSkins_Full [i].isUnlock ? new Color(255, 255, 255, 255) : new Color(0, 0, 0, 0.3f);
            listSkins_Full [i].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Full [i].isUnlock);
            listSkins_Full [i].obj_Selecting.SetActive( modelChar.idSkin_Full == i);
            listSkins_Full [i].obj_Wearing.SetActive(DataManager.ins.gameSave.idSkin_Full == i);
            listSkins_Full [i].obj_VIP.SetActive(GameManager.ins.arrayDataSkin_Full [i].isVip);
            //listSkins_Full [i].gameObject.SetActive(!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Full [i].HideOnIOS);
            listSkins_Full [i].gameObject.SetActive(!GameManager.ins.arrayDataSkin_Full [i].HideOnIOS);
            if (GameManager.ins.arrayDataSkin_Full [i].isVip) listSkins_Full [i].transform.SetAsLastSibling();
        }

        if (DataManager.ins.gameSave.idSkin_Full >= 0) {
            Btn_ChangeTab(3);
        } else if (DataManager.ins.gameSave.idSkin_Hair >= 0) {
            Btn_ChangeTab(0);
        } else if (DataManager.ins.gameSave.idSkin_Hand >= 0) {
            Btn_ChangeTab(1);
        } else {
            Btn_ChangeTab(2);
        }
        //SceneManager.ins.formHome.camFollower.state = 2;
        isInit = true;
        isShowed = true;
        FirebaseManager.ins.ads_reward_offer("TryFree_Shop", "");
    }

    private void ClickToSkin_Hair(int idSkin, bool isClick = true) {
        //Tắt hiệu ứng ở Card Selecting cũ
        if( modelChar.idSkin_Hair >= 0) listSkins_Hair [ modelChar.idSkin_Hair].obj_Selecting.SetActive(false);
        //Bật hiệu ứng ở Card Selecting mới
        if (DataManager.ins.gameSave.idSkin_Hair == idSkin) {//Chọn vào Skin đang mặc
            listSkins_Hair[idSkin].obj_Wearing.SetActive(true);
            DataManager.ins.gameSave.idSkin_Full = -1;
            DataManager.ins.gameSave.idSkin_Hair = idSkin;
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else if (DataManager.ins.gameSave.listSkins_Hair [idSkin].isUnlock && isClick) {//Click vào Skin đã unlock và chưa mặc
            if (DataManager.ins.gameSave.idSkin_Hair >= 0) listSkins_Hair [DataManager.ins.gameSave.idSkin_Hair].obj_Wearing.SetActive(false);
            DataManager.ins.gameSave.idSkin_Full = -1;
            DataManager.ins.gameSave.idSkin_Hair = idSkin;
            listSkins_Hair [idSkin].obj_Wearing.SetActive(true);
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else {
            img_BtnEquipp.sprite = sprite_Equip;
            txt_Equipp.text = "Equip";
            txt_Equipp.color = color_Equip;
        }
         modelChar.idSkin_Hair = idSkin;
        modelChar.idSkin_Full= -1;
        modelChar.idSkin_Hair = idSkin;
        listSkins_Hair [ modelChar.idSkin_Hair].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].isUnlock);
        listSkins_Hair [ modelChar.idSkin_Hair].obj_Selecting.SetActive(true);
        listSkins_Hair [ modelChar.idSkin_Hair].image_BG.sprite = DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].isUnlock ? sprite_Unlock : sprite_Lock;
        listSkins_Hair [ modelChar.idSkin_Hair].image_Avatar.color = DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].isUnlock ?new Color(255,255,255,255) : new Color(0, 0, 0, 0.3f);
        obj_BtnSkinLock.SetActive(!DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].isUnlock && !GameManager.ins.arrayDataSkin_Hair [ modelChar.idSkin_Hair].isVip);
        obj_BtnSkinUnlock.SetActive(DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].isUnlock);
        obj_OneTime.SetActive(DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].play1Game_Unlock1Time);
        txt_Cost.text = "" + GameManager.ins.arrayDataSkin_Hair [ modelChar.idSkin_Hair].cost;

        //Mặc thử skin
        modelChar.WearSkinByIDCur();
    }

    private void ClickToSkin_Hand(int idSkin, bool isClick = true) {
        //Tắt hiệu ứng ở Card Selecting cũ
        if ( modelChar.idSkin_Hand >= 0) listSkins_Hand [ modelChar.idSkin_Hand].obj_Selecting.SetActive(false);
        //Bật hiệu ứng ở Card Selecting mới
        if (DataManager.ins.gameSave.idSkin_Hand == idSkin) {//Chọn vào Skin đang mặc
            listSkins_Hand[idSkin].obj_Wearing.SetActive(true);
            DataManager.ins.gameSave.idSkin_Full = -1;
            DataManager.ins.gameSave.idSkin_Hand = idSkin;
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else if (DataManager.ins.gameSave.listSkins_Hand[idSkin].isUnlock && isClick) {//Click vào Skin đã unlock và chưa mặc
            if (DataManager.ins.gameSave.idSkin_Hand >= 0) listSkins_Hand[DataManager.ins.gameSave.idSkin_Hand].obj_Wearing.SetActive(false);
            DataManager.ins.gameSave.idSkin_Full = -1;
            DataManager.ins.gameSave.idSkin_Hand = idSkin;
            listSkins_Hand [idSkin].obj_Wearing.SetActive(true);
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else {
            img_BtnEquipp.sprite = sprite_Equip;
            txt_Equipp.text = "Equip";
            txt_Equipp.color = color_Equip;
        }
         modelChar.idSkin_Hand = idSkin;
        modelChar.idSkin_Full= -1;
        modelChar.idSkin_Hand = idSkin;
        listSkins_Hand [ modelChar.idSkin_Hand].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].isUnlock);
        listSkins_Hand [ modelChar.idSkin_Hand].obj_Selecting.SetActive(true);
        listSkins_Hand [ modelChar.idSkin_Hand].image_BG.sprite = DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].isUnlock ? sprite_Unlock : sprite_Lock;
        listSkins_Hand [ modelChar.idSkin_Hand].image_Avatar.color = DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].isUnlock ? new Color(255, 255, 255, 255) : new Color(0, 0, 0, 0.3f);
        obj_BtnSkinLock.SetActive(!DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].isUnlock && !GameManager.ins.arrayDataSkin_Hand [ modelChar.idSkin_Hand].isVip);
        obj_BtnSkinUnlock.SetActive(DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].isUnlock);
        obj_OneTime.SetActive(DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].play1Game_Unlock1Time);
        txt_Cost.text = "" + GameManager.ins.arrayDataSkin_Hand [ modelChar.idSkin_Hand].cost;

        //Mặc thử skin
        modelChar.WearSkinByIDCur();
    }

    private void ClickToSkin_Body(int idSkin, bool isClick = true) {
        //Tắt hiệu ứng ở Card Selecting cũ
        if( modelChar.idSkin_Body >= 0) listSkins_Body [ modelChar.idSkin_Body].obj_Selecting.SetActive(false);
        //Bật hiệu ứng ở Card Selecting mới
        if (DataManager.ins.gameSave.idSkin_Body == idSkin) {//Chọn vào Skin đang mặc
            listSkins_Body[idSkin].obj_Wearing.SetActive(true);
            DataManager.ins.gameSave.idSkin_Full = -1;
            DataManager.ins.gameSave.idSkin_Body = idSkin;
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else if (DataManager.ins.gameSave.listSkins_Body[idSkin].isUnlock && isClick) {//Click vào Skin đã unlock và chưa mặc
            if (DataManager.ins.gameSave.idSkin_Body >= 0) listSkins_Body[DataManager.ins.gameSave.idSkin_Body].obj_Wearing.SetActive(false);
            DataManager.ins.gameSave.idSkin_Full = -1;
            DataManager.ins.gameSave.idSkin_Body = idSkin;
            listSkins_Body [idSkin].obj_Wearing.SetActive(true);
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else {
            img_BtnEquipp.sprite = sprite_Equip;
            txt_Equipp.text = "Equip";
            txt_Equipp.color = color_Equip;
        }
         modelChar.idSkin_Body = idSkin;
        modelChar.idSkin_Full= -1;
        modelChar.idSkin_Body = idSkin;
        listSkins_Body [ modelChar.idSkin_Body].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].isUnlock);
        listSkins_Body [ modelChar.idSkin_Body].obj_Selecting.SetActive(true);
        listSkins_Body [ modelChar.idSkin_Body].image_BG.sprite = DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].isUnlock ? sprite_Unlock : sprite_Lock;
        listSkins_Body [ modelChar.idSkin_Body].image_Avatar.color = DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].isUnlock ?new Color(255,255,255,255) : new Color(0, 0, 0, 0.3f);
        obj_BtnSkinLock.SetActive(!DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].isUnlock && !GameManager.ins.arrayDataSkin_Body [ modelChar.idSkin_Body].isVip);
        obj_BtnSkinUnlock.SetActive(modelChar.idSkin_Body != 0 && DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].isUnlock);
        obj_OneTime.SetActive(DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].play1Game_Unlock1Time);
        txt_Cost.text = "" + GameManager.ins.arrayDataSkin_Body [ modelChar.idSkin_Body].cost;

        //Mặc thử skin
        modelChar.WearSkinByIDCur();
    }

    private void ClickToSkin_Full(int idSkin, bool isClick = true) {
        //Tắt hiệu ứng ở Card Selecting cũ
        if ( modelChar.idSkin_Full >= 0) listSkins_Full [ modelChar.idSkin_Full].obj_Selecting.SetActive(false);
        //Bật hiệu ứng ở Card Selecting mới
        if (DataManager.ins.gameSave.idSkin_Full == idSkin) {//Chọn vào Skin đang mặc
            DataManager.ins.gameSave.idSkin_Hair = -1;
            DataManager.ins.gameSave.idSkin_Hand = -1;
            DataManager.ins.gameSave.idSkin_Body = -1;
            DataManager.ins.gameSave.idSkin_Full = idSkin;
            listSkins_Full [idSkin].obj_Wearing.SetActive(true);
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else if (DataManager.ins.gameSave.listSkins_Full[idSkin].isUnlock && isClick) {//Click vào Skin đã unlock và chưa mặc
            if (DataManager.ins.gameSave.idSkin_Full >= 0) listSkins_Full[DataManager.ins.gameSave.idSkin_Full].obj_Wearing.SetActive(false);
            DataManager.ins.gameSave.idSkin_Hair = -1;
            DataManager.ins.gameSave.idSkin_Hand= -1;
            DataManager.ins.gameSave.idSkin_Body = -1;
            DataManager.ins.gameSave.idSkin_Full = idSkin;
            listSkins_Full [idSkin].obj_Wearing.SetActive(true);
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else {
            img_BtnEquipp.sprite = sprite_Equip;
            txt_Equipp.text = "Equip";
            txt_Equipp.color = color_Equip;
        }
         modelChar.idSkin_Full = idSkin;
        modelChar.idSkin_Full= -1;
        modelChar.idSkin_Full = idSkin;
        listSkins_Full [ modelChar.idSkin_Full].obj_Lock.SetActive(!DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].isUnlock);
        listSkins_Full [ modelChar.idSkin_Full].obj_Selecting.SetActive(true);
        listSkins_Full [ modelChar.idSkin_Full].image_BG.sprite = DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].isUnlock ? sprite_Unlock : sprite_Lock;
        listSkins_Full [ modelChar.idSkin_Full].image_Avatar.color = DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].isUnlock ? new Color(255, 255, 255, 255) : new Color(0, 0, 0, 0.3f);
        obj_BtnSkinLock.SetActive(!DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].isUnlock && !GameManager.ins.arrayDataSkin_Full [ modelChar.idSkin_Full].isVip);
        obj_BtnSkinUnlock.SetActive(DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].isUnlock);
        obj_OneTime.SetActive(DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].play1Game_Unlock1Time);
        txt_Cost.text = "" + GameManager.ins.arrayDataSkin_Full [ modelChar.idSkin_Full].cost;

        //Mặc thử skin
        modelChar.WearSkinByIDCur();
    }


    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.tag.Contains(Tag.Player.ToString())) {
                    dragging = true;
                    mousePos_StartX = Input.mousePosition.x;
                    eulerAngle = modelChar.transform.eulerAngles;
                }
            }
        }
        if (Input.GetMouseButton(0)) {
            if (dragging) {
                modelChar.transform.eulerAngles = eulerAngle + new Vector3(0, 0.65f * (mousePos_StartX - Input.mousePosition.x), 0);
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
    }

    public override void Close(bool showUIForm = true) {
        //SceneManager.ins.formHome.camFollower.state = 1;
        if (SceneManager.ins.formHome != null) SceneManager.ins.formHome.BackToSkinCur();
        if (SceneManager.ins.formHome != null &&  SceneManager.ins.formHome.obj_BtnOfferGold != null) SceneManager.ins.formHome.obj_BtnOfferGold.SetActive(false);
        //modelChar.transform.DORotate(Vector3.zero,0.25f, RotateMode.Fast);
        base.Close();
    }

    #region Button
    public void Btn_ChangeTab(int idTab) {
        if(isShowed) SoundManager.ins.sound_Click.PlaySound();
        tab = idTab;
        modelChar.idSkin_Hair = DataManager.ins.gameSave.idSkin_Hair;
         modelChar.idSkin_Hand = DataManager.ins.gameSave.idSkin_Hand;
         modelChar.idSkin_Body = DataManager.ins.gameSave.idSkin_Body;
         modelChar.idSkin_Full = DataManager.ins.gameSave.idSkin_Full;
        obj_ListCardSkins_Hair.SetActive(idTab == 0);
        obj_ListCardSkins_Hand.SetActive(idTab == 1);
        obj_ListCardSkins_Body.SetActive(idTab == 2);
        obj_ListCardSkins_Full.SetActive(idTab == 3);
        obj_TapHair_On.SetActive(idTab == 0);
        obj_TapHand_On.SetActive(idTab == 1);
        obj_TapBody_On.SetActive(idTab == 2);
        obj_TapFull_On.SetActive(idTab == 3);
        obj_TapHair_Off.SetActive(idTab != 0);
        obj_TapHand_Off.SetActive(idTab != 1);
        obj_TapBody_Off.SetActive(idTab != 2);
        obj_TapFull_Off.SetActive(idTab != 3);
        if (tab == 0) {
            modelChar.idSkin_Full = -1;
            if ( modelChar.idSkin_Hair < 0)  modelChar.idSkin_Hair = 0;
            for (int i = 0; i < GameManager.ins.arrayDataSkin_Hair.Length; i++) {
                if (i ==  modelChar.idSkin_Hair) {
                    ClickToSkin_Hair( modelChar.idSkin_Hair, false);
                    if(i <= 5) {
                        scroll_TapHair.horizontalNormalizedPosition = 0;
                    } else if (i >= GameManager.ins.arrayDataSkin_Hair.Length-6) {
                        scroll_TapHair.horizontalNormalizedPosition = 1;
                    } else {
                        scroll_TapHair.horizontalNormalizedPosition = (float)(i-5) / (GameManager.ins.arrayDataSkin_Hair.Length - 10);
                    }
                } else {
                    listSkins_Hair [i].obj_Selecting.SetActive(false);
                    listSkins_Hair [i].obj_Wearing.SetActive(false);
                }
            }
        } else if (tab == 1) {
            modelChar.idSkin_Full = -1;
            if ( modelChar.idSkin_Hand < 0)  modelChar.idSkin_Hand = 0;
            for (int i = 0; i < GameManager.ins.arrayDataSkin_Hand.Length; i++) {
                if (i ==  modelChar.idSkin_Hand) {
                    ClickToSkin_Hand( modelChar.idSkin_Hand, false);
                    if (i <= 5) {
                        scroll_TapHand.horizontalNormalizedPosition = 0;
                    } else if (i >= GameManager.ins.arrayDataSkin_Hand.Length - 6) {
                        scroll_TapHand.horizontalNormalizedPosition = 1;
                    } else {
                        scroll_TapHand.horizontalNormalizedPosition = (float)(i - 5) / (GameManager.ins.arrayDataSkin_Hand.Length - 10);
                    }
                } else {
                    listSkins_Hand [i].obj_Selecting.SetActive(false);
                    listSkins_Hand [i].obj_Wearing.SetActive(false);
                }
            }
        } else if (tab ==2) {
            modelChar.idSkin_Full = -1;
            if ( modelChar.idSkin_Body < 0)  modelChar.idSkin_Body = 0;
            for (int i = 0; i < GameManager.ins.arrayDataSkin_Body.Length; i++) {
                if (i ==  modelChar.idSkin_Body) {
                    ClickToSkin_Body( modelChar.idSkin_Body, false);
                    if (i <= 5) {
                        scroll_TapBody.horizontalNormalizedPosition = 0;
                    } else if (i >= GameManager.ins.arrayDataSkin_Body.Length - 6) {
                        scroll_TapBody.horizontalNormalizedPosition = 1;
                    } else {
                        scroll_TapBody.horizontalNormalizedPosition = (float)(i - 5) / (GameManager.ins.arrayDataSkin_Body.Length - 10);
                    }
                } else {
                    listSkins_Body [i].obj_Selecting.SetActive(false);
                    listSkins_Body [i].obj_Wearing.SetActive(false);
                }
            }
        } else if (tab == 3) {
            if ( modelChar.idSkin_Full < 0)  modelChar.idSkin_Full = 0;
            for (int i = 0; i < GameManager.ins.arrayDataSkin_Full.Length; i++) {
                if (i ==  modelChar.idSkin_Full) {
                    ClickToSkin_Full( modelChar.idSkin_Full, false);
                    if (i <= 5) {
                        scroll_TapFull.horizontalNormalizedPosition = 0;
                    } else if (i >= GameManager.ins.arrayDataSkin_Full.Length - 6) {
                        scroll_TapFull.horizontalNormalizedPosition = 1;
                    } else {
                        scroll_TapFull.horizontalNormalizedPosition = (float)(i - 5) / (GameManager.ins.arrayDataSkin_Full.Length - 10);
                    }
                } else {
                    listSkins_Full [i].obj_Selecting.SetActive(false);
                    listSkins_Full [i].obj_Wearing.SetActive(false);
                }
            }
        }
        modelChar.WearSkinByIDCur();
    }

    public void Btn_SelectSkin(Card_Skin card) {
        SoundManager.ins.sound_Click.PlaySound();
        if (tab == 0) {
            ClickToSkin_Hair(card.idCard);
        } else if (tab == 1) {
            ClickToSkin_Hand(card.idCard);
        } else if (tab == 2) {
            ClickToSkin_Body(card.idCard);
        } else if (tab == 3) {
            ClickToSkin_Full(card.idCard);
        }
    }

    public void Btn_Buy() {
        SoundManager.ins.sound_Click.PlaySound();
        if (tab == 0) {
            //Trừ tiền nếu đủ -> 
            if (DataManager.ins.gameSave.gold >= GameManager.ins.arrayDataSkin_Hair [ modelChar.idSkin_Hair].cost) {
                SoundManager.ins.sound_Click.PlaySound();
                DataManager.ins.ChangeGold(-(GameManager.ins.arrayDataSkin_Hair [ modelChar.idSkin_Hair].cost), "BuySkinHair_Gold", true, false);
                //Thêm hiệu ứng chạy số tiền
                SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
                    goldOld = DataManager.ins.gameSave.gold;
                });
                DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].isUnlock = true;
                DataManager.ins.gameSave.listSkins_Hair [modelChar.idSkin_Hair].isBuy = true; 
                ClickToSkin_Hair( modelChar.idSkin_Hair);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            }
        } else if (tab == 1) {
            //Trừ tiền nếu đủ -> 
            if (DataManager.ins.gameSave.gold >= GameManager.ins.arrayDataSkin_Hand [ modelChar.idSkin_Hand].cost) {
                SoundManager.ins.sound_Click.PlaySound();
                DataManager.ins.ChangeGold(-(GameManager.ins.arrayDataSkin_Hand [ modelChar.idSkin_Hand].cost), "BuySkinHand_Gold", true, false);
                //Thêm hiệu ứng chạy số tiền
                SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
                    goldOld = DataManager.ins.gameSave.gold;
                });
                DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].isUnlock = true;
                DataManager.ins.gameSave.listSkins_Hand [modelChar.idSkin_Hand].isBuy = true;
                ClickToSkin_Hand( modelChar.idSkin_Hand);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            }
        } else if (tab == 2) {
            //Trừ tiền nếu đủ -> 
            if (DataManager.ins.gameSave.gold >= GameManager.ins.arrayDataSkin_Body [ modelChar.idSkin_Body].cost) {
                SoundManager.ins.sound_Click.PlaySound();
                DataManager.ins.ChangeGold(-(GameManager.ins.arrayDataSkin_Body [ modelChar.idSkin_Body].cost), "BuySkinBody_Gold", true, false);
                //Thêm hiệu ứng chạy số tiền
                SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
                    goldOld = DataManager.ins.gameSave.gold;
                });
                DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].isUnlock = true;
                DataManager.ins.gameSave.listSkins_Body [modelChar.idSkin_Body].isBuy = true;
                ClickToSkin_Body( modelChar.idSkin_Body);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            }
        } else if (tab == 3) {
            //Trừ tiền nếu đủ -> 
            if (DataManager.ins.gameSave.gold >= GameManager.ins.arrayDataSkin_Full [ modelChar.idSkin_Full].cost) {
                SoundManager.ins.sound_Click.PlaySound();
                DataManager.ins.ChangeGold(-(GameManager.ins.arrayDataSkin_Full [ modelChar.idSkin_Full].cost), "BuySkinFull_Gold", true, false);
                //Thêm hiệu ứng chạy số tiền
                SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, goldOld, () => {
                    goldOld = DataManager.ins.gameSave.gold;
                });
                DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].play1Game_Unlock1Time = false;
                DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].isUnlock = true;
                DataManager.ins.gameSave.listSkins_Full [modelChar.idSkin_Full].isBuy = true;
                ClickToSkin_Full( modelChar.idSkin_Full);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            }
        }
    }

    public void Btn_VideoAds() {
        SoundManager.ins.sound_Click.PlaySound();
        if (tab == 0) {
            MaxManager.ins.ShowRewardedAd("TryFreeSkinHair_VideoAds", "TryFree_" + DataManager.ins.gameSave.listSkins_Hair [modelChar.idSkin_Hair].keyID, () => {
                DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].play1Game_Unlock1Time = true;
                DataManager.ins.gameSave.listSkins_Hair [ modelChar.idSkin_Hair].isUnlock = true;
                ClickToSkin_Hair( modelChar.idSkin_Hair);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            });
        } else if (tab == 1) {
            MaxManager.ins.ShowRewardedAd("TryFreeSkinHand_VideoAds", "TryFree_" + DataManager.ins.gameSave.listSkins_Hand [modelChar.idSkin_Hand].keyID, () => {
                DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].play1Game_Unlock1Time = true;
                DataManager.ins.gameSave.listSkins_Hand [ modelChar.idSkin_Hand].isUnlock = true;
                ClickToSkin_Hand( modelChar.idSkin_Hand);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            });
        } else if (tab == 2) {
            MaxManager.ins.ShowRewardedAd("TryFreeSkinBody_VideoAds", "TryFree_" + DataManager.ins.gameSave.listSkins_Body [modelChar.idSkin_Body].keyID, () => {
                DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].play1Game_Unlock1Time = true;
                DataManager.ins.gameSave.listSkins_Body [ modelChar.idSkin_Body].isUnlock = true;
                ClickToSkin_Body( modelChar.idSkin_Body);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            });
        }else if (tab == 3) {
            MaxManager.ins.ShowRewardedAd("TryFreeSkinFull_VideoAds", "TryFree_" + DataManager.ins.gameSave.listSkins_Full [modelChar.idSkin_Full].keyID, () => {
                DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].play1Game_Unlock1Time = true;
                DataManager.ins.gameSave.listSkins_Full [ modelChar.idSkin_Full].isUnlock = true;
                ClickToSkin_Full( modelChar.idSkin_Full);
                effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                effect_ChangeSkin.Play();
            });
        } 
    }

    public void Btn_Unequipped() {
        if (img_BtnEquipp.sprite == sprite_Equip) {//Mặc trang bị lên
            SoundManager.ins.sound_Click.PlaySound();
            if (tab == 0) {
                if ( modelChar.idSkin_Hair >= 0) ClickToSkin_Hair( modelChar.idSkin_Hair);
            } else if (tab == 1) {
                if ( modelChar.idSkin_Hand >= 0) ClickToSkin_Hand( modelChar.idSkin_Hand);
            } else if (tab == 2) {
                if ( modelChar.idSkin_Body >= 0) ClickToSkin_Body( modelChar.idSkin_Body);
            } else if (tab == 3) {
                if( modelChar.idSkin_Full >= 0) ClickToSkin_Full( modelChar.idSkin_Full);
            }
            img_BtnEquipp.sprite = sprite_UnEquip;
            txt_Equipp.text = "Unequip";
            txt_Equipp.color = color_UnEquip;
        } else {//Tháo trang bị ra
            SoundManager.ins.sound_Click.PlaySound();
            if (tab == 0) {
                if (modelChar.idSkin_Hair >= 0) { 
                    listSkins_Hair [modelChar.idSkin_Hair].obj_Wearing.SetActive(false);
                    listSkins_Hair [modelChar.idSkin_Hair].obj_Selecting.SetActive(false);
                }
                DataManager.ins.gameSave.idSkin_Hair = modelChar.idSkin_Hair = -1;
                obj_BtnSkinUnlock.SetActive(false);
            } else if (tab == 1) {
                if ( modelChar.idSkin_Hand >= 0) {
                    listSkins_Hand [modelChar.idSkin_Hand].obj_Wearing.SetActive(false);
                    listSkins_Hand [modelChar.idSkin_Hand].obj_Selecting.SetActive(false);
                }
                DataManager.ins.gameSave.idSkin_Hand = modelChar.idSkin_Hand = -1;
                obj_BtnSkinUnlock.SetActive(false);
            } else if (tab == 2) {
                if ( modelChar.idSkin_Body >= 0) {
                    listSkins_Body [modelChar.idSkin_Body].obj_Wearing.SetActive(false);
                    listSkins_Body [modelChar.idSkin_Body].obj_Selecting.SetActive(false);
                }
                DataManager.ins.gameSave.idSkin_Body = modelChar.idSkin_Body =  0;
                listSkins_Body [0].obj_Wearing.SetActive(true);
                listSkins_Body [0].obj_Selecting.SetActive(true);
                scroll_TapBody.horizontalNormalizedPosition = 0;
            } else if (tab == 3) {
                if ( modelChar.idSkin_Full >= 0) {
                    listSkins_Full [modelChar.idSkin_Full].obj_Wearing.SetActive(false);
                    listSkins_Full [modelChar.idSkin_Full].obj_Selecting.SetActive(false);
                }
                DataManager.ins.gameSave.idSkin_Full = modelChar.idSkin_Full = -1;
                DataManager.ins.gameSave.idSkin_Hair = modelChar.idSkin_Hair = -1;
                DataManager.ins.gameSave.idSkin_Hand = modelChar.idSkin_Hand = -1;
                DataManager.ins.gameSave.idSkin_Body = modelChar.idSkin_Body = 0;
                obj_BtnSkinUnlock.SetActive(false);
            }
            modelChar.WearSkinByIDCur();
            img_BtnEquipp.sprite = sprite_Equip;
            txt_Equipp.text = "Equip";
            txt_Equipp.color = color_Equip;
        }
    }

    public void Btn_Close() {
        SoundManager.ins.sound_Click.PlaySound();
        Close();
    }
    #endregion
}

