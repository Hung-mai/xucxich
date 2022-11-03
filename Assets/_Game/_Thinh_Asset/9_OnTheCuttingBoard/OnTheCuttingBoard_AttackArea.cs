using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RootMotion.FinalIK;

public class OnTheCuttingBoard_AttackArea : MonoBehaviour
{
    public GameObject DangerAlert;

    public ParticleSystem smokeGroundImpact;

    public InteractionObject interactObj;
    public InteractionTarget interactTarget_Left;
    public InteractionTarget interactTarget_Right;

    public Collider colliderTriggerKill;

    public bool isLeft = false;
    [SerializeField]private float xScale = 0.9f;
    [SerializeField]private float yScale = 0.9f;
    private void Awake()
    {
        if (!isLeft) DestroyImmediate(interactTarget_Left.gameObject);
        else DestroyImmediate(interactTarget_Right.gameObject);
    }
    public void Danger(bool isDanger)
    {
        if (isDanger)
        {
            DangerAlert.SetActive(true);
        }
        else
        {
            DangerAlert.SetActive(false);
        }
    }
    public void Effect()
    {
        smokeGroundImpact.Clear();
        smokeGroundImpact.Play();
    }
}
