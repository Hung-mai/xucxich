using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformPush_Map : MonoBehaviour
{
    public float durationFlickerBlock = 2.5f;
    public float durationFallBlock = 5f;
    public int widthBlock = 75;
    public int idFallingBlock = -1;
    public Color colorFallBlock;
    public bool isValidateCreateListPosBlock = false;
    public List<Vector2> list_PosBlocksAll;
    public List<PlatformPush_Block> list_BlocksAll;
    public Coroutine coroutine_FallBlock;

    private void OnValidate() {
        if (isValidateCreateListPosBlock && Application.isEditor) {
            list_PosBlocksAll.Clear();
            for (int i = 0; i < list_BlocksAll.Count; i++) {
                Vector2 vector2 = new Vector2(Mathf.RoundToInt(list_BlocksAll[i].transform.localPosition.x/ widthBlock), Mathf.RoundToInt(list_BlocksAll[i].transform.localPosition.z / widthBlock));
                list_PosBlocksAll.Add(vector2);
                list_BlocksAll[i].id = i;
            }
        }
    }

    public IEnumerator FallBlock() {
        FindIDFallBlockNext(new List<int>());
        if (idFallingBlock >= 0 && Form_Gameplay.ins.timeCur > 0) {
            list_BlocksAll [idFallingBlock].isFlicker = true;
            list_BlocksAll [idFallingBlock].StartEffectFlicker(colorFallBlock, durationFlickerBlock);
        }
        yield return new WaitForSeconds(durationFlickerBlock);
        if (idFallingBlock >= 0 && Form_Gameplay.ins.timeCur > 0) {
                list_BlocksAll [idFallingBlock].isFell = true;
            list_BlocksAll[idFallingBlock].tran_MeshBlock.DOMove(list_BlocksAll[idFallingBlock].tran_MeshBlock.position + Vector3.down* widthBlock, durationFallBlock);
            coroutine_FallBlock = StartCoroutine(FallBlock());
        }
    }

    private void FindIDFallBlockNext(List<int> listIDIgnore) {
        List<int> listIDRemoveIgnore = Enumerable.Range(0, list_BlocksAll.Count).ToList();
        for (int i = 0; i < list_BlocksAll.Count; i++) {
            if(list_BlocksAll[i].isFell || list_BlocksAll[i].isFlicker) listIDRemoveIgnore.Remove(i);
        }
        int amoutBlockNoFall = listIDRemoveIgnore.Count;
        if (listIDRemoveIgnore.Count == 1 || listIDRemoveIgnore.Count == listIDIgnore.Count) {
            if(listIDRemoveIgnore.Count > 1) Debug.LogError("Ko tìm đc block có thể sập");
            idFallingBlock = -1;
            return;
        }
        for (int i = 0; i < listIDIgnore.Count; i++) {
            listIDRemoveIgnore.Remove(listIDIgnore[i]);
        }
        idFallingBlock = listIDRemoveIgnore[Random.Range(0, listIDRemoveIgnore.Count)];
        int idBlockGrowCenter = -1;
        for (int i = 0; i < list_BlocksAll.Count; i++) {
            if (!list_BlocksAll[i].isFell && !list_BlocksAll[i].isFlicker && i != idFallingBlock) { 
                idBlockGrowCenter = i;
                break;
            }
        }
        if(idBlockGrowCenter < 0) {
            Debug.LogError("Lỗi idBlockGrowCenter");
            idFallingBlock = -1;
            return;
        }
        List<int> listIDGrow = new List<int>();
        listIDGrow.Add(idBlockGrowCenter);
        //Nếu block đó sập mà khiến lục địa ko liền mạch -> Thì random lại id
        for (int i = 0; i < listIDGrow.Count; i++) {
            for (int j = 0; j < list_PosBlocksAll.Count; j++) {
                if (j == idFallingBlock || listIDGrow.Contains(j) || list_BlocksAll[j].isFell || list_BlocksAll[j].isFlicker) continue;
                if (list_PosBlocksAll[listIDGrow[i]].x == list_PosBlocksAll[j].x && list_PosBlocksAll[listIDGrow[i]].y == list_PosBlocksAll[j].y + 1) {
                    listIDGrow.Add(j);
                } else if (list_PosBlocksAll[listIDGrow[i]].x == list_PosBlocksAll[j].x && list_PosBlocksAll[listIDGrow[i]].y == list_PosBlocksAll[j].y - 1) {
                    listIDGrow.Add(j);
                } else if (list_PosBlocksAll[listIDGrow[i]].x == list_PosBlocksAll[j].x - 1 && list_PosBlocksAll[listIDGrow[i]].y == list_PosBlocksAll[j].y) {
                    listIDGrow.Add(j);
                } else if (list_PosBlocksAll[listIDGrow[i]].x == list_PosBlocksAll[j].x + 1 && list_PosBlocksAll[listIDGrow[i]].y == list_PosBlocksAll[j].y) {
                    listIDGrow.Add(j);
                }
            }
        }
        if (listIDGrow.Count < amoutBlockNoFall - 1) {
            listIDIgnore.Add(idFallingBlock);
            FindIDFallBlockNext(listIDIgnore);
        }
    }

    public int FindIDBlockByPosition(Vector3 position) {
        int idBlockNearest = 0;
        float distanceMin = 0;
        for (int i = 0; i < list_BlocksAll.Count; i++) {
            if (i == 0) {
                idBlockNearest = i;
                distanceMin = (position - list_BlocksAll[i].transform.position).magnitude;
            } else if((position - list_BlocksAll[i].transform.position).magnitude < distanceMin) {
                idBlockNearest = i;
                distanceMin = (position - list_BlocksAll[i].transform.position).magnitude;
            }
        }
        return idBlockNearest;
    }

    public List<int> FindIDBlocksNeighborNoFall(int idBlockStand) {
        List<int> listIDNeighbor = new List<int>();
        for (int i = 0; i < list_PosBlocksAll.Count; i++) {
            if (list_BlocksAll[i].isFell || list_BlocksAll[i].isFlicker) continue;
            if (list_PosBlocksAll[i].x == list_PosBlocksAll[idBlockStand].x && list_PosBlocksAll[i].y == list_PosBlocksAll[idBlockStand].y + 1) {
                listIDNeighbor.Add(i);
            } else if (list_PosBlocksAll[i].x == list_PosBlocksAll[idBlockStand].x && list_PosBlocksAll[i].y == list_PosBlocksAll[idBlockStand].y - 1) {
                listIDNeighbor.Add(i);
            } else if (list_PosBlocksAll[i].x == list_PosBlocksAll[idBlockStand].x - 1 && list_PosBlocksAll[i].y == list_PosBlocksAll[idBlockStand].y) {
                listIDNeighbor.Add(i);
            } else if (list_PosBlocksAll[i].x == list_PosBlocksAll[idBlockStand].x + 1 && list_PosBlocksAll[i].y == list_PosBlocksAll[idBlockStand].y) {
                listIDNeighbor.Add(i);
            }
        }
        return listIDNeighbor;
    }
}
