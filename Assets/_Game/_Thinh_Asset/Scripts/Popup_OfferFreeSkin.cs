using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Popup_OfferFreeSkin : PopupBase
{
    public ModelChar modelChar;
    public List<Data_Skin> dataSkins;
    [HideInInspector]public Data_Skin currentDataSkin;

    public int timeLastShow = 0;

    public override void Show()
    {
        base.Show();
        GameManager.ins.timeOffer = 0;
        /*
         *check từ đầu đến cuối list dataSkins k sở hữu.
         *if(sở hữu) chuyển sang check cái tiếp theo
         *else if(không sở hữu) dùng luôn và dừng xử lý luôn
         *-khi tất cả đều trùng -> random trong list body chưa sở hữu
         */
        foreach (Data_Skin body in dataSkins)
        {
            if (!IsOwnSkinBody(body) && body.keyID != DataManager.ins.gameSave.lastSkinBodyShow.keyID)
            {
                ShowSkinBody(body);
                return;
            }
        }
        //nếu chạy tới đây => sở hữu hết trong list rồi 
        List<Data_Skin> lstBodyNotOwn = GetLstBodyNotOwn();
        if (lstBodyNotOwn.Count == 0)
        {
            Close();
            return;
        }
        ShowSkinBody(lstBodyNotOwn[UnityEngine.Random.Range(0,lstBodyNotOwn.Count)]);
        
    }
    private List<Data_Skin> GetLstBodyNotOwn()
    {
        List<Data_Skin> lstData_SkinBodyNotOwn = new List<Data_Skin>();
        foreach (Skin_Save sv in DataManager.ins.gameSave.listSkins_Body)
        {
            if (!sv.isBuy)
            {
                foreach (Data_Skin d in GameManager.ins.arrayDataSkin_Body)
                {
                    if (sv.keyID == d.keyID)
                    {
                        lstData_SkinBodyNotOwn.Add(d);
                    }
                }
            }
        }
        return lstData_SkinBodyNotOwn;
    }
    private bool IsOwnSkinBody(Data_Skin ds)
    {
        foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Body)
        {
            if (s.keyID == ds.keyID)
            {
                if (s.isBuy)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
    private void ShowSkinBody(Data_Skin ds)
    {
        if (modelChar.idSkin_Full >= 0) modelChar.idSkin_Full = -1;
        
        for(int i = 0; i < GameManager.ins.arrayDataSkin_Body.Length; i++)
        {
            if (GameManager.ins.arrayDataSkin_Body[i] == ds)
            {
                modelChar.idSkin_Body = i;
                DataManager.ins.gameSave.lastSkinBodyShow = new Skin_Save(GameManager.ins.arrayDataSkin_Body[i].keyID);
                break;
            }
        }
        currentDataSkin = ds;
        modelChar.WearSkinByIDCur(true);
        modelChar.animator.SetTrigger("Dance"+UnityEngine.Random.Range(1,10).ToString());//random anim dance
    }
    private void GetSkinBodyOffer()
    {
        MaxManager.ins.ShowRewardedAd("OfferFreeSkin", "OfferFreeSkin_" + currentDataSkin.keyID,()=> {
            DataManager.ins.gameSave.idSkin_Full = -1;
            DataManager.ins.gameSave.idSkin_Body = modelChar.idSkin_Body;
            DataManager.ins.gameSave.listSkins_Body[modelChar.idSkin_Body].play1Game_Unlock1Time = false;
            DataManager.ins.gameSave.listSkins_Body[modelChar.idSkin_Body].isUnlock = true;
            DataManager.ins.gameSave.listSkins_Body[modelChar.idSkin_Body].isBuy = true;
            Close();
        },null);
    }
    #region Button
    public void BtnBtnGetit()
    {
        GetSkinBodyOffer();
    }
    public void OnBtnNoThanks()
    {
        Close();
    }
    public override void Close(bool showUIForm = true)
    {
        base.Close(showUIForm);
        if(SceneManager.ins.formHome != null) SceneManager.ins.formHome.BackToSkinCur();
    }
    #endregion
}
