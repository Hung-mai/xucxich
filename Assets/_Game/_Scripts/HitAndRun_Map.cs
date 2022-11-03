using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAndRun_Map : MonoBehaviour {
    public float speedBoatMax = 25;
    public float timeMoveBoat = 1f;
    public float timeBallUp = 0.2f;
    public float timeBallDown = 0.2f;
    public float timeBallStanding = 1.5f;
    private int rateRandomBall = 100;
    public int idRandomBallOrBomb = 0;
    public List<bool> listRandomBall;
    public int amountCharCorrectInWave = 0;
    public HitAndRun_Boat prefab_Boat;
    public List<HitAndRun_Boat> listBoats;

    public IEnumerator DropBalloonOrBomb() {
        yield return new WaitForSeconds(Random.Range(0.3f, 1.5f));
        rateRandomBall = Random.Range(0, 100);
        amountCharCorrectInWave = 0;
        for (int i = 0; i < listBoats.Count; i++) {
            //VD:percentBomb = 30 -> 30% là Bomb, 70% là Balloon
            if ((HitAndRun_Manager.ins.listChars[i].isAlive || HitAndRun_Manager.ins.listChars[i].isAnimationDyning) && !HitAndRun_Manager.ins.listChars [i].isFinish) {
                if (listRandomBall[idRandomBallOrBomb]) {
                    listBoats[i].isShowingBalloon = true;
                    listBoats[i].isShowingBomb = false;
                    listBoats[i].obj_Balloon.SetActive(true);
                    listBoats[i].obj_Balloon.transform.DOLocalMoveY(1f, timeBallUp);
                    listBoats[i].obj_Balloon.transform.localScale = Vector3.one;
                    listBoats[i].twnScale_Balloon.DORestart();
                } else {
                    listBoats[i].isShowingBalloon = false;
                    listBoats[i].isShowingBomb = true;
                    listBoats[i].obj_Bomb.SetActive(true);
                    listBoats[i].obj_EffectExplode.SetActive(false);
                    listBoats[i].obj_Bomb.transform.DOLocalMoveY(1f, timeBallUp);
                }
            }
        }
        idRandomBallOrBomb++;
        if (idRandomBallOrBomb >= listRandomBall.Count) idRandomBallOrBomb = 0;
        yield return new WaitForSeconds(timeBallUp);
        yield return new WaitForSeconds(timeBallStanding);
        for (int i = 0; i < listBoats.Count; i++) {
            if (listBoats[i].isShowingBalloon) {
                listBoats[i].isShowingBalloon = false;
                listBoats[i].obj_Balloon.transform.DOLocalMoveY(-0.5f, timeBallDown);
            }
            if (listBoats[i].isShowingBomb) {
                listBoats[i].isShowingBomb = false;
                listBoats[i].obj_Bomb.transform.DOLocalMoveY(-0.5f, timeBallDown);
            }
        }
        yield return new WaitForSeconds(timeBallDown);
        for (int i = 0; i < listBoats.Count; i++) {
            listBoats[i].obj_Balloon.SetActive(false);
            listBoats[i].obj_Bomb.SetActive(false);
        }
        if (!HitAndRun_Manager.ins.isGameplay_End) StartCoroutine(DropBalloonOrBomb());

    }
}
