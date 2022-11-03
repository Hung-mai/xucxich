using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnTheCuttingBoard_Manager : Minigame_Manager
{
    public static OnTheCuttingBoard_Manager ins;
    public float speedDamagedChar = 30;

    [Header("-----------ref-----------")]
    public Game9_BossController bossController;
    public OnTheCuttingBoard_Map mapManager;

    private Vector3 pos_CameraWin = new Vector3(0, 0.53f, -0.5f);
    private Vector3 angle_CameraWin =  new Vector3(38.78f, 0,0);
    public override void Awake()
    {
        base.Awake();
        if (ins != null) Destroy(ins.gameObject);
        ins = this;
        GameManager.ins.minigameManager = this;
    }
    private void Start()
    {
        InitGame();
    }
    private void FixedUpdate()
    {
        if (SceneManager.ins.async != null) return;
        if (isGameplay_Start && !isGameplay_End)
        {
            for (int i = 0; i < listChars.Count; i++)
            {
                listChars[i].Main();
            }
        }
        else if (isGameplay_End)
        {
            for (int i = 0; i < listChars.Count; i++)
            {
                if (!listChars[i].isAlive)
                {
                    listChars[i].state_New = Char_State.Death;
                    listChars[i].ReloadAnimation();
                }
                else if (listChars[i].state_Apply != Char_State.Win)
                {
                    listChars[i].state_New = Char_State.Win;
                    listChars[i].ReloadAnimation();
                }
            }
        }
    }
    public override void EndGame()
    {
        if (!isGameplay_End)
        {
            bossController.StopAllCoroutines();
            isGameplay_End = true;
            if (SceneManager.ins.popup_EndGame == null)
            {
                if (listChars[0].isAlive)
                {
                    listChars[0].tran_Rotate.eulerAngles = new Vector3(0, 180, 0);
                    camFollower.ins.state = 4;
                    Form_Gameplay.ins.confetiFx.SetActive(true);
                }
                SceneManager.ins.ShowPopup_EndGame(listChars[0].isAlive, 3f);
            }
        } 
    }

    public override void InitGame()
    {
        listChars[0].username = DataManager.ins.gameSave.username;
        listChars[0].speed = speedChar;
        listChars[0].speedBack = speedDamagedChar * 1.2f;
        listChars[0].modelChar.collider_AttackChar.charAttacker = listChars[0];
        listChars[0].tran_Rotate.localEulerAngles = new Vector3(0, 180, 0);
        listChars [0].modelChar.WearSkinByGameSave(false);
        List<int> listIDUsername = Enumerable.Range(0, GameManager.ins.listUsernames.Length - 1).ToList();
        listIDUsername.Shuffle();
        List<int> listIDColor = Enumerable.Range(0, GameManager.ins.listColorsChar.Length - 1).ToList();
        for (int i = 1; i < maxCharacter; i++)
        {
            if (i >= listChars.Count)
            {
                listChars.Add(Instantiate(prefab_AIBot));
            }
            listChars[i].transform.position = nodes_Start[i].position;
            listChars[i].transform.LookAt(listChars[i].AIBot.pos_TargetMove);
            //listChars [i].tran_Rotate.LookAt(listChars [i].AIBot.pos_TargetMove);
            listChars[i].idChar = i;
            listChars [i].modelChar.idSkin_Full = -1;
            listChars [i].modelChar.idSkin_Hand = Random.Range(0, GameManager.ins.arrayDataSkin_Hand.Length);
            listChars [i].modelChar.idSkin_Hair = Random.Range(0, GameManager.ins.arrayDataSkin_Hair.Length);
            listChars [i].modelChar.idSkin_Body = i;
            listChars [i].modelChar.WearSkinByIDCur(false);
            listChars[i].modelChar.collider_AttackChar.charAttacker = listChars[i];
            listChars [i].username = GameManager.ins.listUsernames[listIDUsername[i - 1]];
            listChars[i].name = "AIBot_" + listChars[i].username;
            listChars[i].speed = speedChar * 0.9f;
            listChars[i].speedBack = speedDamagedChar * 1.2f;
            listChars[i].AIBot.amountRevive = amountAIReviveFree;

            listChars[i].AIBot.pos_TargetMove = listChars[i].transform.position;
        }
        for (int i = 0; i < listChars.Count; i++)
        {
            if (i >= listIndicators.Count)
            {
                listIndicators.Add(Instantiate(prefab_Indicator, SceneManager.ins.canvas_Indicator.transform));
            }
            listIndicators[i].setUpIndicator(listChars[i]);
        }
        //camera
        camFollower.ins.transform.localPosition = camFollower.ins.localPositionIntroGame;
        camFollower.ins.transform.localEulerAngles = camFollower.ins.angleIntroGame;
        camFollower.ins.localPositionWinGame = pos_CameraWin;
        camFollower.ins.angleWinGame = angle_CameraWin;
        //bossController.anim.SetBool("IsSpawn",true);
        bossController.anim.Play("Spawn",0);
        bossController.anim.Play("Spawn", 1);
        bossController.anim.Play("Spawn", 2);
        Timer.Schedule(this, 8f, () => {
            camFollower.ins.state = 1;
        });
        camFollower.ins.actionStartGame = () => {
            isGameplay_Intro = true;
            Form_Gameplay.ins.UIIntroGame();
        };
    }

    public override void StartGame()
    {
        if (!isGameplay_Start)
        {
            isGameplay_Start = true;
            Form_Gameplay.ins.UIStartGame();
            //Form_Gameplay.ins.timeCur = 9999999;//cheat time
            listChars[0].modelChar.sprite_ArrowForward.gameObject.SetActive(true);
            //mapManager.StartActiveTraps();
            bossController.StartAttack();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < nodes_Start.Length; i++)
        {
            Gizmos.DrawWireSphere(nodes_Start[i].position, 3f);
        }
    }
}
