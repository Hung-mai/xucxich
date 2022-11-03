using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager> {
    public bool isLoaded = false;

    public GameSave gameSave;
    private GameSave gameSave_BackUp;//Luôn giống Data gốc, nhưng sẽ check để ko thể bị lỗi

    public List<int>[] list_IDFull_ByColor;//Danh sách các skin full theo màu char

    #region Unity
    private void OnApplicationPause(bool pause) { SaveGame(); }
    private void OnApplicationQuit() { SaveGame(); }
    #endregion

    public void LoadData() {
        try {
            if (isLoaded == false) {
                if (PlayerPrefs.HasKey("GameSave")) gameSave = JsonUtility.FromJson<GameSave>(PlayerPrefs.GetString("GameSave"));
                if (gameSave.isNew) {
                    gameSave = new GameSave();
                    gameSave.isNew = false;
                } else {
                    gameSave.totalSession++;
                    int timeNow = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
                    if (timeNow - gameSave.timeLastOpen > 0) {//Nếu sang ngày mới thì ??
                        gameSave.daysPlayed++;
                        gameSave.totalDays = timeNow - gameSave.timeInstall;
                        GameManager.ins.isShowOfflineEarning = true;
                    }
                    gameSave.timeLastOpen = timeNow;
                }
                //Cheat
                if (GameManager.ins.isRemoveAllAds) gameSave.isNoAds = true;

                //Load skin char
                LoadListCharSkin();
                //Load data mini game
                LoadListMinigame();

                isLoaded = true;
                SaveGame();
            }
        } catch (Exception ex) {
            Debug.LogError("Lỗi LoadData:" + ex);
        }
    }

    public void SaveGame() {
        try {
            if (!isLoaded) return;

            if (gameSave == null) {
                if (gameSave_BackUp != null) {
                    gameSave = gameSave_BackUp;
                    Debug.LogError("gameSave bị null, backup thành công");
                } else {
                    gameSave = new GameSave();
                    Debug.LogError("gameSave bị null, backup ko thành công. Reset data");
                }
            }
            gameSave_BackUp = gameSave;

            PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(gameSave));
            PlayerPrefs.Save();
        } catch (Exception ex) {
            Debug.LogError("Lỗi LoadData:" + ex);
        }
    }

    public void ChangeGold(int amount, string nameEvent, bool isFirebase = true, bool isReloadTextGold = true) {
        if (amount == 0) return;
        gameSave.gold += amount;
        FirebaseManager.ins.OnSetProperty("total_currency", gameSave.gold);//tổng số currency đã nhận được
        if (SceneManager.ins.txt_UIMoney != null && isReloadTextGold) SceneManager.ins.txt_UIMoney.text = gameSave.gold + "";
        if (isFirebase) {
            if (amount > 0) {
                FirebaseManager.ins.earn_virtual_currency("money", Mathf.Abs(amount), nameEvent);
            } else {
                FirebaseManager.ins.spend_virtual_currency("money", Mathf.Abs(amount), nameEvent);
            }
        }
        SaveGame();
    }

    public void LoadListCharSkin() {
        list_IDFull_ByColor = new List<int>[GameManager.ins.listColorsChar.Length];
        for (int i = 0; i < list_IDFull_ByColor.Length; i++) {
            list_IDFull_ByColor [i] = new List<int>();
        }
         Skin_Save [] listSkinFull_Case = new Skin_Save [GameManager.ins.arrayDataSkin_Full.Length];
        for (int i = 0; i < GameManager.ins.arrayDataSkin_Full.Length; i++) {
            listSkinFull_Case [i] = new Skin_Save(GameManager.ins.arrayDataSkin_Full [i].keyID);
            //Nếu có data cũ thì truyền vào
            for (int j = 0; gameSave.listSkins_Full != null && j < gameSave.listSkins_Full.Length; j++) {
                if (listSkinFull_Case [i].keyID == gameSave.listSkins_Full [j].keyID) {
                    listSkinFull_Case [i].isBuy = gameSave.listSkins_Full [j].isBuy;
                    listSkinFull_Case [i].isUnlock = gameSave.listSkins_Full [j].isUnlock;
                    listSkinFull_Case [i].amountVideAds = gameSave.listSkins_Full [j].amountVideAds;
                    listSkinFull_Case [i].play1Game_Unlock1Time = gameSave.listSkins_Full [j].play1Game_Unlock1Time;
                }
            }
            list_IDFull_ByColor [GameManager.ins.arrayDataSkin_Full [i].idColor].Add(i);
        }
        gameSave.listSkins_Full = listSkinFull_Case;

        Skin_Save [] listSkinHair_Case = new Skin_Save [GameManager.ins.arrayDataSkin_Hair.Length];
        for (int i = 0; i < GameManager.ins.arrayDataSkin_Hair.Length; i++) {
            listSkinHair_Case [i] = new Skin_Save(GameManager.ins.arrayDataSkin_Hair [i].keyID);
            //Nếu có data cũ thì truyền vào
            for (int j = 0; gameSave.listSkins_Hair != null && j < gameSave.listSkins_Hair.Length; j++) {
                if (listSkinHair_Case [i].keyID == gameSave.listSkins_Hair [j].keyID) {
                    listSkinHair_Case [i].isBuy = gameSave.listSkins_Hair [j].isBuy;
                    listSkinHair_Case [i].isUnlock = gameSave.listSkins_Hair [j].isUnlock;
                    listSkinHair_Case [i].amountVideAds = gameSave.listSkins_Hair [j].amountVideAds;
                    listSkinHair_Case [i].play1Game_Unlock1Time = gameSave.listSkins_Hair [j].play1Game_Unlock1Time;
                }
            }
        }
        gameSave.listSkins_Hair = listSkinHair_Case;

        Skin_Save [] listSkinHand_Case = new Skin_Save [GameManager.ins.arrayDataSkin_Hand.Length];
        for (int i = 0; i < GameManager.ins.arrayDataSkin_Hand.Length; i++) {
            listSkinHand_Case [i] = new Skin_Save(GameManager.ins.arrayDataSkin_Hand [i].keyID);
            //Nếu có data cũ thì truyền vào
            for (int j = 0; gameSave.listSkins_Hand != null && j < gameSave.listSkins_Hand.Length; j++) {
                if (listSkinHand_Case [i].keyID == gameSave.listSkins_Hand [j].keyID) {
                    listSkinHand_Case [i].isBuy = gameSave.listSkins_Hand [j].isBuy;
                    listSkinHand_Case [i].isUnlock = gameSave.listSkins_Hand [j].isUnlock;
                    listSkinHand_Case [i].amountVideAds = gameSave.listSkins_Hand [j].amountVideAds;
                    listSkinHand_Case [i].play1Game_Unlock1Time = gameSave.listSkins_Hand [j].play1Game_Unlock1Time;
                }
            }
        }
        gameSave.listSkins_Hand = listSkinHand_Case;

        Skin_Save [] listSkinBody_Case = new Skin_Save [GameManager.ins.arrayDataSkin_Body.Length];
        for (int i = 0; i < GameManager.ins.arrayDataSkin_Body.Length; i++) {
            listSkinBody_Case [i] = new Skin_Save(GameManager.ins.arrayDataSkin_Body [i].keyID);
            //Nếu có data cũ thì truyền vào
            for (int j = 0; gameSave.listSkins_Body != null && j < gameSave.listSkins_Body.Length; j++) {
                if (listSkinBody_Case [i].keyID == gameSave.listSkins_Body [j].keyID) {
                    listSkinBody_Case [i].isBuy = gameSave.listSkins_Body [j].isBuy;
                    listSkinBody_Case [i].isUnlock = gameSave.listSkins_Body [j].isUnlock;
                    listSkinBody_Case [i].amountVideAds = gameSave.listSkins_Body [j].amountVideAds;
                    listSkinBody_Case [i].play1Game_Unlock1Time = gameSave.listSkins_Body [j].play1Game_Unlock1Time;
                }
            }
            if (i == 0) {
                listSkinBody_Case [i].isBuy = true;
                listSkinBody_Case [i].isUnlock = true;
            }
        }
        gameSave.listSkins_Body = listSkinBody_Case;
        //Nếu đang mặc 1 bộ bị xóa thì reset CharSkin về 0
        if (gameSave.idSkin_Full >= gameSave.listSkins_Full.Length 
            || gameSave.idSkin_Hair >= gameSave.listSkins_Hair.Length
            || gameSave.idSkin_Hand >= gameSave.listSkins_Hand.Length
            || gameSave.idSkin_Body >= gameSave.listSkins_Body.Length) {
            gameSave.idSkin_Full = -1;
            gameSave.idSkin_Hair = -1;
            gameSave.idSkin_Hand = -1;
            gameSave.idSkin_Body = 0;
        }
    }

    public void LoadListMinigame() {
        MiniGame_Save[] listMinigame_Case = new MiniGame_Save[GameManager.ins.arrayDatasMinigame.Length];
        for (int i = 0; i < GameManager.ins.arrayDatasMinigame.Length; i++) {
            listMinigame_Case[i] = new MiniGame_Save(i, GameManager.ins.arrayDatasMinigame [i].minigame.ToString());
            for (int j = 0; gameSave.list_Minigames != null && gameSave.list_Minigames.Length > 0 && j < gameSave.list_Minigames.Length; j++) {
                if (GameManager.ins.listDatasMinigame_Available.Contains(GameManager.ins.arrayDatasMinigame[i]) &&  listMinigame_Case [i].keyID == gameSave.list_Minigames[j].keyID) {
                    if (gameSave.list_Minigames[j].idMapPlayed_ByRank != null) {
                        for (int k = 0; k < listMinigame_Case[i].idMapPlayed_ByRank.Length; k++) {
                            if (k < gameSave.list_Minigames[j].idMapPlayed_ByRank.Length) {
                                int amountMap = GameManager.ins.arrayDatasMinigame [i].rankMinigames [k].prefab_Maps.Length;
                                listMinigame_Case [i].idMapPlayed_ByRank[k] = Mathf.Clamp(gameSave.list_Minigames [j].idMapPlayed_ByRank [k], 0, Mathf.Max(0, amountMap - 1) );
                            }
                        }
                    }
                }
            }
        }
        gameSave.list_Minigames = listMinigame_Case;
    }
}
