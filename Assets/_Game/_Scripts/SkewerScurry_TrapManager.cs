using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkewerScurry_TrapManager : MonoBehaviour
{
    public SkewerScurry_Trap[] traps;
    public bool isDone = false;

    private enum TrapType
    {
        All,
        Random,
    }

    [SerializeField] private TrapType type = TrapType.All;
    [Range(1,4)][SerializeField]private int amountRandom = 4;

    private SkewerScurry_Trap[] trapsRandom;
   
    public void ReadyTrap(Action CallBack)
    {
        if (type == TrapType.Random)
        {
            System.Random random = new System.Random();
            traps = traps.OrderBy(x => random.Next()).ToArray();
            if (amountRandom > 4)
            {
                amountRandom = 4;
            }
        }
        else if (type == TrapType.All)
        {
            amountRandom = 4;
        }
        StartCoroutine(I_Ready(CallBack));
    }
    IEnumerator I_Ready(Action CallBack)
    {
        for (int i = 0; i < amountRandom; i++)
        {
            traps[i].Ready();
        }

        for (int i = 0; i < amountRandom; i++)
        {
            yield return new WaitUntil(() => traps[i].isReady);
        }
        CallBack();
    }
    public void ActiveTraps()
    {
        isDone = false;

        

        for (int i = 0; i < amountRandom; i++)
        {
            traps[i].Active();
        }
        StartCoroutine(I_CheckAllTrapsDone());
    }


    IEnumerator I_CheckAllTrapsDone()
    {
        for (int i = 0; i < amountRandom; i++)
        {
            yield return new WaitUntil((() => traps[i].isDoneActive == true));
        }
        for (int i = 0; i < amountRandom; i++)
        {
            traps[i].Deactive();
        }
        for (int i = 0; i < amountRandom; i++)
        {
            yield return new WaitUntil(()=> traps[i].isDoneDeactive == true);
        }
        isDone = true;

        SkewerScurry_Manager.ins.mapManager.isActive = false;
    }

}
