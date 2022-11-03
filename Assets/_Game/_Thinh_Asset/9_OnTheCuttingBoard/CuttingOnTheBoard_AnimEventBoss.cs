using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class CuttingOnTheBoard_AnimEventBoss : MonoBehaviour
{
    public Animator anim;
    public string handLeftLoading, handLeftAttack, handLeftEnd, handLeftToIdle;
    public string handRightLoading, handRightAttack, handRightEnd , handRightToIdle ;
    public bool isDoneAnAttack = false;
    public bool isOnRightHand = false;
    [Header("------animation rigging--------")]
    public Rig rig1;
    public Transform targetLeft;
    public Transform targetRight;
    public Transform rigHandLeft;
    public Transform rigHandRight;

    public Transform diemNeo_TayTrai;
    public Transform diemNeo_TayPhai;

    [Header("----------chi so-----------")]
    [SerializeField] private float timeHoldToAttack = 1f;
    [SerializeField] private float timeHoldToReverseAttack = 1f;
    [SerializeField] private float timeWaitToGetDoneAttack = 1f;
    [SerializeField]private float timeAnim_Attack = 0.22f;

    [SerializeField] private Vector3 leftOriginalPos; // dduaw tay cao
    [SerializeField] private Vector3 rightOriginalPos;

    [SerializeField] private Vector3 leftIdlePos; //đưa tay idle
    [SerializeField] private Vector3 rightIdlePos;

    [SerializeField] private float offSet_HandToFinger = 30;

    [Header("------------Collider kill char-------------")]
    public Collider rightHand;
    public Collider leftHand;

    #region dap_tay
    public void Dap_Tay(bool isRightHand)
    {
        isOnRightHand = isRightHand;
        isDoneAnAttack = false;

        leftIdlePos = rigHandLeft.position;
        rightIdlePos = rigHandRight.position;

        rig1.weight = 0;
        if (isRightHand)
        {//tay phải
            anim.SetTrigger(handRightLoading);//bắt đầu vươn tay
        }
        else
        {//tay trái
            anim.SetTrigger(handLeftLoading);
        }

    }
    public void OnEndLoadingHand(int isRightHand)//AnimEvent
    {
        StartCoroutine(I_WaitToAttack(isRightHand == 1 ? true : false));
    }
    IEnumerator I_WaitToAttack(bool isRightHand)
    {
        //set up rig
        //rig1.weight = 0;

        yield return new WaitForSeconds(timeHoldToAttack);


        if (isRightHand)
        {
            targetRight.position = rigHandRight.position; rightOriginalPos = rigHandRight.position;
            anim.SetTrigger(handRightAttack);//đánh xuống mặt đất
            //random 1 pos để đập xuống
            int i = Random.Range(0, OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight.Count);
            rig1.weight = 1;
            targetLeft.DOMove(diemNeo_TayTrai.position,0);
            targetRight.DOMove(new Vector3(OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight[i].transform.position.x,rightOriginalPos.y, OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight[i].transform.position.z - offSet_HandToFinger), timeAnim_Attack / 2).SetEase(Ease.Linear).OnComplete(()=> {
                targetRight.DOMove(OnTheCuttingBoard_Manager.ins.mapManager.areasAttackRight[i].transform.position + Vector3.forward * offSet_HandToFinger, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(()=> {
                    OnEndHandAttack(1);
                });
                
            });
            
        }
        else
        {
            targetLeft.position = rigHandLeft.position; leftOriginalPos = rigHandLeft.position;
            anim.SetTrigger(handLeftAttack);
            //random 1 pos để đập xuống
            int i = Random.Range(0, OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft.Count);
            rig1.weight = 1;
            targetRight.DOMove(diemNeo_TayPhai.position,0);
            targetLeft.DOMove(new Vector3(OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft[i].transform.position.x,leftOriginalPos.y, OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft[i].transform.position.z - offSet_HandToFinger),timeAnim_Attack /2).SetEase(Ease.Linear).OnComplete(()=> {
                targetLeft.DOMove(OnTheCuttingBoard_Manager.ins.mapManager.areasAttackLeft[i].transform.position + Vector3.forward * offSet_HandToFinger, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(()=> {
                    OnEndHandAttack(0);
                });
                
            });
            
        }
    }
    public void OnEndHandAttack(int isRightHand)
    {
        StartCoroutine(I_WaitTorReverseAttack(isRightHand == 1 ? true : false));
    }
    IEnumerator I_WaitTorReverseAttack(bool isRightHand)
    {
        yield return new WaitForSeconds(timeHoldToReverseAttack);
        if (isRightHand)
        {
            //targetRight.DOMove(rightOriginalPos, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(() => {
            //    targetRight.DOMove(rightIdlePos, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(() => {
            //        rig1.weight = 0;

            //        //anim.SetTrigger(handRightEnd);//thu tay lại
            //        anim.SetTrigger(handRightToIdle);//thu tay lại

            //        isDoneAnAttack = true;
            //    });
            //}); //reverse hand
            targetRight.DOMove(diemNeo_TayPhai.position, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(()=> {
                //rig1.weight = 0;

                //anim.SetTrigger(handRightEnd);//thu tay lại
                anim.SetTrigger(handRightToIdle);//thu tay lại

                isDoneAnAttack = true;
            });
        }
        else
        {
            //targetLeft.DOMove(leftOriginalPos, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(() => {
            //    targetLeft.DOMove(leftIdlePos, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(() => {
            //        rig1.weight = 0;

            //        //anim.SetTrigger(handLeftEnd);
            //        anim.SetTrigger(handLeftToIdle);

            //        isDoneAnAttack = true;
            //    });
            //}); //reverse hand
            targetLeft.DOMove(diemNeo_TayTrai.position, timeAnim_Attack).SetEase(Ease.Linear).OnComplete(()=> {
                //rig1.weight = 0;

                //anim.SetTrigger(handLeftEnd);
                anim.SetTrigger(handLeftToIdle);

                isDoneAnAttack = true;
            });
        }
        


    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawWireSphere(targetLeft.position,5f);
        Gizmos.DrawWireSphere(targetRight.position, 5f);

        //vẽ điểm neo
        Gizmos.DrawWireSphere(diemNeo_TayPhai.position, 5f);
        Gizmos.DrawWireSphere(diemNeo_TayTrai.position, 5f);

        
    }
}
