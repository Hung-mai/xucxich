using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HitAndRun_Boat : MonoBehaviour
{
    public bool isShowingBalloon = true;
    public bool isShowingBomb = false;
    public GameObject obj_Balloon;
    public GameObject obj_Bomb;
    public SoundObject sound_Balloon;
    public SoundObject sound_Bomb;
    public GameObject obj_EffectExplode;
    public ParticleSystem particleSystem_Wind;
    public Collider_KillChar collider_KillChar_Bomb;
    public DOTweenAnimation twnScale_Balloon;
    public TextMeshPro txt_Point;

    public void BalloonBurst_MoveBoat(bool isPlayer = false) {
        obj_Balloon.SetActive(false);
        int point = (10 - HitAndRun_Manager.ins.hitAndRun_Map.amountCharCorrectInWave*1);
        if(isPlayer) particleSystem_Wind.Play();
        sound_Balloon.PlaySound();
        transform.DOMove(transform.position +  Vector3.back* point / 10f * HitAndRun_Manager.ins.hitAndRun_Map.speedBoatMax, HitAndRun_Manager.ins.hitAndRun_Map.timeMoveBoat);
        txt_Point.text = "+" + point;
        txt_Point.alpha = 1;
        txt_Point.transform.localPosition = new Vector3(txt_Point.transform.localPosition.x, 35, txt_Point.transform.localPosition.z);
        txt_Point.transform.DOLocalMoveY(45, HitAndRun_Manager.ins.hitAndRun_Map.timeMoveBoat*2);
        txt_Point.DOFade(0, HitAndRun_Manager.ins.hitAndRun_Map.timeMoveBoat*2);
        HitAndRun_Manager.ins.hitAndRun_Map.amountCharCorrectInWave++;
    }

    public void BombExplosion_MoveBoat() {
        obj_Bomb.SetActive(false);
        obj_EffectExplode.SetActive(true);
        sound_Bomb.PlaySound();
    }
}
