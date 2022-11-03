using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkewerScurry_Manager : Minigame_Manager
{
    public static SkewerScurry_Manager ins;
    public GameObject canvasForm;//get camera
    public Camera cam;
    public float speedDamagedChar = 30;

    [Header("-----------ref----------")]
    public Vector3 pos_CameraWin;
    public Vector3 angle_CameraWin;
    public SkewerScurry_map mapManager;
    public override void Awake()
    {
        base.Awake();
        if (ins != null) Destroy(ins.gameObject);
        ins = this;
        GameManager.ins.minigameManager = this;
        canvasForm = SceneManager.ins.formCanvas.gameObject;
        cam = canvasForm.GetComponent<Canvas>().worldCamera;
    }
    private void Start()
    {
        InitGame();
    }
    public override void StartGame()
    {
        if (!isGameplay_Start)
        {
            isGameplay_Start = true;
            Form_Gameplay.ins.UIStartGame();
            listChars[0].modelChar.sprite_ArrowForward.gameObject.SetActive(true);
            //platformPush_Map.coroutine_FallBlock = platformPush_Map.StartCoroutine(platformPush_Map.FallBlock());
            mapManager.StartActiveTraps();
        }
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
    public override void InitGame()
    {
        listChars[0].username = DataManager.ins.gameSave.username;
        listChars[0].speed = speedChar;
        listChars[0].speedBack = speedDamagedChar ;
        listChars[0].modelChar.collider_AttackChar.charAttacker = listChars[0];
        listChars[0].tran_Rotate.localEulerAngles = new Vector3(0, 180, 0);
        listChars [0].modelChar.WearSkinByGameSave(false);
        List<int> listIDUsername = Enumerable.Range(0, GameManager.ins.listUsernames.Length - 1).ToList();
        listIDUsername.Shuffle();
        List<int> listIDColor = Enumerable.Range(0, GameManager.ins.listColorsChar.Length - 1).ToList();
        listIDColor.Remove(listChars [0].modelChar.idColor);
        listIDColor.Shuffle();
        listIDColor.Insert(0, listChars [0].modelChar.idColor);
        List<int> listPointIntelligence = Enumerable.Range(1, maxCharacter).ToList();
        listPointIntelligence.Shuffle();
        for (int i = 1; i < maxCharacter; i++)
        {
            if (i >= listChars.Count)
            {
                listChars.Add(Instantiate(prefab_AIBot));
            }
            listChars [i].idChar = i;
            listChars [i].transform.position = nodes_Start [i].position;
            listChars[i].transform.LookAt(listChars[i].AIBot.pos_TargetMove);
            listChars[i].AIBot.pos_TargetMove = listChars[i].transform.position;
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
            listChars[i].speed = speedChar;
            listChars[i].speedBack = speedDamagedChar ;
            listChars[i].AIBot.amountRevive = amountAIReviveFree;
            listChars[i].AIBot.pointIntelligence = pointIntelligence_StartMin + listPointIntelligence[i] * pointIntelligence_StartRange / listPointIntelligence.Count;
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
        Timer.Schedule(this, 1.5f, () => {
            camFollower.ins.state = 1;
        });
        camFollower.ins.actionStartGame = () => {
            isGameplay_Intro = true;
            Form_Gameplay.ins.UIIntroGame();
        };
    }
    public override void EndGame()
    {
        if (!isGameplay_End)
        {
            isGameplay_End = true;
            if (SceneManager.ins.popup_EndGame == null)
            {
                if (listChars [0].isAlive) {
                    listChars [0].tran_Rotate.eulerAngles = new Vector3(0, 180, 0);
                    camFollower.ins.transform.position = Camera.main.transform.position;
                    camFollower.ins.transform.eulerAngles = Camera.main.transform.eulerAngles;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < nodes_Start.Length; i++)
        {
            Gizmos.DrawWireSphere(nodes_Start[i].position, 3f);
        }
    }
    public float TimeScale;
    [ContextMenu("SetTime")]
    public void TimeScale1()
    {
        Time.timeScale = TimeScale;
    }
}
