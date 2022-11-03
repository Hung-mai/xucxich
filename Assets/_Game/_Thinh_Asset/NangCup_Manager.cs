using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NangCup_Manager : MonoBehaviour
{
    public GameObject FX_MoneyFlow;

    private float timeWaitToAction = 0f;

    private void Start()
    {
        StartCoroutine(I_ActionScene());
    }
    IEnumerator I_ActionScene()
    {
        yield return new WaitForSeconds(timeWaitToAction);
        FX_MoneyFlow.SetActive(true);
    }
}
