using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RowBlocks
{
    public List<SkewerScurry_Block> Blocks;
}
public class SkewerScurry_map : MonoBehaviour
{
    public SkewerScurry_TrapManager[] trapsManager;
    public float spdMove_Trap;
    public List<RowBlocks> Rows; //phải đặt đúng vị trí trên ma trận
    public float radiusBlock;
    public float widthBlock;//x
    public float heightBlock;//z
    public float depthBlock;//y

    public bool isReadyTrap = false;
    public bool isShake = false;
    public bool isActive = false;
    [Header("----------------manager block danger & block clear ---------------")]
    public List<SkewerScurry_Block> blocksDanger;
    public List<SkewerScurry_Block> blocksClear;
    
    private Coroutine C_StartActiveTraps;

    [SerializeField]private float TIME_HIDE = 1.5f;//thời gian chờ cho ng chơi trốn
    private float TIME_WAIT_ACTIVE = 1f; //thời gian chờ kích hoạt bẫy tiếp theo
    private int indexLastTrapsManagerActive = -1;
    public void StartActiveTraps()
    {
        FillListBlocksDangerAndClear();
        C_StartActiveTraps = StartCoroutine(I_StartActiveTraps());
    }

    private void FillListBlocksDangerAndClear()
    {
        foreach (RowBlocks r in Rows)
        {
            foreach (SkewerScurry_Block b in r.Blocks)
            {
                if (b.isDanger && !b.isObtacle)
                {
                    blocksDanger.Add(b);
                }
                else if(!b.isDanger && !b.isObtacle)
                {
                    blocksClear.Add(b);
                }
            }
        }
    }
    /*
* cach nhau khoang 1.5s den 3.5s thi trap active 1 lan
* tinh active tu khoang khi trap da thu ve thanh cong
*/
    IEnumerator I_StartActiveTraps()
    {
        while (true)
        {
            int i = randomIndexNoLoop(trapsManager.Length);
            isReadyTrap = false;
            isActive = true;
            trapsManager[i].ReadyTrap(()=> {
                isReadyTrap = true;
            });
            yield return new WaitUntil(()=>isReadyTrap == true);
            
            yield return new WaitForSeconds(TIME_HIDE);

            isShake = false;
            if (Form_Gameplay.ins.timeCur <= 0)
            {
                EndTraps();
                yield return null;
            }
            trapsManager[i].ActiveTraps();//active trap

            yield return new WaitUntil(() => trapsManager[i].isDone == true);
            if (Form_Gameplay.ins.timeCur <= 0)
            {
                EndTraps();
            }
        }
    }
    private int randomIndexNoLoop(int countIndex)
    {//tam thoi can review lai
        List<int> li = new List<int>();
        for (int i = 0; i < countIndex; i++)
        {
            li.Add(i);
        }
        //List<int> li = new List<int> { 0, 1, 2, 3 };//list index can random

        if (indexLastTrapsManagerActive == -1)
        {
            int x = Random.Range(0, li.Count);
            indexLastTrapsManagerActive = x;
            return x;
        }
        else
        {
            li.Remove(indexLastTrapsManagerActive);
            int x = Random.Range(0, li.Count);
            indexLastTrapsManagerActive = li[x];
            return li[x];
        }
    }
    public void EndTraps()
    {
        StopCoroutine(C_StartActiveTraps);
    }
    
}
