using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Skin", menuName = "Assets/Data_Skin", order = 4)]
public class Data_Skin : ScriptableObject {
    public string keyID;
    public CharSkin_Type type;//0:Hat 1:Pants

    public string nameCharSkin;
    public int idColor = -1;//Sẽ fix cứng màu của UI theo Texture Skin (Nếu có)
    public Color colorEyelid;
    public Texture texure_Skin;//Texture Skin
    public Sprite icon;
    public int cost;
    public int amountVideoAds;
    public bool isVip = false;//Những skin phải xem VideoAds khi Offer
    public bool HideOnIOS = false;//Ẩn những skin này ở IOS

    public CharSkin_Component [] listComponent;
}

[System.Serializable]
public class CharSkin_Component {
    public CharSkin_Skeleton skeleton;
    public Vector3 pos_Skin;
    public Vector3 rot_Skin;
    public Vector3 scale_Skin;

    public GameObject obj_3D;//Obj 3D chỉ gắn vào 1 xương
}