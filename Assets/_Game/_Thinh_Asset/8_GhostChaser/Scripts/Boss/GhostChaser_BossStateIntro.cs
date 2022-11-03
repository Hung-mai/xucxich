using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostChaser_BossStateIntro : GhostChaser_BossStateBase
{
    private bool isCheck = false;
    public override void EnterState(GhostChaser_BossManager boss)
    {
        boss.state = GhostChaser_BossManager.State.Intro;
        _GhostChaser_Manager.ins.map.borderUp.SetActive(false);
        boss.anim.SetTrigger("IsChase");
        boss.navMeshAgent.SetDestination(boss.targetIntro);
        boss.navMeshAgent.stoppingDistance = 0;
    }
    public override void UpdateState(GhostChaser_BossManager boss)
    {
        if (Vector3.Distance(boss.transform.position,boss.targetIntro) <= 3 && !isCheck)
        {
            Debug.Log("Done Boss Intro");
            boss.navMeshAgent.stoppingDistance = 30;
            
            isCheck = true;
            boss.anim.SetTrigger("IsScream");
            //boss.transform.DOLookAt(Form_Gameplay.ins.player.transform.position,1f).SetEase(Ease.Linear);
            boss.transform.DORotate(new Vector3(0,180f,0),0.75f).SetEase(Ease.Linear).OnComplete(()=> {
                
            });
        }
    }
}
public class GhostChaser_bossStateGoTo2Side : GhostChaser_BossStateBase
{
    private bool isCheck = false;
    public override void EnterState(GhostChaser_BossManager boss)
    {
        boss.navMeshAgent.enabled = true;
        boss.state = GhostChaser_BossManager.State.GoSide;
        if (boss.targetIntro2 == null)
        {
            boss.SwitchState(boss.ChaseState);
            return;
        }
        boss.anim.SetTrigger(boss.bossAnimEvent.chase);
        boss.navMeshAgent.SetDestination(boss.targetIntro2);
        boss.navMeshAgent.stoppingDistance = 0;
    }

    public override void UpdateState(GhostChaser_BossManager boss)
    {
        if (boss.targetIntro2 == null) return;
        if (Vector3.Distance(boss.transform.position, boss.targetIntro2) <= 3 && !isCheck)
        {
            isCheck = true;
            boss.navMeshAgent.stoppingDistance = 30;
            boss.SwitchState(boss.ChaseState);
        }
    }
}
