using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GhostChaser_BossStateIdle : GhostChaser_BossStateBase
{
    public override void EnterState(GhostChaser_BossManager boss)
    {
        boss.navMeshAgent.isStopped = true;
        boss.navMeshAgent.ResetPath();
        boss.state = GhostChaser_BossManager.State.Idle;
        boss.anim.SetTrigger(boss.bossAnimEvent.idle);
    }

    public override void UpdateState(GhostChaser_BossManager boss)
    {
        
    }
}
