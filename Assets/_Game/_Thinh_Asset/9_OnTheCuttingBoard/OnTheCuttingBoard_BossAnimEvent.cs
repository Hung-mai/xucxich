using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OnTheCuttingBoard_BossAnimEvent : MonoBehaviour
{
    public Action OnEndIdle;
    public Action BeginHit;
    public Action OnEndHit;
    public Action OnEndHandUp;
    public Action OnStartHandUpLeft;
    public Action OnHitIntro;
    
    public void OnEndIdle_Anim()
    {
        OnEndIdle();
    }
    public void BeginHit_Anim()
    {
        BeginHit();
    }
    public void OnEndHit_Anim()
    {
        OnEndHit();
    }
    public void OnEndHandUp_Anim()
    {
        OnEndHandUp();
    }
    public void OnStartHandUpLeft_AnimEvent()
    {
        OnStartHandUpLeft();
    }
    public void OnHit()
    {
        OnHitIntro();
    }
    
}
