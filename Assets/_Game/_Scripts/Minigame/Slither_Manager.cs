using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slither_Manager : Minigame_Manager {
    public static Slither_Manager ins;

    public float speedDamagedChar = 30;

    [Header("--------------- ref ---------------")]
    public Slither_Map slither_Map;

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
                listChars [i].Main();
                //Nếu đủ khoảng cách thì thêm Position Old
                if(listChars[i].listPosTail.Count == 0 || (listChars[i].transform.position - listChars[i].listPosTail[0]).sqrMagnitude >= ( i== 0 ? slither_Map.distanceSmoothMove : slither_Map.distanceSmoothMove *0.39f)) {
                    listChars[i].listPosTail.Insert(0, listChars[i].transform.position);
                    if(listChars[i].listPosTail.Count > slither_Map.distanceFoodsTail_ByAmountSmoothMove * (listChars[i].listFoods.Count + 5)) {//Xóa các điểm cũ những vẫn để thừa ra 5 điểm
                        listChars[i].listPosTail.RemoveAt(listChars[i].listPosTail.Count - 1);
                    }
                }
                //Di chuyển các food theo sau
                for (int j = 0; j < listChars[i].listFoods.Count; j++) {
                    int idPosTail = Mathf.Clamp((j + 1) * (listChars [i].isBooster ? slither_Map.distanceFoodsTail_ByAmountSmoothMove - 2 : slither_Map.distanceFoodsTail_ByAmountSmoothMove), 0, listChars[i].listPosTail.Count - 1);
                    if (listChars[i].listFoods[j].isMoveFollowChar) {
                            listChars[i].listFoods[j].transform.position = listChars[i].listPosTail[idPosTail];
                        } else {
                            listChars[i].listFoods[j].transform.position = Vector3.MoveTowards(listChars[i].listFoods[j].transform.position, listChars[i].listPosTail[idPosTail], 250*Time.fixedDeltaTime);
                        }
                    //if ((listChars[i].listFoods[j].transform.position - listChars[i].listPosTail[(j + 1) * slither_Map.smoothMove]).sqrMagnitude > 0.01f) 
                    if (listChars [i].listPosTail.Count > 0) { 
                            listChars [i].listFoods [j].transform.LookAt( listChars [i].listPosTail [Mathf.Clamp( idPosTail - 1, 0, listChars [i].listPosTail.Count-1)] );
                    } 
                }
            }
            //Tính điểm để xếp hạng
            List<Character> listCharByTop = listChars.OrderByDescending(c => c.listFoods.Count).ToList();
            int top = 0;
            for (int i = 0; i < listCharByTop.Count; i++) {
                if(listCharByTop[i].isAlive && listCharByTop [i].listFoods.Count > 0) { 
                    listCharByTop [i].top = top;
                    top++;
                } else {
                    listCharByTop [i].top = 100;
                }
            }
        } else if (isGameplay_End) {
            for (int i = 0; i < listChars.Count; i++) {
                if (!listChars[i].isAlive) {
                    listChars[i].state_New = Char_State.Death;
                    listChars[i].ReloadAnimation();
                } else if (listChars[i].state_Apply != Char_State.Win ) {
                    listChars[i].state_New = Char_State.Win;
                    listChars[i].ReloadAnimation();
                }
            }
        }
    }

    public override void InitGame() {
        listChars [0].username = DataManager.ins.gameSave.username;
        listChars[0].speed = speedChar;
        listChars[0].speedBack = speedDamagedChar;
        listChars [0].modelChar.collider_AttackChar.charAttacker = listChars [0];
        listChars [0].tran_Rotate.localEulerAngles = new Vector3(0,180,0);
        listChars [0].modelChar.WearSkinByGameSave(false);
        List<int> listIDUsername = Enumerable.Range(0, GameManager.ins.listUsernames.Length - 1).ToList();
        listIDUsername.Shuffle();
        List<int> listIDColor = Enumerable.Range(0, GameManager.ins.listColorsChar.Length - 1).ToList();
        listIDColor.Remove(listChars [0].modelChar.idColor);
        listIDColor.Shuffle();
        listIDColor.Insert(0, listChars [0].modelChar.idColor);
        for (int i = 1; i < maxCharacter; i++) {
            if (i >= listChars.Count) {
                listChars.Add(Instantiate(prefab_AIBot));
            }
            listChars [i].idChar = i;
            listChars [i].transform.position = nodes_Start [i].position;
            listChars[i].AIBot.TargetFoodNext();
            listChars [i].transform.LookAt(listChars [i].AIBot.pos_TargetMove);
            //listChars [i].tran_Rotate.LookAt(listChars [i].AIBot.pos_TargetMove);
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
            listChars[i].modelChar.collider_AttackChar.charAttacker = listChars[i];
            listChars [i].username = GameManager.ins.listUsernames [listIDUsername [i - 1]];
            listChars [i].name = "AIBot_" + listChars [i].username;
            listChars[i].speed = speedChar*1.05f;
            listChars[i].speedBack = speedDamagedChar;
            listChars [i].AIBot.amountRevive = amountAIReviveFree;
        }
        for (int i = 0; i < listChars.Count; i++) {
            if (i >= listIndicators.Count) {
                listIndicators.Add(Instantiate(prefab_Indicator, SceneManager.ins.canvas_Indicator.transform));
            }
            listIndicators[i].setUpIndicator(listChars[i]);
        }
        for (int i = 0; i < slither_Map.listFoodsAll.Count; i++) {
            slither_Map.listFoodsAll[i].mesh.material.color = slither_Map.colorFoodBase;
        }
        camFollower.ins.transform.localPosition = camFollower.ins.localPositionIntroGame;
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
            listChars [0].modelChar.sprite_ArrowForward.gameObject.SetActive(true);
            slither_Map.StartSpawn();
        }
    }
    public override void EndGame() {
        if (!isGameplay_End) {
            isGameplay_End = true;
            if (SceneManager.ins.popup_EndGame == null) {
                if (listChars [0].isAlive) { 
                    listChars [0].tran_Rotate.eulerAngles = new Vector3(0, 180, 0);
                    camFollower.ins.state = 4;
                    Form_Gameplay.ins.confetiFx.SetActive(true);
                }
                Timer.Schedule(this, 3, () => {
                    if (SceneManager.ins.popup_EndGame == null) {
                        SceneManager.ins.ShowPopup_EndGame(listChars [0].isAlive);
                    }
                });
            }
        }
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (int i = 0; i < nodes_Start.Length; i++) {
            Gizmos.DrawWireSphere(nodes_Start[i].position, 3f);
        }
    }
}
