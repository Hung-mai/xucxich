using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thinh_test_Script : MonoBehaviour
{
    public Minigame levelCheat = Minigame.None;
    [ContextMenu("Print Test")]
    public void Print()
    {
        Debug.Log((int)levelCheat);
    }

    [Header("--------Tools--------")]
    public GameObject[] sortObjects;
    [SerializeField]private float offset;
    [ContextMenu("SORT")]
    public void SortObject()
    {
        for (int i = 0; i < sortObjects.Length; i++)
        {
            if (i == sortObjects.Length - 1) break;
            Debug.Log(i);
            sortObjects[i + 1].transform.position = new Vector3(sortObjects[i].transform.position.x + offset, sortObjects[0].transform.position.y, sortObjects[0].transform.position.z) ;
        }
    }
    public Vector2 mousePos;
    private void FixedUpdate()
    {
        mousePos = Input.mousePosition;
    }
    [Header("---------player---------")]
    public ModelChar md;
    [ContextMenu("Test")]
    public void abs()
    {
        md.WearSkinByIDCur(true);
    }
    public Animator anim;
    public string stringTrigger;
    [ContextMenu("testanim")]
    public void slenderControl()
    {
        anim.SetTrigger(stringTrigger);
    }
}
