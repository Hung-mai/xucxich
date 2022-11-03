using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WackyRun_Manager : Minigame_Manager {
    public static WackyRun_Manager ins;

    [Header("--------------- ref ---------------")]
    public Vector3 pos_CameraIntro;
    public List<Collider_Checkpoint> listCheckpoint;
    public List<Collider_ChangeCamera> listChangeCameras;

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
                if (listChars[i].isFinish && listChars[i].state_Apply != Char_State.Win) {
                    listChars[i].timeRunToWin -= Time.fixedDeltaTime;
                    if (listChars[i].timeRunToWin < 0) {
                        listChars[i].tran_Rotate.eulerAngles = new Vector3(0,180,0);
                        listChars[i].state_New = Char_State.Win;
                        if (i == 0) camFollower.ins.state = 4;
                        if (i == 0 && SceneManager.ins.popup_EndGame == null) {
                            SceneManager.ins.ShowPopup_EndGame(true, 3);
                        }
                        listChars[i].ReloadAnimation();
                    } else {
                        if (i == 0) Form_Gameplay.ins.confetiFx.SetActive(true);
                        listChars [i].state_New = Char_State.Run;
                        listChars[i].ReloadAnimation();
                    }
                } else {
                    listChars[i].Main();
                }
            }
            //Tính điểm để xếp hạng khi chưa có ai về đích
            if (hasCharFinishLine == false && Form_Gameplay.ins.timeCur < Constants.TIME_GAMEPLAY - 3) {
                List<Character> listCharByTop = listChars.OrderByDescending(c => c.transform.position.z).ToList();
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
                if (listChars[i].isFinish && listChars[i].state_Apply != Char_State.Win) {
                    listChars[i].timeRunToWin -= Time.fixedDeltaTime;
                    if (listChars[i].timeRunToWin < 0) {
                        listChars[i].tran_Rotate.eulerAngles = new Vector3(0, 180, 0);
                        listChars[i].state_New = Char_State.Win;
                        if (i == 0) camFollower.ins.state = 4;
                        if (i == 0 && SceneManager.ins.popup_EndGame == null) {
                            SceneManager.ins.ShowPopup_EndGame(true, 3);
                        }
                        listChars[i].ReloadAnimation();
                    } else {
                        if (i == 0) Form_Gameplay.ins.confetiFx.SetActive(true);
                        listChars [i].state_New = Char_State.Run;
                        listChars[i].ReloadAnimation();
                    }
                } else if (!listChars[i].isAlive) {
                    listChars[i].state_New = Char_State.Death;
                    listChars[i].ReloadAnimation();
                } else if (listChars[i].isFinish) {
                    if (listChars[i].state_Apply != Char_State.Win) {
                        listChars[i].state_New = Char_State.Win;
                        listChars[i].ReloadAnimation();
                    }
                } else {
                    if (listChars[i].state_Apply != Char_State.Lose) {
                        listChars[i].state_New = Char_State.Lose;
                        listChars[i].ReloadAnimation();
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
        for (int i = 0; i < listCheckpoint.Count; i++) {
            listCheckpoint [i].idCheckpoint = i;
        }
        for (int i = 1; i < maxCharacter; i++) {
            if (i >= listChars.Count) {
                listChars.Add(Instantiate(prefab_AIBot));
            }
            listChars [i].idChar = i;
            listChars [i].transform.position = nodes_Start [i].position;
            listChars [i].AIBot.pos_TargetMove = new Vector3(nodes_Start [i].position.x, listCheckpoint [listCheckpoint.Count -1].transform.position.y, listCheckpoint [listCheckpoint.Count -1].transform.position.z);
            //listChars[i].tran_Rotate.LookAt(listChars[i].AIBot.pos_TargetMove);
            if (Random.Range(0, 1000) < Mathf.Min( DataManager.ins.gameSave.levelEnded*10, 30) && DataManager.ins.list_IDFull_ByColor[listIDColor[i]].Count > 0) {
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
            listChars[i].modelChar.collider_AttackChar.charAttacker = listChars[i];
            listChars [i].username = GameManager.ins.listUsernames[listIDUsername[i - 1]];
            listChars[i].name = "AIBot_" + listChars[i].username;
            listChars[i].speed = speedChar*0.95f;
            listChars[i].AIBot.amountRevive = amountAIReviveFree;
            listChars[i].timeRunToWin = Random.Range(0.8f, 1.2f);
        }
        for (int i = 0; i < listChars.Count; i++) {
            if (i >= listIndicators.Count) {
                listIndicators.Add(Instantiate(prefab_Indicator, SceneManager.ins.canvas_Indicator.transform));
            }
            //listChars[i].tran_IndicatorTarget.localPosition -= Vector3.up*0.03f;
            listIndicators[i].setUpIndicator(listChars[i]);
        }
        
        camFollower.ins.transform.localPosition = pos_CameraIntro;
        camFollower.ins.transform.localEulerAngles = camFollower.ins.angleIntroGame;
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
            for (int j = 0; j < listCheckpoint.Count; j++) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(new Vector3(nodes_Start [i].position.x, listCheckpoint [j].transform.position.y, listCheckpoint [j].transform.position.z), 3f);
                Gizmos.color = Color.green;
                if (j == 0) {
                    Gizmos.DrawLine(nodes_Start [i].position, new Vector3(nodes_Start [i].position.x, listCheckpoint [j].transform.position.y, listCheckpoint [j].transform.position.z));
                } else {
                    Gizmos.DrawLine(new Vector3(nodes_Start [i].position.x, listCheckpoint [j-1].transform.position.y, listCheckpoint [j-1].transform.position.z), new Vector3(nodes_Start [i].position.x, listCheckpoint [j].transform.position.y, listCheckpoint [j].transform.position.z));
                }
            }
        }
        Gizmos.color = Color.gray;
        for (int i = 0; i < listChangeCameras.Count; i++) {
            Gizmos.DrawWireCube(listChangeCameras [i].transform.position, new Vector3(150,100,25));
        }
    }
}
