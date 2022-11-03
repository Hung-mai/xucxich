using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class SkewerScurry_Trap : MonoBehaviour
{
    public bool isMoveX;
    public float targetX;
    public float targetZ;
    public bool isDoneActive = false;
    public bool isDoneDeactive = false;
    public bool isReady = false;
    [Header("-------------vi tri cua trap tren ma tran------------")]
    public int row = -1; //mac dinh chua duoc xep
    public int col = -1; //mac dinh chua duoc xep
    public SkewerScurry_TrapPos trapPos;
    [Header("---------Block Danger-----------")]
    public List<SkewerScurry_Block> BlocksDanger;
    public SkewerScurry_Block blockTarget;
    public SkewerScurry_Block obtacleBlock;

    [Header("------------------ref--------------------")]
    public GameObject trapObjectParent;
    public GameObject trapObject;
    public Collider colKill;
    private float OriginalPosXZ;
    private int directionMove;
    private float distanceCentertoBorderBlock = 34f;
    private float distanceMoveReady = 6f;
    private float distanceBackReady = 10f;
    //block trap co the cham den
    private float TIME_DELAY = 0.5f;
    private float TIME_DELAY_BEFORE_ACTIVE = 0.5f;
    private float SPD_ROTATE_BACK = -7;
    private float DIVINE_SPD_BACK = 4;

    private void Start()
    {

        //assign đc BlocksDanger và blockTarget tự động
        BlocksDanger.Clear();
        switch (trapPos)
        {
            case SkewerScurry_TrapPos.Top://top
                directionMove = -1;
                //duyet tat ca cac row
                //vao row do chi kiem tra cai index = col
                for (int i = 0; i < SkewerScurry_Manager.ins.mapManager.Rows.Count; i++)
                {//duyet 0 1 2 3
                    if (SkewerScurry_Manager.ins.mapManager.Rows[i].Blocks[col].isObtacle)
                    {
                        obtacleBlock = SkewerScurry_Manager.ins.mapManager.Rows[i].Blocks[col];
                        break;
                    }
                    else
                    {
                        blockTarget = SkewerScurry_Manager.ins.mapManager.Rows[i].Blocks[col];
                        BlocksDanger.Add(blockTarget);
                    }
                }
                break;
            case SkewerScurry_TrapPos.Down://down
                directionMove = 1;
                //duyet tat ca cac row
                //vao row do chi kiem tra cai index = col
                for (int i = SkewerScurry_Manager.ins.mapManager.Rows.Count - 1; i >= 0; i--)
                {//duyet 3 2 1 0
                    if (SkewerScurry_Manager.ins.mapManager.Rows[i].Blocks[col].isObtacle)
                    {
                        obtacleBlock = SkewerScurry_Manager.ins.mapManager.Rows[i].Blocks[col];
                        break;
                    }
                    else
                    {
                        blockTarget = SkewerScurry_Manager.ins.mapManager.Rows[i].Blocks[col];
                        BlocksDanger.Add(blockTarget);
                    }
                }
                break;
            case SkewerScurry_TrapPos.Left://left
                directionMove = 1;
                for (int i = 0; i < SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks.Count; i++)
                {
                    //duyet hang 0 1 2 3
                    if (SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks[i].isObtacle)
                    {
                        obtacleBlock = SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks[i];
                        break;
                    }
                    else
                    {
                        blockTarget = SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks[i];
                        BlocksDanger.Add(blockTarget);
                    } 
                }
                break;
            case SkewerScurry_TrapPos.Right://right
                directionMove = -1;
                for (int i = SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks.Count - 1; i >= 0; i--)
                {
                    //duyet hang 3 2 1 0
                    if (SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks[i].isObtacle)
                    {
                        obtacleBlock = SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks[i];
                        break;
                    }
                    else
                    {
                        blockTarget = SkewerScurry_Manager.ins.mapManager.Rows[row].Blocks[i];
                        BlocksDanger.Add(blockTarget);
                    }
                }
                break;
        }
        //sau day la da co targetBlock : block ma trap nay cham toi
        //neu targetBlock = null => dang bi chan dau ngay tu block dau tien
        //---------------------------------------------

        if (isMoveX)
        {
            OriginalPosXZ = trapObjectParent.transform.position.x;
        }
        else
        {
            OriginalPosXZ = trapObjectParent.transform.position.z;
        }
    }
    public void SignClearBlockForAI()
    {
        foreach (SkewerScurry_Block b in BlocksDanger)
        {
            b.SignBlocksDanger();
        }
    }
    public void ActiveDangerBlock()
    {
        foreach (SkewerScurry_Block b in BlocksDanger)
        {
            b.isDanger = true;
        }
    }
    public void ActiveWarning()
    {
        //warning
        foreach (SkewerScurry_Block b in BlocksDanger)
        {
            b.Warning();
        }
    }
    public void Ready()
    {
        isReady = false;
        //if (blockTarget == null)
        //{
        //    isReady = true;
        //    return;
        //}
        //xoay nhẹ nhàng khoảng 45 độ cùng lúc đi ra ngoài 1 chút
        //thụt vào sâu hơn lúc đầu , rồi xoay nhanh dần
        //xoay nhẹ nhàng khoảng 45 độ
        if (isMoveX)
        {
            ActiveDangerBlock();
            trapObject.transform.DORotate(new Vector3(90,trapObject.transform.eulerAngles.y, trapObject.transform.eulerAngles.z),0.5f).SetEase(Ease.Linear);
            trapObjectParent.transform.DOMoveX(OriginalPosXZ + directionMove * distanceMoveReady, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                Timer.Schedule(this,TIME_DELAY,()=> {
                    trapObjectParent.transform.DOMoveX(OriginalPosXZ - directionMove * distanceBackReady, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                        if(Form_Gameplay.ins.timeCur > 0) ActiveWarning();
                        isReady = true;
                    });
                });
            });
        }
        else
        {
            ActiveDangerBlock();
            trapObject.transform.DORotate(new Vector3(90, trapObject.transform.eulerAngles.y, trapObject.transform.eulerAngles.z), 0.5f).SetEase(Ease.Linear);
            trapObjectParent.transform.DOMoveZ(OriginalPosXZ + directionMove * distanceMoveReady, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                Timer.Schedule(this, TIME_DELAY, () => {
                    trapObjectParent.transform.DOMoveZ(OriginalPosXZ - directionMove * distanceBackReady, 0.5f).SetEase(Ease.Linear).OnComplete(() => {
                        if (Form_Gameplay.ins.timeCur > 0) ActiveWarning();
                        isReady = true;
                    });
                });
            });
        }
    }
    public void Active()
    {
        
        isDoneActive = false; // khi xong thi bien nay` se chuyen thanh` true
        Vector3 targetRot = new Vector3(360, trapObject.transform.eulerAngles.y, trapObject.transform.eulerAngles.z);

        if (blockTarget == null)
        {
            //isDoneActive = true;
            //isDoneDeactive = true;

            Tween Rotate1 = trapObject.transform.DORotate(targetRot, 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            Timer.Schedule(this, TIME_DELAY_BEFORE_ACTIVE, () => {
                colKill.enabled = true;
                if (isMoveX)
                {

                    trapObjectParent.transform.DOMoveX(obtacleBlock.transform.position.x +(SkewerScurry_Manager.ins.mapManager.widthBlock * -directionMove), Mathf.Abs((obtacleBlock.transform.position.x - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap)).SetEase(Ease.Linear).OnComplete(() => {
                        isDoneActive = true;
                        Rotate1.Kill();
                        if (!SkewerScurry_Manager.ins.mapManager.isShake)
                        {
                            CameraShake.ins.Shake();
                            SkewerScurry_Manager.ins.mapManager.isShake = true;
                        }
                    });
                }
                else
                {

                    trapObjectParent.transform.DOMoveZ(obtacleBlock.transform.position.z+(SkewerScurry_Manager.ins.mapManager.heightBlock * -directionMove), Mathf.Abs((obtacleBlock.transform.position.z - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap)).SetEase(Ease.Linear).OnComplete(() => {
                        isDoneActive = true;
                        Rotate1.Kill();
                        if (!SkewerScurry_Manager.ins.mapManager.isShake)
                        {
                            CameraShake.ins.Shake();
                            SkewerScurry_Manager.ins.mapManager.isShake = true;
                        }
                    });
                }
            });
            return;
        }
        
        
        Tween Rotate = trapObject.transform.DORotate(targetRot, 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        Timer.Schedule(this,TIME_DELAY_BEFORE_ACTIVE,()=> {
            colKill.enabled = true;
            if (isMoveX)
            {

                trapObjectParent.transform.DOMoveX(blockTarget.transform.position.x, Mathf.Abs((blockTarget.transform.position.x - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap)).SetEase(Ease.Linear).OnComplete(() => {
                    isDoneActive = true;
                    Rotate.Kill();
                    if (!SkewerScurry_Manager.ins.mapManager.isShake)
                    {
                        CameraShake.ins.Shake();
                        SkewerScurry_Manager.ins.mapManager.isShake = true;
                    }
                });
            }
            else
            {

                trapObjectParent.transform.DOMoveZ(blockTarget.transform.position.z, Mathf.Abs((blockTarget.transform.position.z - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap)).SetEase(Ease.Linear).OnComplete(() => {
                    isDoneActive = true;
                    Rotate.Kill();
                    if (!SkewerScurry_Manager.ins.mapManager.isShake)
                    {
                        CameraShake.ins.Shake();
                        SkewerScurry_Manager.ins.mapManager.isShake = true;
                    }
                });
            }
        });
        
    }
    public void Deactive()
    {
        
        if (blockTarget == null) 
        {//Obstacles
            isDoneDeactive = false;
            Vector3 targetRot = new Vector3(360, trapObject.transform.eulerAngles.y, trapObject.transform.eulerAngles.z);
            if (isMoveX)
            {
                Rotate(trapObject.transform, SPD_ROTATE_BACK, -1, Mathf.Abs((obtacleBlock.transform.position.x - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * DIVINE_SPD_BACK));
                //trapObject.transform.DORotate(targetRot, Mathf.Abs((obtacleBlock.transform.position.x - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * 3),RotateMode.FastBeyond360).SetEase(Ease.Linear);
                trapObjectParent.transform.DOMoveX(OriginalPosXZ, Mathf.Abs((obtacleBlock.transform.position.x - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * 3)).SetEase(Ease.Linear).OnComplete(() => {
                    isDoneDeactive = true;
                    colKill.enabled = false;
                });
            }
            else
            {
                Rotate(trapObject.transform, SPD_ROTATE_BACK, -1, Mathf.Abs((obtacleBlock.transform.position.z - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * DIVINE_SPD_BACK));
                //trapObject.transform.DORotate(targetRot, Mathf.Abs((obtacleBlock.transform.position.z - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * 3), RotateMode.FastBeyond360).SetEase(Ease.Linear);
                trapObjectParent.transform.DOMoveZ(OriginalPosXZ, Mathf.Abs((obtacleBlock.transform.position.z - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * DIVINE_SPD_BACK)).SetEase(Ease.Linear).OnComplete(() => {
                    isDoneDeactive = true;
                    colKill.enabled = false;
                });
            }
            return; 
        }
        

        

        if (isMoveX)
        {
            //Tween t = trapObject.transform.DORotate(targetRot1, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            Rotate(trapObject.transform, SPD_ROTATE_BACK, -3, Mathf.Abs((blockTarget.transform.position.x - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * DIVINE_SPD_BACK));
            trapObjectParent.transform.DOMoveX(OriginalPosXZ, Mathf.Abs((blockTarget.transform.position.x - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * DIVINE_SPD_BACK)).SetEase(Ease.Linear).OnComplete(()=> {
                isDoneDeactive = true;
                colKill.enabled = false;
                //t.Kill();
            });
        }
        else
        {
            Rotate(trapObject.transform, SPD_ROTATE_BACK, -3, Mathf.Abs((blockTarget.transform.position.z - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * DIVINE_SPD_BACK));
            //Tween t = trapObject.transform.DORotate(targetRot1, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1); 
            trapObjectParent.transform.DOMoveZ(OriginalPosXZ, Mathf.Abs((blockTarget.transform.position.z - OriginalPosXZ) / SkewerScurry_Manager.ins.mapManager.spdMove_Trap * DIVINE_SPD_BACK)).SetEase(Ease.Linear).OnComplete(() => {
                isDoneDeactive = true;
                colKill.enabled = false;
                //t.Kill();
            });
        }

        //Block clear warning
        foreach (SkewerScurry_Block b in BlocksDanger)
        {
            b.Clear();
        }

        isDoneDeactive = false;
    }
    #region rotate mechanic

    private bool isRotate = false;
    private Transform T_objectRotate;
    private int direction = 1; //-1 or 1
    private float spdRotate;
    private void FixedUpdate()
    {
        //rotate mechanic x axis
        if (isRotate)
        {
            T_objectRotate.Rotate(Vector3.right * spdRotate);
        }
    }
    private void Rotate(Transform t , float spdRot, int dir, float timeDo)
    {
        isRotate = true;
        T_objectRotate = t;
        direction = dir;
        spdRotate = spdRot;
        StartCoroutine(I_Rotate(timeDo));
    }
    IEnumerator I_Rotate(float time)
    {
        yield return new WaitForSeconds(time);
        isRotate = false;
    }
    #endregion
}
