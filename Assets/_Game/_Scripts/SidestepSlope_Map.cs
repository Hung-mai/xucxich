using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidestepSlope_Map : MonoBehaviour
{
    public float speedXThrow = 0;
    public int minPositionZ_Shrink = 95;
    public int minPositionZ_Disable = 75;
    public int velocityZMin = -80;
    public float periodThrow = 0.5f;
    public Vector2 speedX = new Vector2(-25, 50);
    public Vector2 speedY = new Vector2(-50, 50);
    public Vector2 speedZ = new Vector2(50, 100);
    private int idPrefabRocks = 0;
    public List<SidestepSlope_Rock> listPrefabRocks;
    public List<SidestepSlope_Rock> listRocks;
    public void Start() {
         StartCoroutine(ThrowRock());
    }

    public IEnumerator ThrowRock() {
        //Ném từng Rock một và từ trái qua phải
        speedXThrow += 50;
        if (speedXThrow > speedX.y) speedXThrow = speedX.x;
        //Nếu có Rock nào trong list chưa sửa dụng thì dùng. Còn ko thì tạo mới luôn
        SidestepSlope_Rock rockThrow = GetRockNotUsed();
        rockThrow.transform.position = new Vector3(0, SidestepSlope_Manager.ins.listTargetMove[SidestepSlope_Manager.ins.listTargetMove.Count - 2].transform.position.y +50, SidestepSlope_Manager.ins.listTargetMove[SidestepSlope_Manager.ins.listTargetMove.Count - 2].transform.position.z);
        rockThrow.transform.localScale = Vector3.one;
        rockThrow.gameObject.SetActive(true);
        //rock.rigid.AddForce(new Vector3(Random.Range(speedX.x, speedX.y), Random.Range(speedY.x, speedY.y), Random.Range(speedZ.x, speedZ.y)), ForceMode.VelocityChange);
        rockThrow.rigid.AddForceAtPosition(new Vector3(speedXThrow, Random.Range(speedY.x, speedY.y), Random.Range(speedZ.x, speedZ.y)), new Vector3(Random.Range(-40, 40), Random.Range(10, -10), Random.Range(-20, 20)), ForceMode.VelocityChange);
        yield return new WaitForSeconds(periodThrow);
        if(!SidestepSlope_Manager.ins.isGameplay_End)  StartCoroutine(ThrowRock());
    }

    private void Update() {
        for (int i = 0; i < listRocks.Count; i++) {
            if (listRocks[i].gameObject.activeSelf) {
                if (listRocks[i].transform.position.z < minPositionZ_Shrink && listRocks [i].transform.localScale.x == 1) {
                    listRocks [i].transform.DOScale(0.01f,0.15f);
                }
                if (listRocks [i].transform.position.z < minPositionZ_Disable && listRocks [i].gameObject.activeSelf) {
                    listRocks [i].gameObject.SetActive(false);
                }
                listRocks [i].rigid.velocity = new Vector3(listRocks[i].rigid.velocity.x, listRocks[i].rigid.velocity.y, velocityZMin); 
            }
        }
    }

    public SidestepSlope_Rock GetRockNotUsed() {
        for (int i = 0; i < listRocks.Count; i++) {
            if (!listRocks[i].gameObject.activeSelf) return listRocks[i];
        }
        SidestepSlope_Rock rockNotUsed = Instantiate(listPrefabRocks[idPrefabRocks], transform);
        idPrefabRocks++;
        if (idPrefabRocks >= listPrefabRocks.Count) idPrefabRocks = 0;
        listRocks.Add(rockNotUsed);
        return rockNotUsed;
    }
}
