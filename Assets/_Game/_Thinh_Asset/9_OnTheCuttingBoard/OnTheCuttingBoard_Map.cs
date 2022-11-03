using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class OnTheCuttingBoard_Map : MonoBehaviour
{
    public List<OnTheCuttingBoard_AttackArea> areasAttackRight;
    public List<OnTheCuttingBoard_AttackArea> areasAttackLeft;

    public Transform Board;
    [Header("--------------ZoneAttack-----------")]
    public Vector3 LeftTop;
    public Vector3 rightBot;

    public float ZMax;
    public float ZMin;
    public float XMax;
    public float XMin;

    [Header("--------Diem neo tay idle----------")]
    public InteractionObject left_Point;
    public InteractionObject right_Point;

    private void Awake()
    {
        ZMax = LeftTop.z;
        XMin = LeftTop.x;

        ZMin = rightBot.z;
        XMax = rightBot.x;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (OnTheCuttingBoard_AttackArea a in areasAttackRight)
        {
            Gizmos.DrawWireSphere(a.transform.position,3f);
        }
        foreach (OnTheCuttingBoard_AttackArea a in areasAttackLeft)
        {
            Gizmos.DrawWireSphere(a.transform.position, 3f);
        }
    }
}
