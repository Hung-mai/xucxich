using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GhostChaser_BossStateChase : GhostChaser_BossStateBase
{
    public override void EnterState(GhostChaser_BossManager boss)
    {
        boss.navMeshAgent.isStopped = false;
        boss.navMeshAgent.ResetPath();
        boss.state = GhostChaser_BossManager.State.Chase;
        boss.anim.SetTrigger(boss.bossAnimEvent.chase);
    }

    public override void UpdateState(GhostChaser_BossManager boss)
    {
        if (Form_Gameplay.ins.timeCur <= 0)
        {
            //boss.navMeshAgent.SetDestination(boss.transform.position) ;
            boss.SwitchState(boss.IdleState);
            return;
        }
        //cứ 3-5 s đổi mục tiêu 1 lần hoặc mục tiêu isAlive = false
        //if (!boss.target.isAlive || boss.currentTimeChangeTarget <= 0)
        //{
        //    boss.ChangeTarget();
        //    boss.currentTimeChangeTarget = boss.timeChangeTarget;
        //}
        boss.navMeshAgent.SetDestination(boss.target.transform.position);
        if (Vector3.Distance(boss.target.transform.position, boss.transform.position) <= boss.rangeAttack)
        {
            boss.SwitchState(boss.HitState);
            return;
        }
        #region switch target if any enemy is close
        if (_GhostChaser_Manager.ins.listCharAlive.Count < 1) return;
        for(int i = 0; i < _GhostChaser_Manager.ins.listCharAlive.Count; i++)
        {
            if (Vector3.Distance(_GhostChaser_Manager.ins.listCharAlive[i].transform.position,boss.transform.position) <= boss.rangeAttack)
            {
                boss.target = _GhostChaser_Manager.ins.listCharAlive[i];
                boss.SwitchState(boss.HitState);
            }
        }
        #endregion
    }
}
