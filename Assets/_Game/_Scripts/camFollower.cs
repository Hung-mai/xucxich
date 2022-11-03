using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class camFollower : MonoBehaviour {
    public static camFollower ins;
    public int state;

    [Header("-------- IntroGame --------")]
    public Vector3 localPositionIntroGame;
    public Vector3 angleIntroGame;
    
    [Header("-------- StartGame (1) --------")]
    public Vector3 localPositionStartGame;
    public Vector3 angleStartGame;
    public float speedStartGame = 0.039f;
    public Action actionStartGame;

    [Header("-------- PlayGame (2)--------")]
    public Vector3 localPositionPlayGame;
    public Vector3 anglePlayGame;
    public float speedPlayGame = 0.015f;
    public Action actionPlayGame;

    [Header("-------- InGame_0 (3)--------")]
    public Vector3 localPositionInGame_0;
    public Vector3 angleInGame_0;
    public float speedInGame_0 = 0.015f;
    public Action actionInGame_0;

    [Header("-------- WinGame  (4)--------")]
    public Vector3 localPositionWinGame;
    public Vector3 angleWinGame;
    public float speedWinGame = 0.015f;
    public Action actionWinGame;

    public bool isDoing = false;
    private Vector3 localPositionOrigin = Vector3.zero;
    private Vector3 eulerAnglesOrigin = Vector3.zero;

    private void Awake() {
        ins = this;
        state = 0;
    }

    void LateUpdate() {
        if (state == 1) {//Intro -> Start
            if ((localPositionStartGame - transform.localPosition).sqrMagnitude > 0.00001f) {
                if (!isDoing) {
                    isDoing = true;
                    localPositionOrigin = transform.localPosition;
                    eulerAnglesOrigin = transform.eulerAngles;
                }
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, localPositionStartGame, speedStartGame);
                if ((localPositionStartGame - localPositionOrigin).sqrMagnitude == 0) {
                    transform.eulerAngles = angleStartGame;
                } else {
                    transform.eulerAngles = eulerAnglesOrigin + (angleStartGame - eulerAnglesOrigin) * (1 - (localPositionStartGame - transform.localPosition).sqrMagnitude/(localPositionStartGame - localPositionOrigin).sqrMagnitude);
                }
                //transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, angleStartGame.x, speedStartGame), Mathf.LerpAngle(transform.eulerAngles.y, angleStartGame.y, speedStartGame), 0);
            } else {
                state = 0;
                isDoing = false;
                actionStartGame?.Invoke();
                actionStartGame = null;
            }
        }else if (state == 2) {//Start -> Play
            if ((localPositionPlayGame - transform.localPosition).sqrMagnitude > 0.00001f) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, localPositionPlayGame, speedPlayGame);
                transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, anglePlayGame.x, speedPlayGame), Mathf.LerpAngle(transform.eulerAngles.y, anglePlayGame.y, speedPlayGame), 0);
            } else {
                state = 0;
                actionPlayGame?.Invoke();
                actionPlayGame = null;
            }
        } else if (state == 3) {//?
            if ((localPositionInGame_0 - transform.localPosition).sqrMagnitude > 0.00001f) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, localPositionInGame_0, speedInGame_0);
                transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, angleInGame_0.x, speedInGame_0), Mathf.LerpAngle(transform.eulerAngles.y, angleInGame_0.y, speedInGame_0), 0);
            } else {
                state = 0;
                actionInGame_0?.Invoke();
                actionInGame_0 = null;
            }
        } else if (state == 4) {//?
            if ((localPositionWinGame - transform.localPosition).sqrMagnitude > 0.00001f) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, localPositionWinGame, speedWinGame);
                transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, angleWinGame.x, speedWinGame), Mathf.LerpAngle(transform.eulerAngles.y, angleWinGame.y, speedWinGame), 0);
                if(Form_Gameplay.ins.minigameManager.minigame == Minigame.SkewerScurry) {
                    Camera.main.transform.position = transform.position;
                    Camera.main.transform.eulerAngles = transform.eulerAngles;
                }
            } else {
                state = 0;
                actionWinGame?.Invoke();
                actionWinGame = null;
            }
        }
    }
}
