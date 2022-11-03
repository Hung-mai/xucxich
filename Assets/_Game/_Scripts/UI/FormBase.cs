using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class FormBase : MonoBehaviour
{
    public FormUI idForm;
    public bool isOpened;
    public DOTweenAnimation[] twn_Pos;
    public Action OnClosed;

    public virtual void Show()
    {
        isOpened = true;
        gameObject.SetActive(true);
        OnClosed = null;
    }

    public virtual void Close()
    {
        isOpened = false;
        if (OnClosed != null)
        {
            OnClosed();
            OnClosed = null;
        }
    }
}
