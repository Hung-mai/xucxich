using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slither_Map : MonoBehaviour
{
    public float MaxX_SpawnFood = 230;
    public float MaxZ_SpawnFood = 230;
    public float periodSpawn = 2;
    public float periodFoodConnectToChar = 1;
    public float distanceSmoothMove = 10;
    public int distanceFoodsTail_ByAmountSmoothMove = 3;
    public int maxFoodsInMap = 50;
    public Color colorFoodBase;

    public Transform parentFoodsNotUsed;
    public List<Slither_Food> listFoodsAll;//List chứa tất các các Food
    public List<Slither_Food> listFood_NotUsed;//List chứa tất các các Food chưa đc ăn trên Map
    private int idPrefabFood = 0;
    public List<Slither_Food> listPrefabFoods;//List chứa tất cả các loại Food
    public void StartSpawn() {
        StartCoroutine(SpwanFood());
    }

    private IEnumerator SpwanFood() {
        if (listFood_NotUsed.Count <= maxFoodsInMap) {
            Slither_Food foodNew = GetFoodNotUsed();//Nếu có Food nào trong list chưa sửa dụng thì dùng. Còn ko thì tạo mới luôn
            foodNew.transform.position = new Vector3(Random.Range(-MaxX_SpawnFood, MaxX_SpawnFood), 0, Random.Range(-MaxZ_SpawnFood, MaxZ_SpawnFood));
            foodNew.transform.eulerAngles = Vector3.up * Random.Range(-180, 180);
            foodNew.transform.parent = parentFoodsNotUsed;
            foodNew.id = listFoodsAll.Count - 1;
            foodNew.name = "Food (" + listFoodsAll.Count + ")";
            foodNew.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(periodSpawn);
        if (!Slither_Manager.ins.isGameplay_End) StartCoroutine(SpwanFood());
    }

    private Slither_Food GetFoodNotUsed() {
        for (int i = 0; i < listFood_NotUsed.Count; i++) {
            if (!listFood_NotUsed[i].gameObject.activeSelf) return listFood_NotUsed[i];
        }
        Slither_Food foodNotUsed = Instantiate(listPrefabFoods[idPrefabFood]);
        foodNotUsed.mesh.material.color = colorFoodBase;
        idPrefabFood++;
        if (idPrefabFood >= listPrefabFoods.Count) idPrefabFood = 0;
        listFood_NotUsed.Add(foodNotUsed);
        listFoodsAll.Add(foodNotUsed);
        return foodNotUsed;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(2*MaxX_SpawnFood, 20, 2*MaxZ_SpawnFood));
    }
}
