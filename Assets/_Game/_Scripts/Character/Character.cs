using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Character : MonoBehaviour{
    public int idChar = 0;
    public float scale;
    public int level = 0;
    public string username;
    public bool isAlive = true;
    public bool isVelocityDamaged = false;
    public bool isDamaged = false;
    public bool isFinish = false;
    public bool isAnimationDyning = false;
    public bool isBooster = false;
    public float timeUndying = 0;
    public float timeRunToWin = 1;
    public int idCheckpoint = -1;
    public int top = 100;
    public Char_State state_New = Char_State.Idle;
    public Char_State state_Apply = Char_State.None;
    public ColliderKillChar colliderKillChar = ColliderKillChar.None;

    public float speed;
    public float speedBack;
    public Transform tran_Rotate;
    public Collider myCollider;
    public Rigidbody myRigidbody;
    public ModelChar modelChar;
    public GameObject obj_Shadow;
    public SoundObject sound_DieWater;
    //public AudioSource audio_Die;
    public Transform tran_IndicatorTarget;
    public GameObject effectBlood;
    public ParticleSystem effect_SpeedUp;
    public List<Slither_Food> listFoods = new List<Slither_Food>();//Slither
    public List<Vector3> listPosTail = new List<Vector3>();//Slither
    [HideInInspector] public Indicator indicator;
    [HideInInspector] public Character charAttacker;

    [HideInInspector] public AIBot AIBot;
    [HideInInspector] public Player player;

    public virtual void Awake() {
        isAlive = true;
        state_New = Char_State.Idle;
        scale = transform.localScale.x;
    }

    public virtual void AniEvent_ApplyDamage() {//Bật Collider tấn công lên và nó sẽ tự disable sau 0.3s
        if(Form_Gameplay.ins.minigameManager.minigame != Minigame.WackyRun) modelChar.collider_AttackChar.gameObject.SetActive(true);
    }

    public virtual void AniEvent_EndAttack() {//Hết Ani Attack thì tự động chuyển về Ani Idle -> State cũng chuyển về Idle
        state_New = Char_State.Idle;
    }

    public void OnTriggerEnter_Damaged(Character attacker) {
        if (attacker.idChar != idChar && isAlive && !isDamaged && Form_Gameplay.ins.minigameManager.minigame != Minigame.HitAndRun) {
            charAttacker = attacker;
            state_New = Char_State.Damaged;
        }
    }

    public void OnTriggerEnter_Die(ColliderKillChar Collider) {
        if (isAlive) {
            isAlive = false;
            modelChar.effect_TrailBooster.gameObject.SetActive(false);
            if (Form_Gameplay.ins.minigameCur == Minigame.GhostChaser)//thinh 
            {
                _GhostChaser_Manager.ins.RemoveCharFromListAlive(this);
            } else if (Form_Gameplay.ins.minigameCur == Minigame.Slither) {
                //Disconect toàn bộ Food
                for (int i = 0; i < listFoods.Count; i++) {
                    listFoods [i].DisconnectToChar();
                }
                listFoods.Clear();
            }
        }
        colliderKillChar = Collider;
        switch (Collider) {
            case ColliderKillChar.None:
                break;
            case ColliderKillChar.HorizontalSlice:
                break;
            case ColliderKillChar.VerticalSlice:
                break;
            case ColliderKillChar.Smash:
                break;
            case ColliderKillChar.Water:
                break;
            case ColliderKillChar.Bomb:
                break;
        }
    }

    public void OnTriggerEnter_Checkpoint(int idCheckpoint) {
        this.idCheckpoint = idCheckpoint;
        if (AIBot != null && Form_Gameplay.ins.minigameManager.minigame == Minigame.SnakeBlock) {
            AIBot.pos_TargetMove = SnakeBlock_Manager.ins.listCheckpoint [Mathf.Clamp(idCheckpoint + 1, 0 , SnakeBlock_Manager.ins.listCheckpoint.Count -1)].transform.position;
        }
    }

    public void OnTriggerEnter_ChangeCamera(int idStateCamera) {
        camFollower.ins.state = idStateCamera;
    }

    public void OnTriggerEnter_Balloon() {
        HitAndRun_Manager.ins.hitAndRun_Map.listBoats [idChar].BalloonBurst_MoveBoat(idChar == 0);
    }

    public void OnTriggerEnter_Food(Slither_Food food) {
        //Tăng số lượng đuôi
        food.ConnectToChar(this);
    }

    public void OnTriggerEnter_Finish() {
        if (isAlive && !isFinish) {
            isFinish = true;
            if(!Form_Gameplay.ins.minigameManager.listCharsWon.Contains(this)) {
                Form_Gameplay.ins.minigameManager.hasCharFinishLine = true;
                Form_Gameplay.ins.minigameManager.listCharsWon.Add(this);
            }
        }
    }

    public IEnumerator Undying() {
        if (timeUndying >0) {
            yield return new WaitForSeconds(0.12f);
            modelChar.obj_Animation.SetActive(false);
            obj_Shadow.SetActive(false);
            yield return new WaitForSeconds(0.12f);
            modelChar.obj_Animation.SetActive(true);
            obj_Shadow.SetActive(true);
            timeUndying -= 0.24f;
            if (timeUndying > 0) StartCoroutine(Undying());
        }
    }

    public abstract void Main();
    public void Reborn() {
        isAlive = true;
        isAnimationDyning = false;
        isDamaged = false;
        isVelocityDamaged = false;
        if (AIBot != null) AIBot.amountRevive--;
        indicator.OnReborn();
        state_New = Char_State.Idle;
        ReloadAnimation();
        timeUndying = 1.25f;
        Timer.Schedule(this, 0.5f, () => {
            if (isBooster) modelChar.effect_TrailBooster.gameObject.SetActive(true);
        });
        if (Form_Gameplay.ins.minigameManager.minigame == Minigame.WackyRun) {
            timeUndying = 1.5f;
            if (idCheckpoint == -1) {
                transform.position = new Vector3(WackyRun_Manager.ins.nodes_Start[idChar].position.x, 10, WackyRun_Manager.ins.nodes_Start[idChar].position.z);
            } else {
                transform.position = new Vector3(WackyRun_Manager.ins.nodes_Start[idChar].position.x, 10, WackyRun_Manager.ins.listCheckpoint[idCheckpoint].transform.position.z);
            }
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.SnakeBlock) {
            if (idCheckpoint == -1) {
                transform.position = new Vector3(SnakeBlock_Manager.ins.nodes_Start[idChar].position.x + Random.Range(-20,20), 10, SnakeBlock_Manager.ins.nodes_Start[idChar].position.z+ Random.Range(-20, 20));
            } else {
                transform.position = new Vector3(SnakeBlock_Manager.ins.listCheckpoint[idCheckpoint].transform.position.x + Random.Range(-20, 20), 10, SnakeBlock_Manager.ins.listCheckpoint[idCheckpoint].transform.position.z+ Random.Range(-20, 20));
            }
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.SkewerScurry) {
            transform.position = new Vector3(SkewerScurry_Manager.ins.nodes_Start [idChar].position.x, 5, SkewerScurry_Manager.ins.nodes_Start [idChar].position.z);
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.GhostChaser) {
            transform.position = new Vector3(_GhostChaser_Manager.ins.nodes_Start [idChar].position.x, 5, _GhostChaser_Manager.ins.nodes_Start [idChar].position.z);
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.OnTheCuttingBoard) {
            transform.position = new Vector3(OnTheCuttingBoard_Manager.ins.nodes_Start [idChar].position.x, 5, OnTheCuttingBoard_Manager.ins.nodes_Start [idChar].position.z);
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.Slither) {
                transform.position = new Vector3(Slither_Manager.ins.nodes_Start [idChar].position.x, 10, Slither_Manager.ins.nodes_Start [idChar].position.z);
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.PlatformPush) {
            int orderBlockNotFall = 0;
            for (int i = 0; i < PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll.Count; i++) {
                if (!PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[i].isFlicker && !PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[i].isFell) {
                    orderBlockNotFall = i;
                    break;
                }
            }
            transform.position = new Vector3(PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[orderBlockNotFall].transform.position.x + Random.Range(-20, 20), 10, PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[orderBlockNotFall].transform.position.z + Random.Range(-20, 20));
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.HitAndRun) {
            timeUndying = -1;
            //transform.position = new Vector3(HitAndRun_Manager.ins.platformPush_Map.list_BlocksAll [orderBlockNotFall].transform.position.x, 10, PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll [orderBlockNotFall].transform.position.z);
        } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.SidestepSlope) {
            transform.position = new Vector3(transform.position.x, transform.position.y +15f, transform.position.z);
            /*if (AIBot != null) { 
                AIBot.idBlockTarget = 0;
                AIBot.pos_TargetMove = new Vector3(SidestepSlope_Manager.ins.nodes_Start [idChar].position.x *1.5f, SidestepSlope_Manager.ins.listTargetMove [SidestepSlope_Manager.ins.listChars [idChar].AIBot.idBlockTarget].transform.position.y, SidestepSlope_Manager.ins.listTargetMove [SidestepSlope_Manager.ins.listChars [idChar].AIBot.idBlockTarget].transform.position.z);
            }*/
            //transform.position = new Vector3(HitAndRun_Manager.ins.platformPush_Map.list_BlocksAll [orderBlockNotFall].transform.position.x, 10, PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll [orderBlockNotFall].transform.position.z);
        } else {
            transform.position = Vector3.zero;
        }
        if (colliderKillChar == ColliderKillChar.Water) {
            modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Drown], false);
        } else {
            myCollider.enabled = true;
            myRigidbody.isKinematic = false;
            obj_Shadow.SetActive(true);
            effectBlood.SetActive(false);
            modelChar.obj_Animation.gameObject.SetActive(true);
            modelChar.obj_Death.gameObject.SetActive(false);
        }
        colliderKillChar = ColliderKillChar.None;
        StartCoroutine(Undying());
    }
    public bool IsGrounded() {
        if (GameManager.ins.minigameManager.minigame == Minigame.PlatformPush && AIBot != null) {
            int orderBlockStand = PlatformPush_Manager.ins.platformPush_Map.FindIDBlockByPosition(transform.position);
            if(orderBlockStand >= 0 && PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[orderBlockStand].isFell) return false;
        }else if (GameManager.ins.minigameManager.minigame == Minigame.SidestepSlope) {
            return true;
        }
        RaycastHit hit;
        bool a=  Physics.Raycast(transform.position, -Vector3.up, out hit, 2);
        return hit.collider != null && !hit.collider.isTrigger;
    }
    public abstract void ReloadAnimation();
}