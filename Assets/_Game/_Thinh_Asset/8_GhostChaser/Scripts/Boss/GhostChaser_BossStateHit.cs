using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GhostChaser_BossStateHit : GhostChaser_BossStateBase 
{
    public override void EnterState(GhostChaser_BossManager boss)
    {
        boss.navMeshAgent.isStopped = true;
        boss.navMeshAgent.ResetPath();
        boss.state = GhostChaser_BossManager.State.Hit;
        boss.anim.SetFloat(boss.bossAnimEvent.spdAttack,boss.spdAttack);
        boss.transform.DOLookAt(boss.target.transform.position, boss.timeLookAtTarget).SetEase(Ease.Linear).OnComplete(() =>
        {
            boss.anim.SetTrigger(boss.bossAnimEvent.hit);
        });
    }

    public override void UpdateState(GhostChaser_BossManager boss)
    {
        //boss.transform.DOLookAt(boss.target.transform.position, Time.fixedDeltaTime * boss.spdRotate ).SetEase(Ease.Linear);
    }
    
}
