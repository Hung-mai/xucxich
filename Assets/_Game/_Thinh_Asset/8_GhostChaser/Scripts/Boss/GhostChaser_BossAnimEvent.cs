using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GhostChaser_BossAnimEvent : MonoBehaviour
{
    public GhostChaser_BossManager.BossSkin bossSkin;
    public Collider colHandAttack;

    [Header("------ParameterAnim-------")]
    public string idle;
    public string chase;
    public string hit;
    public string intro;
    public string spdAttack;
    public string taken;

    public Action OnEndHit_AnimEvent;
    public Action OnEndAnimHit_AnimEvent;
    public Action OnHit_AnimEvent;
    public Action OnEndScream_AnimEvent;


    public void OnEndHit()
    {
        OnEndHit_AnimEvent();
    }
    public void OnEndAnimHit()
    {
        OnEndAnimHit_AnimEvent();
    }
    public void OnHit()
    {
        OnHit_AnimEvent();
    }
    public void OnEndScream()
    {
        OnEndScream_AnimEvent();
    }
}
