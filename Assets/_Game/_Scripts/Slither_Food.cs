using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Slither_Food : MonoBehaviour
{
    public int id;
    public MeshRenderer mesh;
    public Collider myCollider;
    public Character myChar;
    public bool isMoveFollowChar = false;

    private void OnTriggerEnter(Collider other) {
        Character character = other.GetComponent<Character>();
        if (character != null && character.isAlive) { 
            if ( myChar == null) {
                character.OnTriggerEnter_Food(this);
            } else {
                if (character.idChar != myChar.idChar && character.timeUndying < 0) {
                    character.OnTriggerEnter_Die(ColliderKillChar.Smash);
                }
            }
        }
    }

    public void ConnectToChar(Character character) {
        myChar = character;
        name = "Food (" + id + ")" + "_" + myChar.listFoods.Count;
        mesh.material.color = GameManager.ins.listColorsChar[ character.modelChar.idColor];
        myChar.listFoods.Add(this);
        Slither_Manager.ins.slither_Map.listFood_NotUsed.Remove(this);
        //Di chuyển đến vị trí cuối đuôi của Char
        Vector3 posTrail = myChar.listPosTail[Mathf.Clamp(myChar.listFoods.Count * Slither_Manager.ins.slither_Map.distanceFoodsTail_ByAmountSmoothMove, 0, myChar.listPosTail.Count -1)];
        //Timer.Schedule(this, Slither_Manager.ins.slither_Map.periodFoodConnectToChar, ()=>{
            //isMoveFollowChar = true;
        //});
        //transform.DOMove(posTrail, Slither_Manager.ins.slither_Map.periodFoodConnectToChar).onComplete = ()=> {
        //transform.position = posTrail;

        //};
        StartCoroutine(CountTime());
    }

    IEnumerator CountTime() {
            yield return new WaitForSecondsRealtime(1f);
            isMoveFollowChar = true;
    }

    public void DisconnectToChar() {
        isMoveFollowChar = false;
        myChar = null;
        mesh.material.color = Slither_Manager.ins.slither_Map.colorFoodBase;
        name = "Food (" + id + ")";
        Slither_Manager.ins.slither_Map.listFood_NotUsed.Add(this);
    }
}
