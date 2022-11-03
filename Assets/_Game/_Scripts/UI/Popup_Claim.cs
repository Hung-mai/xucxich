using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Popup_Claim : PopupBase
{/*
    public Image imgAvatar;
    public RectTransform mask;
    public TextMeshProUGUI txtProcess;

    public GameObject panel_Continue;
    public GameObject panel_Claim;
    public override void Show()
    {
        base.Show();
        if (DataManager.ins.gameSave.keyIDSkinClaimming != "")
        {
            foreach (Data_Skin ds in GameManager.ins.arrayDatasSkin)
            {
                if (ds.keyID == DataManager.ins.gameSave.keyIDSkinClaimming)
                {
                    imgAvatar.sprite = ds.img;
                    imgAvatar.SetNativeSize();

                    float tongcong = imgAvatar.rectTransform.sizeDelta.y;
                    float heightMask = tongcong * DataManager.ins.gameSave.processClaiming / 100;
                    mask.sizeDelta = new Vector2(mask.sizeDelta.x,-heightMask);
                    mask.anchoredPosition = new Vector2(mask.anchoredPosition.x,heightMask / 2);

                    txtProcess.text = DataManager.ins.gameSave.processClaiming.ToString() + " %";
                    break;
                }
            }
        }
        else
        {
            //chua xu ly luong nay`
        }
    }
    [ContextMenu("TEST")]
    public void TEST()
    {
        RunProcess(100);
    }
    private void RunProcess(int processTarget)
    {
        float tongcong = imgAvatar.rectTransform.sizeDelta.y;
        float heightMaskTarget = tongcong * processTarget / 100;

        mask.DOSizeDelta(new Vector2(mask.sizeDelta.x, -heightMaskTarget),1f).SetEase(Ease.Linear);
        mask.DOAnchorPos(new Vector2(mask.anchoredPosition.x, heightMaskTarget / 2),1f).SetEase(Ease.Linear).OnUpdate(()=> {
            //quy đổi heightmaskTarget ra %
            txtProcess.text = (-mask.sizeDelta.y / tongcong * 100).ToString("00.") + " %";
        }).OnComplete(()=> {
            if (processTarget == 100)
            {
                //pháo hoa nổ 
                //hiện UI
                panel_Claim.SetActive(true);
            }
            else
            {
                panel_Continue.SetActive(true);
            }
        });

        
    }
    #region Button
    public void Btn_Continue()
    {

    }
    public void Btn_Claim()
    {

    }
    public void Btn_LoseIt()
    {
        Debug.Log("OK");
    }
    #endregion
    */
}
