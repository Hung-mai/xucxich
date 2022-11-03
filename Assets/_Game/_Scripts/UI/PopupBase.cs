using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour{
    public PopupUI idPopup;
    public CanvasGroup canvasGroup;
    public bool isCloseByEscape;
    public bool isOpened;

    [HideInInspector]
    public Action OnClosed;

    public virtual void Show()
    {
        isOpened = true;
        gameObject.SetActive(true);
        SceneManager.ins.popupList.Add(this);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1f, 0.2f).SetUpdate(true);
        }
    }

    public virtual void Close(bool showUIForm = true)
    {
        isOpened = false;
        if (OnClosed != null)
        {
            OnClosed();
            OnClosed = null;
        }
        gameObject.SetActive(false);
    }
}
