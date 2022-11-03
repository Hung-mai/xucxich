using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitAndRun_Manager : Minigame_Manager
{
    public static HitAndRun_Manager ins;

    [Header("--------------- ref ---------------")]
    public Vector3 pos_CameraWin;
    public Vector3 angle_CameraWin;
    public HitAndRun_Map hitAndRun_Map;
    public Transform tran_Finish;

    public override void Awake() {
        base.Awake();
        if (ins != null) Destroy(ins.gameObject);
        ins = this;
        GameManager.ins.minigameManager = this;
    }

    private void Start() {
        InitGame();
    }

    void FixedUpdate() {
        if (SceneManager.ins.async != null) return;
        if (isGameplay_Start && !isGameplay_End) {
            for (int i = 0; i < listChars.Count; i++) {
                if (listChars [i].isFinish && listChars [i].state_Apply != Char_State.Win) {
                    if (i == 0) Form_Gameplay.ins.confetiFx.SetActive(true);
                    listChars [i].state_New = Char_State.Win;
                    hitAndRun_Map.listBoats[i].transform.DOMove(new Vector3(hitAndRun_Map.listBoats [i].transform.position.x, hitAndRun_Map.listBoats [i].transform.position.y, tran_Finish.position.z  - 10) , 1);
                    if (i == 0) camFollower.ins.state = 4;
                    if (i == 0 && SceneManager.ins.popup_EndGame == null) {
                        SceneManager.ins.ShowPopup_EndGame(true, 3f);
                    }
                    listChars [i].ReloadAnimation();
                } else {
                    listChars [i].Main();
                }
            }
            //Tính điểm để xếp hạng khi chưa có ai về đích
            if (hasCharFinishLine == false && Form_Gameplay.ins.timeCur < Constants.TIME_GAMEPLAY - 3) {
                List<Character> listCharByTop = listChars.OrderBy(c => c.transform.position.z).ToList();
                int top = 0;
                for (int i = 0; i < listCharByTop.Count; i++) {
                    if (listCharByTop [i].isAlive) {
                        listCharByTop [i].top = top;
                        top++;
                    } else {
                        listCharByTop [i].top = 100;
                    }
                }
            }
        } else if (isGameplay_End) {
            for (int i = 0; i < listChars.Count; i++) {
                if (listChars [i].isFinish && listChars [i].state_Apply != Char_State.Win) {
                    if (i == 0) Form_Gameplay.ins.confetiFx.SetActive(true);
                    listChars [i].state_New = Char_State.Win;
                    if (i == 0) camFollower.ins.state = 4;
                    if (i == 0 && SceneManager.ins.popup_EndGame == null) {
                        SceneManager.ins.ShowPopup_EndGame(true, 3f);
                    }
                    listChars [i].ReloadAnimation();
                } else if (!listChars[i].isAlive) {
                    listChars[i].state_New = Char_State.Death;
                    listChars[i].ReloadAnimation();
                } else if (listChars [i].isFinish) {
                    if (listChars [i].state_Apply != Char_State.Win) {
                        listChars [i].state_New = Char_State.Win;
                        listChars [i].ReloadAnimation();
                    }
                } else {
                    if (listChars [i].state_Apply != Char_State.Lose) {
                        listChars [i].state_New = Char_State.Lose;
                        listChars [i].ReloadAnimation();
                    }
                }
            }
        }
    }

    public override void InitGame() {
        listChars [0].username = DataManager.ins.gameSave.username;
        listChars [0].speed = speedChar;
        listChars [0].modelChar.collider_AttackChar.charAttacker = listChars [0];
        listChars [0].modelChar.WearSkinByGameSave(false);
        List<int> listIDUsername = Enumerable.Range(0, GameManager.ins.listUsernames.Length - 1).ToList();
        listIDUsername.Shuffle();
        List<int> listIDColor = Enumerable.Range(0, GameManager.ins.listColorsChar.Length - 1).ToList();
        listIDColor.Remove(listChars [0].modelChar.idColor);
        listIDColor.Shuffle();
        listIDColor.Insert(0, listChars [0].modelChar.idColor);
        List<int> listPointIntelligence = Enumerable.Range(1, maxCharacter).ToList();
        listPointIntelligence.Shuffle();
        for (int i = 1; i < maxCharacter; i++) {
            if (i >= listChars.Count) {
                listChars.Add(Instantiate(prefab_AIBot));
            }
            listChars [i].idChar = i;
            listChars [i].transform.position = nodes_Start [i].position;
            listChars [i].AIBot.pos_TargetMove = new Vector3(nodes_Start [i].position.x, transform.position.y, nodes_Start [i].position.z - 999);
            //listChars[i].tran_Rotate.LookAt(listChars[i].AIBot.pos_TargetMove);
            if (Random.Range(0, 1000) < Mathf.Min(DataManager.ins.gameSave.levelEnded*10, 30) && DataManager.ins.list_IDFull_ByColor [listIDColor [i]].Count > 0) {
                listChars [i].modelChar.idSkin_Full = DataManager.ins.list_IDFull_ByColor [listIDColor [i]] [Random.Range(0, DataManager.ins.list_IDFull_ByColor [listIDColor [i]].Count)];
                listChars [i].modelChar.idSkin_Hand = -1;
                listChars [i].modelChar.idSkin_Hair = -1;
                listChars [i].modelChar.idSkin_Body = -1;
            } else {
                listChars [i].modelChar.idSkin_Full = -1;
                listChars [i].modelChar.idSkin_Hand = Random.Range(0, GameManager.ins.arrayDataSkin_Hand.Length);
                listChars [i].modelChar.idSkin_Hair = Random.Range(0, GameManager.ins.arrayDataSkin_Hair.Length);
                listChars [i].modelChar.idSkin_Body = listIDColor [i];
            }
            listChars [i].modelChar.WearSkinByIDCur(false);
            listChars [i].modelChar.collider_AttackChar.charAttacker = listChars[i];
            listChars [i].username = GameManager.ins.listUsernames [listIDUsername [i - 1]];
            listChars [i].name = "AIBot_" + listChars [i].username;
            listChars [i].speed = speedChar;
            listChars [i].AIBot.amountRevive = amountAIReviveFree;
            listChars [i].AIBot.pointIntelligence = pointIntelligence_StartMin + listPointIntelligence [i]*pointIntelligence_StartRange/listPointIntelligence.Count;
        }
        List<HitAndRun_Boat> listBoatCache = new List<HitAndRun_Boat>();
        for (int i = 0; i < listChars.Count; i++) {
            if (i >= listIndicators.Count) {
                listIndicators.Add(Instantiate(prefab_Indicator, SceneManager.ins.canvas_Indicator.transform));
            }
            listIndicators [i].setUpIndicator(listChars [i]);
            listChars[i].tran_IndicatorTarget.localPosition += Vector3.up*0.03f;
            HitAndRun_Boat boat;
            if (i <= hitAndRun_Map.listBoats.Count - 1) {
                boat = hitAndRun_Map.listBoats [i];
                boat.transform.position = listChars [i].transform.position;
            } else {
                boat = Instantiate(hitAndRun_Map.prefab_Boat, listChars [i].transform.position, new Quaternion());
            }
            boat.gameObject.SetActive(true);
            listBoatCache.Add(boat);
            boat.transform.parent = null;
            boat.collider_KillChar_Bomb.action = boat.BombExplosion_MoveBoat;
            listChars [i].transform.SetParent(hitAndRun_Map.listBoats [i].transform);
            listChars [i].transform.localPosition = Vector3.zero;
        }
        hitAndRun_Map.listBoats = listBoatCache;
        camFollower.ins.transform.localPosition = camFollower.ins.localPositionIntroGame;
        camFollower.ins.transform.localEulerAngles = camFollower.ins.angleIntroGame;
        camFollower.ins.localPositionWinGame = pos_CameraWin;
        camFollower.ins.angleWinGame = angle_CameraWin;
        Timer.Schedule(this, 1.5f, () => {
            camFollower.ins.state = 1;
        });
        camFollower.ins.actionStartGame = () => {
            isGameplay_Intro = true;
            Form_Gameplay.ins.UIIntroGame();
        };
    }

    public override void StartGame() {
        if (!isGameplay_Start) {
            isGameplay_Start = true;
            Form_Gameplay.ins.UIStartGame();
            hitAndRun_Map.StartCoroutine(hitAndRun_Map.DropBalloonOrBomb());
            for (int i = 0; i < hitAndRun_Map.listBoats.Count; i++) {
                hitAndRun_Map.listBoats[i].twnScale_Balloon.DORestart();
            }
        }
    }
    public override void EndGame() {
        if (!isGameplay_End) {
            isGameplay_End = true;
            Timer.Schedule(this, 3, () => {
                if (SceneManager.ins.popup_EndGame == null) {
                    SceneManager.ins.ShowPopup_EndGame(false);
                }
            });
        }
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < nodes_Start.Length; i++) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(nodes_Start [i].position, 3f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(nodes_Start [i].position, new Vector3(nodes_Start [i].position.x , nodes_Start [i].position.y, nodes_Start [i].position.z - 999));
        }
    }
}
