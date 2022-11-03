using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class SkewerScurry_Block : MonoBehaviour
{
    public GameObject warning;
    public DOTweenAnimation warningAnim;
    public bool isDanger = false;
    public bool isObtacle = false;

    public Color colorGizmoDraw;
    public Color warnningColor;

    public MeshRenderer box_DamMau;
    public MeshRenderer box_NhatMau;

    private Sequence mySequence;
    private Color baseColor;
    private bool isTween = true;
    private void TweenYoYo()
    {
        if (!isTween) 
        {
            if (box_DamMau.gameObject.activeInHierarchy)
            {
                box_DamMau.material.DOColor(baseColor, 0.5f);
            }
            else if (box_NhatMau.gameObject.activeInHierarchy)
            {
                box_NhatMau.material.DOColor(baseColor, 0.5f);
            }
            return; 
        }
        if (box_DamMau.gameObject.activeInHierarchy)
        {
            mySequence.Append(box_DamMau.material.DOColor(warnningColor, 0.5f).OnComplete(() => {
                box_DamMau.material.DOColor(baseColor,0.5f).OnComplete(()=> {
                    TweenYoYo();
                });
                
            }));
        }
        else if (box_NhatMau.gameObject.activeInHierarchy)
        {
            mySequence.Append(box_NhatMau.material.DOColor(warnningColor, 0.5f).OnComplete(()=> {
                box_NhatMau.material.DOColor(baseColor,0.5f).OnComplete(()=> {
                    TweenYoYo();
                });
            }));
        }
    }
    public void SignBlocksDanger()
    {
        if (isObtacle) return;
        //remove bản khỏi list clear to
        try
        {
            SkewerScurry_Manager.ins.mapManager.blocksClear.Remove(this);
        }
        catch (Exception e)
        {
            Debug.LogError("(kiem soat)block khong co trong list clear");
        }
        //add bản thân vào list Danger to
        SkewerScurry_Manager.ins.mapManager.blocksDanger.Add(this);
    }
    public void SignBlockClear()
    {
        if (isObtacle) return;
        //remove bản thân khỏi list danger to 
        try
        {
            SkewerScurry_Manager.ins.mapManager.blocksDanger.Remove(this);
        }
        catch (Exception e)
        {
            Debug.LogError("(kiem soat)block khong co trong list Danger");
        }
        //add bản thân vào list clear
        SkewerScurry_Manager.ins.mapManager.blocksClear.Add(this);
    }
    public void Warning()
    {
        isDanger = true;
        //warning.SetActive(true);
        //warningAnim.DORestart();
        if (box_DamMau.gameObject.activeInHierarchy)
        {
            isTween = true;
            baseColor = box_DamMau.material.color;
            mySequence = DOTween.Sequence();
            TweenYoYo();
        }
        else if(box_NhatMau.gameObject.activeInHierarchy)
        {
            isTween = true;
            baseColor = box_NhatMau.material.color;
            mySequence = DOTween.Sequence();
            TweenYoYo();
        }

        SignBlocksDanger();
    }
    public void Clear()
    {
        isDanger = false;
        //warning.SetActive(false);
        isTween = false;

        SignBlockClear();
    }
    /*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = colorGizmoDraw;
        float radiusBlock = FindObjectOfType<SkewerScurry_map>().radiusBlock;
        SkewerScurry_map map = FindObjectOfType<SkewerScurry_map>();
        
        Vector3 sizeBlock = new Vector3(map.widthBlock, map.depthBlock,map.heightBlock);

        Gizmos.DrawWireCube(transform.position,sizeBlock);
        //Gizmos.DrawWireSphere(transform.position,radiusBlock);
    }
    */
}
