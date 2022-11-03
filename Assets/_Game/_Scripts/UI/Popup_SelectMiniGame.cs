using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class Popup_SelectMiniGame : PopupBase
{
    /*
     * hình ảnh ở trong ô ảnh sẽ nhảy liên tục cùng với tên thay đổi liên tục
     * sau 1 khoảng thời gian hoặc 1 số lần đổi thì sẽ nhảy ra 1 game tiếp theo
     */
    [Header("--------pop select mini game--------")]
    public TextMeshProUGUI txtGameName;
    //public TextMeshProUGUI txtGameInfo;

    public GameObject [] listStarDifficulty;

    public Image imgMainGame;
    public RectTransform gameImg_container;
    public RectTransform frame_container;

    public override void Show()
    {
        base.Show();
        if (SceneManager.ins.obj_Gold != null)
        {
            SceneManager.ins.obj_Gold.SetActive(false);
        }
        if (SceneManager.ins.obj_Cup != null)
        {
            SceneManager.ins.obj_Cup.SetActive(false);
        }
        for (int i = 0; i < listStarDifficulty.Length; i++) {
            listStarDifficulty [i].SetActive(i <= Mathf.CeilToInt(DataManager.ins.gameSave.difficulty - 0.01f));
        }
    }
    public void Active(Data_Minigame nextMinigame,Action OnComplete)
    {
        StartCoroutine(I_Active(nextMinigame, OnComplete));
    }
    IEnumerator I_Active(Data_Minigame nextMinigame,Action OnComplete)
    {
        bool a = true;
        Timer.Schedule(this,3f,()=> {
            a = false;
            Debug.Log("a = false");
        });
        float t = 0.05f;
        int i = 0;
        while (a)
        {
            if (i >= GameManager.ins.listDatasMinigame_Available.Count) i = 0;
            Data_Minigame dm = GameManager.ins.listDatasMinigame_Available [i];

            imgMainGame.sprite = dm.imgThumbnail;
            txtGameName.text = dm.nameDisplay;
            //txtGameInfo.text = dm.GameInfo;

            yield return new WaitForSeconds(t);
            t += 0.03f;
            i++;
        }
        imgMainGame.sprite = nextMinigame.imgThumbnail;
        txtGameName.text = nextMinigame.nameDisplay;
        yield return new WaitForSeconds(1f);
        frame_container.DOScale(new Vector3(1.2f,1.2f,1),0.15f);
        yield return new WaitForSeconds(0.15f);
        frame_container.DOScale(Vector3.one, 0.15f);
        yield return new WaitForSeconds(1.25f);
        OnComplete();
    }
    public void CloseMySelf()
    {
        gameObject.SetActive(false);
    }
}
