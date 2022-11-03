using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class _GhostChaser_Map : MonoBehaviour
{
    public Transform zoneNavMesh;
    public Transform door_L;
    public Transform door_R;
    public Transform door;
    public bool isDoor_reskin = false;
    //public bool isDoorOpen = false;

    [Header("--------border---------")]
    public GameObject borderUp;

    [SerializeField]private float PosOpen;
    [SerializeField]private float PosClose;
    [SerializeField] private float timeOpenClose = 0.7f;

    private float AngleOpen = 60;
    private float timeOpen = 0.5f;
    private float timeClose = 1.5f;
    private Ease openType = Ease.Linear;
    

    public void OpenDoor(Action OnComplete = null)
    {
        if (!isDoor_reskin)
        {
            door_L.DORotate(new Vector3(0, AngleOpen, 0), timeOpen, RotateMode.Fast).SetEase(openType);
            door_R.DORotate(new Vector3(0, -AngleOpen, 0), timeOpen, RotateMode.Fast).SetEase(openType).OnComplete(() => {
                //isDoorOpen = true;
                OnComplete();
            });
        }
        else
        {
            door.DOLocalMoveY(PosOpen,timeOpenClose).OnComplete(()=> {
                OnComplete();
            });
        }
        
        
    }
    public void CloseDoor(Action OnComplete = null)
    {
        if (!isDoor_reskin)
        {
            door_L.DORotate(new Vector3(0, 180, 0), timeClose, RotateMode.Fast).SetEase(openType);
            door_R.DORotate(new Vector3(0, -180, 0), timeClose, RotateMode.Fast).SetEase(openType).OnComplete(() => {
                //isDoorOpen = false;
                OnComplete();
            });
        }
        else
        {
            door.DOLocalMoveY(PosClose, timeOpenClose).OnComplete(() => {
                OnComplete();
            });
        }
        
    }
}
