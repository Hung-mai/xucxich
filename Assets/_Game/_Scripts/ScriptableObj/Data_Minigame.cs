using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Minigame", menuName = "Assets/Data_Minigame", order = 3)]
public class Data_Minigame : ScriptableObject {
    public Minigame minigame;
    public string nameDisplay;

    public Sprite imgThumbnail;
    public string GameInfo;
    public string txt_Tutorial;

    public RankMinigame[] rankMinigames;
}

[Serializable]
public class RankMinigame {
    //public Minigame_Manager [] prefab_Maps_ABTest_Easy;
    public Minigame_Manager [] prefab_Maps;
    //public Minigame_Manager [] prefab_Maps_ABTest_Hard;
}
