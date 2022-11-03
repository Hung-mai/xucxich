using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelChar : MonoBehaviour {
    [Header("--------- Data Skin----------")]
    public Data_Skin dataSkin;
    public int idSkin_Full = -1;
    public int idSkin_Hair = -1;
    public int idSkin_Hand = -1;
    public int idSkin_Body = 0;
    public int idColor;

    [Header("--------- Ref ----------")]
    //public GameObject objCrown;
    public SpriteRenderer sprite_ArrowForward;
    public Collider_AttackChar collider_AttackChar;
    public GameObject [] obj_shoes;
    public ParticleSystem effect_TrailBooster;
    public Renderer [] meshBody;
    public Renderer [] meshColor;
    public int idBrown = 0;
    public int idLid = -1;
    public int idEye = 0;
    public int idMounth = 7;
    public GameObject [] list_Brown;
    public GameObject [] list_Lid;
    public GameObject [] list_Eye;
    public GameObject [] list_Mounth;

    public GameObject obj_Animation;
    public GameObject obj_Death;
    public ModelDeath [] listModelDeath;

    public Character character;
    public Animator animator;

   
    public SkinModel [] listSkinModel;

    private void Awake() {
        ReloadFace();
    }

    public virtual void AniEvent_ApplyDamage() {
        character.AniEvent_ApplyDamage();
    }

    public virtual void AniEvent_EndAttack() {
        character.AniEvent_EndAttack();
    }

    public void WearSkinByGameSave(bool isRemoveOld = true) {
        idSkin_Full = DataManager.ins.gameSave.idSkin_Full;
        idSkin_Hair = DataManager.ins.gameSave.idSkin_Hair;
        idSkin_Hand = DataManager.ins.gameSave.idSkin_Hand;
        idSkin_Body = DataManager.ins.gameSave.idSkin_Body;
        WearSkinByIDCur(isRemoveOld);
    }

    public void WearSkinByIDCur(bool isRemoveOld = true) {
        if (idSkin_Full >= 0) {
            idSkin_Hair = -1;
            idSkin_Hand = -1;
            idSkin_Body = -1;
        } else {
            if (idSkin_Body < 0) idSkin_Body = 0;
        }
        if (isRemoveOld) { //Xóa mọi Obj Skin cũ
            for (int i = 0; i < listSkinModel.Length; i++) {
                if (listSkinModel [i].obj_3D != null) Destroy(listSkinModel [i].obj_3D.gameObject);
            }
        }
        //Nếu Skin có Texture
        ReloadMaterial_Body();
        if (idSkin_Full >= 0 && idSkin_Full < GameManager.ins.arrayDataSkin_Full.Length) {
            for (int i = 0; i < GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent.Length; i++) {
                if (GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent [i].obj_3D != null) {
                    GameObject newModel = Instantiate(GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent [i].obj_3D);
                    newModel.transform.parent = listSkinModel [(int)GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent [i].skeleton].tran_Container;
                    newModel.transform.localPosition = GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent [i].pos_Skin;
                    newModel.transform.localEulerAngles = GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent [i].rot_Skin;
                    newModel.transform.localScale =  GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent [i].scale_Skin;
                    listSkinModel [(int)GameManager.ins.arrayDataSkin_Full [idSkin_Full].listComponent [i].skeleton].obj_3D = newModel;
                }
            }
        } else {
            if (idSkin_Hair >= 0 && idSkin_Hair < GameManager.ins.arrayDataSkin_Hair.Length) {
                if (GameManager.ins.arrayDataSkin_Hair [idSkin_Hair].listComponent [0].obj_3D != null) {
                    GameObject newModel = Instantiate(GameManager.ins.arrayDataSkin_Hair [idSkin_Hair].listComponent [0].obj_3D);
                    newModel.transform.parent = listSkinModel [(int)GameManager.ins.arrayDataSkin_Hair [idSkin_Hair].listComponent [0].skeleton].tran_Container;
                    newModel.transform.localPosition = GameManager.ins.arrayDataSkin_Hair [idSkin_Hair].listComponent [0].pos_Skin;
                    newModel.transform.localEulerAngles = GameManager.ins.arrayDataSkin_Hair [idSkin_Hair].listComponent [0].rot_Skin;
                    newModel.transform.localScale =GameManager.ins.arrayDataSkin_Hair [idSkin_Hair].listComponent [0].scale_Skin;
                    listSkinModel [(int)GameManager.ins.arrayDataSkin_Hair [idSkin_Hair].listComponent [0].skeleton].obj_3D = newModel;
                }
            }  
            if (idSkin_Hand >= 0 && idSkin_Hand < GameManager.ins.arrayDataSkin_Hand.Length) {
                for (int i = 0; i < GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent.Length; i++) {
                    if (GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent [i].obj_3D != null) {
                        GameObject newModel = Instantiate(GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent [i].obj_3D);
                        newModel.transform.parent = listSkinModel [(int)GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent [i].skeleton].tran_Container;
                        newModel.transform.localPosition = GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent [i].pos_Skin;
                        newModel.transform.localEulerAngles = GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent [i].rot_Skin;
                        newModel.transform.localScale = GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent [i].scale_Skin;
                        listSkinModel [(int)GameManager.ins.arrayDataSkin_Hand [idSkin_Hand].listComponent [i].skeleton].obj_3D = newModel;
                    }
                }
            }
        }
    }

    private void ReloadMaterial_Body() {
        if (idSkin_Full >= 0 && idSkin_Full < GameManager.ins.arrayDataSkin_Full.Length) {
            if(GameManager.ins.arrayDataSkin_Full [idSkin_Full].texure_Skin != null) {
                for (int i = 0; i < meshBody.Length; i++) {
                    meshBody [i].material.mainTexture = GameManager.ins.arrayDataSkin_Full [idSkin_Full].texure_Skin;
                    meshBody [i].material.color =  Color.white;
                }
                for (int i = 0; i < meshColor.Length; i++) {
                    meshColor [i].material.color =   GameManager.ins.arrayDataSkin_Full [idSkin_Full].colorEyelid;
                }
            } else {
                for (int i = 0; i < meshBody.Length; i++) {
                    meshBody [i].material.mainTexture = null;
                    meshBody [i].material.color =  GameManager.ins.listColorsChar [GameManager.ins.arrayDataSkin_Full [idSkin_Full].idColor];
                }
                for (int i = 0; i < meshColor.Length; i++) {
                    meshColor [i].material.color =  GameManager.ins.listColorsChar [GameManager.ins.arrayDataSkin_Full [idSkin_Full].idColor];
                }
            }
            idColor = GameManager.ins.arrayDataSkin_Full [idSkin_Full].idColor;
        }else if (idSkin_Body >= 0 && idSkin_Full < GameManager.ins.arrayDataSkin_Body.Length) {
            if (GameManager.ins.arrayDataSkin_Body [idSkin_Body].texure_Skin != null) {
                for (int i = 0; i < meshBody.Length; i++) {
                    meshBody [i].material.mainTexture = GameManager.ins.arrayDataSkin_Body [idSkin_Body].texure_Skin;
                    meshBody [i].material.color =  Color.white;
                }
                for (int i = 0; i < meshColor.Length; i++) {
                    meshColor [i].material.color =   GameManager.ins.arrayDataSkin_Body [idSkin_Body].colorEyelid;
                }
            } else {
                for (int i = 0; i < meshBody.Length; i++) {
                    meshBody [i].material.mainTexture = null;
                    meshBody [i].material.color =  GameManager.ins.listColorsChar [GameManager.ins.arrayDataSkin_Body [idSkin_Body].idColor];
                }
                for (int i = 0; i < meshColor.Length; i++) {
                    meshColor [i].material.color =  GameManager.ins.listColorsChar [GameManager.ins.arrayDataSkin_Body [idSkin_Body].idColor];
                }
            }
            idColor = GameManager.ins.arrayDataSkin_Body [idSkin_Body].idColor;
        }
        sprite_ArrowForward.color = GameManager.ins.listColorsChar [idColor];
    }

    public void ReloadFace() {
        for (int i = 0; i < list_Brown.Length; i++) {
            list_Brown [i].SetActive(i == idBrown);
        }
        for (int i = 0; i < list_Lid.Length; i++) {
            list_Lid [i].SetActive(i == idLid);
        }
        for (int i = 0; i < list_Eye.Length; i++) {
            list_Eye [i].SetActive(i == idEye);
        }
        for (int i = 0; i < list_Mounth.Length; i++) {
            list_Mounth [i].SetActive(i == idMounth);
        }
    }

    public void ReloadModelDeath(ColliderKillChar colliderKillChar = ColliderKillChar.None) {
        for (int i = 0; i < listModelDeath.Length; i++) {
            if (listModelDeath [i].killChar == colliderKillChar) {
                listModelDeath [i].obj.SetActive(true);
                for (int j = 0; j < listModelDeath [i].tran_part.Length; j++) {
                    listModelDeath [i].tran_part [j].localPosition = Vector3.zero;
                    listModelDeath [i].tran_part [j].localEulerAngles = Vector3.zero;
                    listModelDeath [i].collider [j].enabled = true;
                    int forceTop_X = Random.Range(0, 100) >= 50 ? -500*Random.Range(3, 8) : 500*Random.Range(3, 8);
                    int forceBot_X = Random.Range(0, 100) >= 50 ? -500*Random.Range(3, 6) : 500*Random.Range(3, 6);
                    if (Form_Gameplay.ins.minigameManager.minigame == Minigame.WackyRun) forceTop_X = forceBot_X = 0;
                    int forceTop_Z = Random.Range(0, 100) >= 50 ? -500*Random.Range(3, 8) : 500*Random.Range(3, 8);
                    int forceBot_Z = Random.Range(0, 100) >= 50 ? -500*Random.Range(3, 6) : 500*Random.Range(3, 6);
                    int forceBot_Y = 500*Random.Range(4, 9);
                    listModelDeath [i].rigid [j].isKinematic = false;
                    Vector3 force = new Vector3(forceTop_X, forceBot_Y, forceTop_Z);
                    listModelDeath [i].rigid [j].AddForce(force, ForceMode.Force);
                }
            } else {
                listModelDeath [i].obj.SetActive(false);
            }
        }
    }

    public void LockModelDeath(ColliderKillChar colliderKillChar = ColliderKillChar.None) {
        for (int i = 0; i < listModelDeath.Length; i++) {
            if (listModelDeath [i].killChar == colliderKillChar) {
                for (int j = 0; j < listModelDeath [i].tran_part.Length; j++) {
                    listModelDeath [i].rigid [j].isKinematic = true;
                    listModelDeath [i].collider [j].enabled = false;                
                }
            }
        }
    }
}

[System.Serializable]
public class SkinModel {
    public CharSkin_Skeleton skeleton;
    public bool isSkinMesh;
    public Transform tran_Container;
    public GameObject obj_3D;//Obj 3D chỉ gắn vào 1 xương
}

[System.Serializable]
public class ModelDeath {
    public ColliderKillChar killChar;
    public GameObject obj;
    public Transform [] tran_part;
    public Rigidbody[] rigid;
    public Collider[] collider;
}
