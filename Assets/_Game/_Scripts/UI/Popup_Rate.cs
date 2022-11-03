using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Rate : PopupBase{
    public int idStarRate = -1;

    public GameObject obj_BtnSureOn;
    public GameObject obj_BtnSureOff;
    public Card_Star[] listStar;
    private bool isShow;
    public bool IsShow { get => isShow; }

    public override void Show()
    {
        base.Show();
        for (int i = 0; i < listStar.Length; i++)
        {
            listStar[i].idCard = i;
        }
        idStarRate = -1;
        obj_BtnSureOn.SetActive(false);
        obj_BtnSureOff.SetActive(true);
        isShow = true;

        DataManager.ins.gameSave.starRate = 0;//Coi như đã rate rồi
        DataManager.ins.SaveGame();
    }

    #region BUTTON
    public void BtnOk()
    {
        
        SoundManager.ins.sound_Click.PlaySound();
        DataManager.ins.gameSave.starRate = idStarRate;

        //Nếu Rate >= 5 sao 
        if (idStarRate >= 5)
        {
            //Chuyển qua Store
            if (GameManager.ins.isIOS)
            {
                Application.OpenURL("https://apps.apple.com/us/app/id1512903345");
            }
            else
            {
                Application.OpenURL("http://play.google.com/store/apps/details?id=" + Application.identifier);
            }
        }
        DataManager.ins.SaveGame();
        Close();
    }

    public void BtnStar(Card_Star card)
    {
        
        SoundManager.ins.sound_Click.PlaySound();
        idStarRate = card.idCard + 1;
        obj_BtnSureOn.SetActive(true);
        obj_BtnSureOff.SetActive(false);
        for (int i = 0; i < listStar.Length; i++)
        {
            listStar[i].obj_StarOn.SetActive(i <= card.idCard);
        }
    }

    public void BtnClose()
    {
        
        SoundManager.ins.sound_Click.PlaySound();
        isShow=false;
        Close();
    }
    #endregion
}
