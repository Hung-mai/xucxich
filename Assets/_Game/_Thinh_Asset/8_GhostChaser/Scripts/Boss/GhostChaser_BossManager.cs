using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class GhostChaser_BossManager : MonoBehaviour
{
    #region Boss Change Skin
    [System.Serializable]
    class BossSkin_Info
    {
        public BossSkin idSkin;
        public GameObject model;
    }
    public enum BossSkin
    {
        Huggy,
        HuggyPink,
        Bunny,
        Freddy,
        Bird,
        BirdOther,
    }
    [Header("-----Boss Skin-----")]
    [SerializeField]private BossSkin bossSkin = BossSkin.Huggy;
    [SerializeField]private BossSkin_Info[] RefBossSkins;
    [ContextMenu("Update Boss Skin")]
    private void UpdateBossSkin()
    {
        foreach (BossSkin_Info bi in RefBossSkins)
        {
            if (bi.idSkin == bossSkin)
            {
                if (bossAnimEvent != null && bossAnimEvent.bossSkin != bi.idSkin)
                {
                    Destroy(anim.gameObject);
                    GameObject obj = Instantiate(bi.model,Vector3.zero,Quaternion.Euler(0,0,0),transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localEulerAngles = Vector3.zero;
                    anim = obj.GetComponent<Animator>() ;
                    bossAnimEvent = anim.GetComponent<GhostChaser_BossAnimEvent>();
                    colHandAttack = bossAnimEvent.GetComponent<GhostChaser_BossAnimEvent>().colHandAttack;
                }
                else if(bossAnimEvent == null)
                {
                    GameObject obj = Instantiate(bi.model, Vector3.zero, Quaternion.Euler(0, 0, 0), transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localEulerAngles = Vector3.zero;
                    anim = obj.GetComponent<Animator>();
                    bossAnimEvent = anim.GetComponent<GhostChaser_BossAnimEvent>();
                    colHandAttack = bossAnimEvent.GetComponent<GhostChaser_BossAnimEvent>().colHandAttack;
                }
                else
                {
                    Debug.Log("Ok");
                }
            }
        }
    }
    
    #endregion
    

    public Transform effectPos;
    public enum State
    {
        Idle,
        Chase,
        Hit,
        Intro,
        GoSide,
        Taken,
    }
    [Header("---------state machine----------")]
    public State state;
    public GhostChaser_BossStateBase currentState;

    //khai bao state
    public GhostChaser_BossStateIdle IdleState = new GhostChaser_BossStateIdle();
    public GhostChaser_BossStateHit HitState = new GhostChaser_BossStateHit();
    public GhostChaser_BossStateChase ChaseState = new GhostChaser_BossStateChase();
    public GhostChaser_BossStateIntro IntroState = new GhostChaser_BossStateIntro();
    public GhostChaser_bossStateGoTo2Side GoSideState = new GhostChaser_bossStateGoTo2Side();
    public GhostChaser_BossStateTaken TakenState = new GhostChaser_BossStateTaken();

    [Header("---------Attribute-------")]
    public NavMeshAgent navMeshAgent;
    public Animator anim;

    public Character target;
    public Character lastTarget;

    public Collider colHandAttack;

   
    public float timeChangeTarget = 5;
    [HideInInspector]public float currentTimeChangeTarget;
    [Header("range phát hiện của boss - range = khoảng đập tay")]
    public float rangeAttack ;
    [Header("thời gian boss quay vào target để đập - set càng thấp càng nhanh")]
    [SerializeField]internal float timeLookAtTarget = 0.2f;
    [Header("tốc độ đập")]
    [SerializeField] internal float spdAttack = 1.5f;

    [Header("--------Intro-------")]
    public bool isIntroDone = false;

    public float spdMove_Intro = 1;
    public Vector3 targetIntro;
    public Vector3 targetIntro2;

    [Header("-----------ref--------------")]
    public GhostChaser_BossAnimEvent bossAnimEvent;
    public float rangeDetectAI = 80;

    private void Awake()
    {
        UpdateBossSkin();
        bossAnimEvent.OnHit_AnimEvent = () =>
        {
            colHandAttack.enabled = true;
            Sut_Dat_ObjectPooler.instance.SpawnFromPool("SmokeEffect", effectPos.transform.position, Quaternion.Euler(-90,0,0),new Vector3(10,10,10)); 
        };
        bossAnimEvent.OnEndHit_AnimEvent = () =>
        {
            colHandAttack.enabled = false;
        };
        bossAnimEvent.OnEndAnimHit_AnimEvent = () =>
        {
            SwitchState(ChaseState);
            ChangeTargetToPlayer();
        };
        bossAnimEvent.OnEndScream_AnimEvent = () => {
            SwitchState(IdleState);
            isIntroDone = true;
        };
    }
    private void Start()
    {
        currentTimeChangeTarget = timeChangeTarget;
        //SwitchState(IdleState);
        target = Form_Gameplay.ins.player;
    }
    public void StartGame()
    {
        SwitchState(ChaseState);
    }

    internal void SwitchState(GhostChaser_BossStateBase state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    private void FixedUpdate()
    {
        currentTimeChangeTarget -= Time.fixedDeltaTime;
        currentState.UpdateState(this);

    }
    private void ChangeTargetToPlayer()
    {
        target = _GhostChaser_Manager.ins.listChars[0];
    }
    internal void ChangeTarget()
    {
        lastTarget = target;
        List<Character> listAlive = new List<Character>();
        foreach (Character c in _GhostChaser_Manager.ins.listChars)
        {
            if (c.isAlive) listAlive.Add(c);
        }
        if (listAlive.Count == 0) {
            SwitchState(ChaseState);
            return;
        }
        if (listAlive.Contains(target) && listAlive.Count > 1) listAlive.Remove(target);

        int i = 0;
        if (listAlive.Count > 1) i = UnityEngine.Random.Range(0, listAlive.Count);

        
            target = listAlive [i];

    }
    internal AnimationClip FindAnimation(Animator animator , string name)
    {
        foreach (AnimationClip a in anim.runtimeAnimatorController.animationClips)
        {
            if (a.name == name)
            {
                return a;
            }
        }
        return null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangeAttack);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,rangeDetectAI);
    }
}
