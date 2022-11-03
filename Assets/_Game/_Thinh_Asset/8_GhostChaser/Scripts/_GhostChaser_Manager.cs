using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class _GhostChaser_Manager : Minigame_Manager
{
    public static _GhostChaser_Manager ins;
    public float speedDamagedChar = 30;

    public List<Character> listCharAlive;

    [Header("---------ref----------")]
    public GhostChaser_BossManager[] boss;
    public _GhostChaser_Map map;
    public Transform[] nodeStartBoss;
    public Transform nodeStartOneBoss;
    public Transform[] nodeGoto2Side;

    private Vector3 pos_CameraWin = new Vector3(0,0.602f,-0.7f);
    private Vector3 angle_CameraWin = new Vector3(39,0,0);

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
            isGameplay_End = true;
            if (SceneManager.ins.popup_EndGame == null)
            {
                if (listChars[0].isAlive)
                {
                    listChars[0].tran_Rotate.eulerAngles = new Vector3(0, 180, 0);
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

    public override void InitGame()
    {
        listChars[0].username = DataManager.ins.gameSave.username;
        listChars[0].speed = speedChar;
        listChars[0].speedBack = speedDamagedChar;
        listChars[0].modelChar.collider_AttackChar.charAttacker = listChars[0];
        listChars[0].tran_Rotate.localEulerAngles = new Vector3(0, 180, 0);
        listChars [0].modelChar.WearSkinByGameSave(false);
        listCharAlive.Add(listChars[0]);

        List<int> listIDUsername = Enumerable.Range(0, GameManager.ins.listUsernames.Length - 1).ToList();
        listIDUsername.Shuffle();
        List<int> listIDColor = Enumerable.Range(0, GameManager.ins.listColorsChar.Length - 1).ToList();
        listIDColor.Remove(listChars [0].modelChar.idColor);
        listIDColor.Shuffle();
        listIDColor.Insert(0, listChars [0].modelChar.idColor);
        for (int i = 1; i < maxCharacter; i++)
        {
            if (i >= listChars.Count)
            {
                listChars.Add(Instantiate(prefab_AIBot));
            }
            listChars [i].idChar = i;
            listChars [i].transform.position = nodes_Start[i].position;
            listChars[i].transform.LookAt(listChars[i].AIBot.pos_TargetMove);
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
            listChars [i].username = GameManager.ins.listUsernames[listIDUsername[i - 1]];
            listChars[i].name = "AIBot_" + listChars[i].username;
            listChars[i].speed = speedChar * 0.95f;
            listChars[i].speedBack = speedDamagedChar;
            listChars[i].AIBot.amountRevive = amountAIReviveFree;

            listChars[i].AIBot.pos_TargetMove = listChars[i].transform.position;
            listCharAlive.Add(listChars[i]);
        }
        for (int i = 0; i < listChars.Count; i++)
        {
            if (i >= listIndicators.Count)
            {
                listIndicators.Add(Instantiate(prefab_Indicator, SceneManager.ins.canvas_Indicator.transform));
            }
            listIndicators[i].setUpIndicator(listChars[i]);
        }
        //camera xử lý sau
        camFollower.ins.transform.localPosition = camFollower.ins.localPositionIntroGame;
        camFollower.ins.transform.localEulerAngles = camFollower.ins.angleIntroGame;
        camFollower.ins.localPositionWinGame = pos_CameraWin;
        camFollower.ins.angleWinGame = angle_CameraWin;
        Timer.Schedule(this, 0.5f, () => {
            //thuc hien truoc khi camera chay ve startgame
            map.OpenDoor();
        });
        if (boss.Length > 1)
        {
            for (int i = 0; i < boss.Length; i++)
            {
                boss[i].targetIntro = nodeStartBoss[i].position;
                boss[i].targetIntro2 = nodeGoto2Side[i].position;
                //when b gooutside done => cam action
                boss[i].SwitchState(boss[i].IntroState);
            }
        }
        else if (boss.Length == 1)
        {
            boss[0].targetIntro = nodeStartOneBoss.position;
            boss[0].SwitchState(boss[0].IntroState);
        }
        StartCoroutine(I_Intro_WaitBossGoOutsideDone());

        
        
        
    }
    IEnumerator I_Intro_WaitBossGoOutsideDone()
    {
        foreach (GhostChaser_BossManager b in boss)
        {
            yield return new WaitUntil(()=>b.isIntroDone);
        }
        map.borderUp.SetActive(true);
        map.CloseDoor();
        camFollower.ins.state = 1;
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
            //Form_Gameplay.ins.timeCur = 9999999;
            listChars[0].modelChar.sprite_ArrowForward.gameObject.SetActive(true);


            //bossController.StartAttack();
            foreach (GhostChaser_BossManager b in boss)
            {
                //b.StartGame();
                b.SwitchState(b.GoSideState);
            }
            
        }
    }
    public void RemoveCharFromListAlive(Character character)
    {
        if (!listCharAlive.Contains(character)) return;
        listCharAlive.Remove(character);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < nodes_Start.Length; i++)
        {
            Gizmos.DrawWireSphere(nodes_Start[i].position, 3f);
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < nodeStartBoss.Length; i++)
        {
            Gizmos.DrawWireSphere(nodeStartBoss[i].position,4f);
        }
        Gizmos.DrawWireSphere(nodeStartOneBoss.position,4f);
        Gizmos.color = Color.green;
        for (int i = 0; i < nodeGoto2Side.Length; i++)
        {
            Gizmos.DrawWireSphere(nodeGoto2Side[i].position, 4f);
        }
    }
}
