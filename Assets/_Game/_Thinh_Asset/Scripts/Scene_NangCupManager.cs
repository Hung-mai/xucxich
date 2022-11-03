using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_NangCupManager : MonoBehaviour
{
    public Player player;
    public string nangCup, nangCupIdle;
    private void Start()
    {
        PlayScript();
    }
    private void PlayScript()
    {
        //cho` 1s
        player.modelChar.animator.Play(nangCup);
    }
}
