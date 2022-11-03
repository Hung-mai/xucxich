using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBlock : MonoBehaviour
{
    public float speed ;
    private float distanceBlocks = 49;//90%*50
    private int idPointTarget = 0;
    public List<Transform> listBlocks;
    public Transform [] listPointsTarget;

    void FixedUpdate() {
        if (SceneManager.ins.async != null) return;
        if ((listBlocks[0].position - listPointsTarget[idPointTarget].position).magnitude <= speed * 0.55f) {//Chính xác là 50% bán kính nhưng để 60% cho chắc
            idPointTarget++;
            if (idPointTarget > listPointsTarget.Length - 1) idPointTarget = 0;
        }
        listBlocks [0].transform.position =  Vector3.MoveTowards(listBlocks [0].transform.position, listPointsTarget [idPointTarget].position, speed);
        if((listBlocks [0].position - listBlocks [listBlocks.Count-1].position).magnitude >= distanceBlocks) listBlocks [listBlocks.Count-1].transform.position =  Vector3.MoveTowards(listBlocks [listBlocks.Count-1].transform.position, listBlocks [listBlocks.Count-2].transform.position, speed);
        if ((listBlocks [0].position - listBlocks [1].position).magnitude >= distanceBlocks) {
            listBlocks [0].localPosition = new Vector3(Mathf.RoundToInt(listBlocks [0].localPosition.x) , Mathf.RoundToInt(listBlocks [0].localPosition.y), Mathf.RoundToInt(listBlocks [0].localPosition.z));
            Transform tranCache = listBlocks [listBlocks.Count-1];
            tranCache.position = listBlocks [0].position;
            listBlocks.Remove(listBlocks [listBlocks.Count-1]);
            listBlocks.Insert(0,tranCache);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (int i = 0; i < listPointsTarget.Length; i++) {
            Gizmos.DrawWireSphere(listPointsTarget [i].position, 3f);
            if (i == 0) { 
                Gizmos.DrawLine(listPointsTarget [0].position, listPointsTarget [listPointsTarget.Length-1].position);
            } else {
                Gizmos.DrawLine(listPointsTarget [i].position, listPointsTarget [i-1].position);
            }
        }
    }
}
