using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card_Skin : MonoBehaviour {
    public int idCard;
    public Image image_BG;
    public Image image_Avatar;
    //public TextMeshProUGUI text_Cost;
    public GameObject obj_Lock;
    public GameObject obj_Selecting;
    public GameObject obj_Wearing;
    public GameObject obj_VIP;
    //public GameObject obj_BtnVideoAds;
    //public GameObject obj_BtnBuy;

    public Popup_Skin popup_Skin;

    public void Btn_Select()
    {
        SoundManager.ins.sound_Click.PlaySound();
        popup_Skin.Btn_SelectSkin(this);
    }
}
