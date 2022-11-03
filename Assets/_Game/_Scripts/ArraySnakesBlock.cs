using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArraySnakesBlock : MonoBehaviour
{
    public bool isValidateCreatePath = false;
    public GameObject prefab_Block;
    public List<Vector2> list_PosNavObstacles;
    public List<GameObject> list_ObjNavObstacles;
    public List<Vector2> listPosBlocks;
    public Transform tranParentSnakesBlock;
    public SnakeBlock [] arraySnakesBlock;

    private void Awake() {
        for (int i = 0; i < list_ObjNavObstacles.Count; i++) {
            list_ObjNavObstacles [i].transform.localPosition = new Vector3(list_ObjNavObstacles [i].transform.localPosition.x,0.5f, list_ObjNavObstacles [i].transform.localPosition.z);
        }
    }

    private void FixedUpdate() {
        if (list_ObjNavObstacles == null || list_ObjNavObstacles.Count == 0) return;
        //Lưu lại vị trí tất cả các Block trong SnakeBlock vào 1 list<Vector2>
        listPosBlocks.Clear();
        for (int i = 0; i < arraySnakesBlock.Length; i++) {
            for (int j = 0; j < arraySnakesBlock [i].listBlocks.Count; j++) {
                Vector2 posBlock = new Vector2(arraySnakesBlock [i].listBlocks [j].localPosition.x, arraySnakesBlock [i].listBlocks [j].localPosition.z);
                if (!listPosBlocks.Contains(posBlock))   listPosBlocks.Add(posBlock);
            }
        }
        //Duyệt List để tắt những obj trùng vị trí list<Vector2> và những obj còn lại sẽ đc bật
        for (int i = 0; i < list_ObjNavObstacles.Count; i++) {
            list_ObjNavObstacles [i].SetActive(!listPosBlocks.Contains(list_PosNavObstacles[i]));
        }
    }

    private void OnValidate() {
        if (isValidateCreatePath && Application.isEditor) {
            //Reset lại List và xóa các obj cũ
            if (list_ObjNavObstacles != null) {
                for (int i = 0; i < list_ObjNavObstacles.Count; i++) {
                    DestroyImmediate(list_ObjNavObstacles[i]);
                }
                list_PosNavObstacles.Clear();
                list_ObjNavObstacles.Clear();
            } else {
                list_PosNavObstacles = new List<Vector2>();
                list_ObjNavObstacles = new List<GameObject>();
            }
            //Tạo ra 1 list các obj đường đi cho tất cả các SnakeBlock. Và gắn chúng vào List
            for (int i = 0; i < arraySnakesBlock.Length; i++) {
                for (int j = 0; j < arraySnakesBlock[i].listPointsTarget.Length; j++) {
                    if (j == 0) {
                        CreatePath_By2PointsTarget(arraySnakesBlock[i].listPointsTarget[0].localPosition, arraySnakesBlock[i].listPointsTarget[arraySnakesBlock[i].listPointsTarget.Length - 1].localPosition);
                    } else {
                        CreatePath_By2PointsTarget(arraySnakesBlock[i].listPointsTarget[j].localPosition, arraySnakesBlock[i].listPointsTarget[j - 1].localPosition);
                    }
                }
            }
        }
    }

    private void CreatePath_By2PointsTarget(Vector3 posStart, Vector3 posEnd) {
        int amountStep = (int)Mathf.Max(Mathf.Abs((posEnd - posStart).x), Mathf.Abs((posEnd - posStart).z));
        Vector3 step = (posEnd - posStart)/amountStep;
        for (int i = 0; i < amountStep; i++) {
            GameObject navObstacle = Instantiate(prefab_Block, tranParentSnakesBlock);
            navObstacle.transform.localPosition = posStart + step*(i+1);
            if (!list_PosNavObstacles.Contains(new Vector2(navObstacle.transform.localPosition.x, navObstacle.transform.localPosition.z))) {
                list_PosNavObstacles.Add(new Vector2(navObstacle.transform.localPosition.x, navObstacle.transform.localPosition.z));
                list_ObjNavObstacles.Add(navObstacle);
            }
        }
    }
}
