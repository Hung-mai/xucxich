using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class GhostChaser_BossStateBase 
{
    public abstract void EnterState(GhostChaser_BossManager boss);

    public abstract void UpdateState(GhostChaser_BossManager boss);
}
