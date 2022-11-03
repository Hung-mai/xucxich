using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChaser_BossStateTaken : GhostChaser_BossStateBase
{
    public override void EnterState(GhostChaser_BossManager boss)
    {
        boss.anim.SetTrigger(boss.bossAnimEvent.taken);
        boss.state = GhostChaser_BossManager.State.Taken;
        
    }

    public override void UpdateState(GhostChaser_BossManager boss)
    {
        
    }
}
