using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Game9_BossController : MonoBehaviour
{
    [System.Serializable]
    class SkinBoss
    {
        public BossSkin skinId;
        public OnTheCuttingBoard_RefBoss refBoss;
        public InteractionTarget leftHand;
        public InteractionTarget rightHand;
    }
    public enum BossSkin
    {
        Mommy,
        Slender,
    }
    [Header("--------Change Skin-------")]
    public BossSkin skin = BossSkin.Mommy;
    [SerializeField]private SkinBoss[] bossSkinDatas;


    [Header("--------Atributte------")]

    public BossState state = BossState.Idle;
    public FullBodyBipedIK fullBodyIK;
    public InteractionSystem interactionSystem;
    public FullBodyBipedEffector[] effectors;


    [Header("---------animation----------")]
    public Animator anim;
    
    
    public string HandUpLeft, Idle, HandUpRight, EndHandUpRight, EndHandUpLeft;

    [Header("-----------ref------------")]
    public OnTheCuttingBoard_BossAnimEvent BossAnimEvent;

    [Header("----------colliderKill---------- Không dùng nữa" +
        "dùng trigger kill ở area cho chuẩn xác")]
    public Collider rightHand;
    public Collider leftHand;

    //check
    private bool isLeft = false;
    private bool isDoneHandUp = false;
    private bool isTouchGround = false;

    private float TimeWaitToIdle = 0.5f;
    [SerializeField] private float spdSmash = 1.5f;
    [SerializeField]private float TIME_HOLD_DOWN = 0.3f;
    [SerializeField] private float TIME_WAIT = 1f; //t.g fai du de reset animation
    [SerializeField] private float TIME_HOLD_UP = 0.75f;
    [SerializeField] private float TIME_OFF_TRIGGER = 0.2f;
    [SerializeField] private float ChanceTargetPlayer = 50;

    private Vector3 currentEuler;

    [Header("----------target visual--------NONE")]
    [SerializeField]private InteractionObject target;
    [SerializeField] private OnTheCuttingBoard_AttackArea targetArea;

    [SerializeField] private bool isToolInteraction = false;
    [SerializeField] private bool isRightTest = false;
    
    private void ChangeSkin()
    {
        if (anim == null)
        {
            foreach (SkinBoss sb in bossSkinDatas)
            {
                if (sb.skinId == skin)
                {
                    OnTheCuttingBoard_RefBoss b = Instantiate(sb.refBoss, transform);
                    b.transform.localPosition = new Vector3(5.4000001f, -2.79999995f, -23.5f);
                    b.transform.localEulerAngles = new Vector3(0, 331.961975f, 0);
                    b.transform.localScale = Vector3.one * 120;

                    //sign atributte
                    fullBodyIK = b.fullBodyIK;
                    interactionSystem = b.interactionSys;
                    anim = b.anim;
                    BossAnimEvent = b.animEvent;
                }
            }
            
        }
        else
        {
            if (anim.GetComponent<OnTheCuttingBoard_RefBoss>().bossId != skin)
            {
                foreach (SkinBoss sb in bossSkinDatas)
                {
                    if (sb.skinId == skin)
                    {
                        DestroyImmediate(anim.gameObject);

                        OnTheCuttingBoard_RefBoss b = Instantiate(sb.refBoss, transform);
                        b.transform.localPosition = new Vector3(5.4000001f, -2.79999995f, -23.5f);
                        b.transform.localEulerAngles = new Vector3(0,0, 0);
                        b.transform.localScale = Vector3.one * 120;

                        //sign atributte
                        fullBodyIK = b.fullBodyIK;
                        interactionSystem = b.interactionSys;
                        anim = b.anim;
                        BossAnimEvent = b.animEvent;
                    }
                }
            }
        }

        switch (skin)
        {
            case BossSkin.Slender:
                foreach (SkinBoss s in bossSkinDatas)
                {
                    if (s.skinId == BossSkin.Slender)
                    {
                        ChangeInteractTransformLeft(s.leftHand);
                        ChangeInteractTransformRight(s.rightHand);
                    }
                }
                break;
        } 
    }
    private void ChangeInteractTransformLeft(InteractionTarget target)
    {
        foreach (OnTheCuttingBoard_AttackArea a in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft)
        {
            DestroyImmediate(a.interactTarget_Left.gameObject);
            a.interactTarget_Left = Instantiate(target,a.interactObj.transform);
        }
    }
    private void ChangeInteractTransformRight(InteractionTarget target)
    {
        foreach (OnTheCuttingBoard_AttackArea a in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight)
        {
            DestroyImmediate(a.interactTarget_Right.gameObject);
            a.interactTarget_Right = Instantiate(target, a.interactObj.transform);
        }
    }
    private void Awake()
    {
        ChangeSkin();

        BossAnimEvent.OnHitIntro = () => {
            CameraShake.ins.Shake();
        };
        //BossAnimEvent.OnStartHandUpLeft = () => {
        //    BossAnimEvent.transform.DOLocalRotate(Vector3.zero,0);
        //};
        BossAnimEvent.OnEndHandUp = () => {
            isDoneHandUp = true;
        };;
        
        currentEuler = anim.transform.localEulerAngles;
        interactionSystem.speed = spdSmash;
        interactionSystem.resetToDefaultsSpeed = spdSmash;
    }
    private void Start()
    {
        foreach (OnTheCuttingBoard_AttackArea e in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft)
        {//add event tay trai
            e.interactObj.events[0].unityEvent.AddListener(() => {
                targetArea.Effect();
                isTouchGround = true;
            });
            e.interactObj.events[0].messages[0].function = "OnEndAction";
            e.interactObj.events[0].messages[0].recipient = this.gameObject;
        }
        foreach (OnTheCuttingBoard_AttackArea e in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight)
        {//add event tay phai
            e.interactObj.events[0].unityEvent.AddListener(() => {
                targetArea.Effect();
                isTouchGround = true;
            });
            e.interactObj.events[0].messages[0].function = "OnEndAction";
            e.interactObj.events[0].messages[0].recipient = this.gameObject;
        }
    }
    private void OnEndAction()
    {//event cua hit khi cham area
        CameraShake.ins.Shake();
        //if (targetArea.isLeft)
        //{
        //    leftHand.enabled = true;
        //    leftHand.isTrigger = true;
        //}
        //else
        //{
        //    rightHand.enabled = true;
        //    rightHand.isTrigger = true;
        //}
        targetArea.colliderTriggerKill.gameObject.SetActive(true);
        StartCoroutine(I_OffTrigger());
    }
    IEnumerator I_OffTrigger()
    {
        yield return new WaitForSeconds(TIME_OFF_TRIGGER);
        //if (targetArea.isLeft)
        //{
        //    leftHand.isTrigger = false;
        //}
        //else
        //{
        //    rightHand.isTrigger = false;
        //}
        targetArea.colliderTriggerKill.gameObject.SetActive(false);
    }
    public void StartAttack()
    {
        StartCoroutine(I_WaitDoneAction());
    }
    IEnumerator I_WaitDoneAction()
    {
        while (true)
        {
            isDoneHandUp = false;
            isTouchGround = false;

            isLeft = !isLeft;
            #region calculate target
            if (isToolInteraction)
            {
                if (isRightTest)
                {
                    targetArea = OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight[0];
                    target = OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight[0].interactObj;
                }
                else
                {
                    targetArea = OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft[0];
                    target = OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft[0].interactObj;
                }
            }
            else CalculateTarget();
            #endregion

            if (targetArea.isLeft)
            {
                anim.SetTrigger(HandUpLeft);
            }
            else
            {
                anim.SetTrigger(HandUpRight);
            }
            yield return new WaitUntil(() => isDoneHandUp);
            //rightHand.enabled = false; leftHand.enabled = false;
            targetArea.Danger(true);

            yield return new WaitForSeconds(TIME_HOLD_UP);

            
            if (targetArea.isLeft) interactionSystem.StartInteraction(effectors[0], target, true);
            else interactionSystem.StartInteraction(effectors[1], target, true);

            yield return new WaitUntil(()=>isTouchGround);
            


            yield return new WaitForSeconds(TIME_HOLD_DOWN);

            if (targetArea.isLeft)
            {
                //BossAnimEvent.transform.DOLocalRotate(currentEuler,0).SetEase(Ease.Linear);
                anim.SetTrigger(EndHandUpLeft);
            }
            else
            {
                anim.SetTrigger(EndHandUpRight);
            }

            
            interactionSystem.ResumeAll();
            
            targetArea.Danger(false);

            
            yield return new WaitForSeconds(TIME_WAIT);
            

        }
    }

    private void CalculateTarget()
    {
        bool isTargetPlayer = false;
        float change = Random.Range(0,100);
        if (change <= ChanceTargetPlayer || !AnyBotAlive())
        {
            isTargetPlayer = true;
        }
        if (isTargetPlayer)
        {//player
            Transform player = OnTheCuttingBoard_Manager.ins.listChars[0].transform;
            float mindis = 9999f;
            
            foreach (OnTheCuttingBoard_AttackArea t in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft)
            {
                if (Vector3.Distance(player.position,t.transform.position) <= mindis)
                {
                    mindis = Vector3.Distance(player.position, t.transform.position);
                    targetArea = t;
                    target = t.interactObj;
                }
            }
            foreach (OnTheCuttingBoard_AttackArea t in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight)
            {
                if (Vector3.Distance(player.position, t.transform.position) <= mindis)
                {
                    mindis = Vector3.Distance(player.position, t.transform.position);
                    targetArea = t;
                    target = t.interactObj;
                }
            }

        }
        else
        {//bot
            Transform someBot = OnTheCuttingBoard_Manager.ins.listChars[Random.Range(1,OnTheCuttingBoard_Manager.ins.listChars.Count)].transform;
            float mindis = 9999f;

            foreach (OnTheCuttingBoard_AttackArea t in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft)
            {
                if (Vector3.Distance(someBot.position, t.transform.position) <= mindis)
                {
                    mindis = Vector3.Distance(someBot.position, t.transform.position);
                    targetArea = t;
                    target = t.interactObj;
                }
            }
            foreach (OnTheCuttingBoard_AttackArea t in OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight)
            {
                if (Vector3.Distance(someBot.position, t.transform.position) <= mindis)
                {
                    mindis = Vector3.Distance(someBot.position, t.transform.position);
                    targetArea = t;
                    target = t.interactObj;
                }
            }
        }
    }

    private bool AnyBotAlive()
    {
        foreach (Character c in OnTheCuttingBoard_Manager.ins.listChars)
        {
            if (c.isAlive)
            {
                return true;
            }
        }
        return false;
    }

    
    private void GetTargetAttackPosition(bool isLeft)
    {
        if (isLeft)
        {
            targetArea = OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft[Random.Range(0, OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft.Count)];
            target = targetArea.interactObj;
        }
        else
        {
            targetArea = OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight[Random.Range(0, OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight.Count)];
            target = targetArea.interactObj;
        }
    }
    
}
