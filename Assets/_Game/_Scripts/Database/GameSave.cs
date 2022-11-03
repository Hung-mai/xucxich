
using System;
using System.Collections.Generic;

[System.Serializable]
public class GameSave
{
    //-------- State --------
    public bool isNew = true;
    public int volumeMusic = 80;
    public int volumeSound = 80;
    public bool isVibrate = true;
    public bool isNoAds = false;
    public int starRate = -1;
    public int amountWin_AfterShowInter = 0;
    public int amountLose_AfterShowInter = 0;

    //-------- Value --------
    public int gold = 0;
    public int level = 0;//Level hiện tại
    public int levelStart = 0;//số lần vào chơi minigame (Chơi lại vẫn bắn)
    public int levelEnded = 0;
    public int levelPass = 0;//Level mà user đã vượt qua (Win Level hoặc Skip Level)
    public int amountFailInLevelCur = 0;//Số lần thua ở Level hiện tại
    public int amountWinAll = 0;
    public int idColor = 0;
    public string username = "Player #431";
    //public string nextSpin = string.Empty;

    //-------- Minigame --------
    public int idRandomMinigame = -1;
    public bool isRandomMinigameNext = true;
    public MiniGame_Save[] list_Minigames;
    public float difficulty = 1;

    //-------- Skin Offer End Game --------
    public float progressSkin = 0;
    public Skin_Save currentSkinUnlock;
    public int countTimeShowOffer_EndGame = 0;

    //------- Skin body offer after selectminigame
    public Skin_Save lastSkinBodyShow;

    public int idSkin_Full;//Bộ skin của nhân vật
    public int idSkin_Hair;
    public int idSkin_Hand;
    public int idSkin_Body;
    public Skin_Save [] listSkins_Full;
    public Skin_Save [] listSkins_Hair;
    public Skin_Save [] listSkins_Hand;
    public Skin_Save [] listSkins_Body;

    //-------- Firebase --------
    public int timeInstall;//Thời điểm cài game
    public int timeLastOpen;//Thời điểm cuối cùng mở game
    public int timeOfflineEarning;//Thời nhận Earning
    public int dayNoAds;//Ngày bật NoAds
    public int secondNoAds;//Giây bật NoAds
    public bool showSpinWinAll;
    public bool showFistOpenSpin;
    //public int daySpin;
    //public int secondSpin;
    public int daysPlayed = 0;//Số ngày đã User có mở game lên
    public int totalDays = 0;//Ngày hiện tại - Ngày cài đặt
    public int totalSession = 0;//Tống số session
    public bool[] firebase_retent_type = new bool[8];//Lưu lại retention của những ngày đã bắn

    //-------- Other --------
    public GameSave()
    {
        isNew = true;
        volumeMusic = 80;
        volumeSound = 80;
        isVibrate = true;
        isNoAds = false;
        starRate = -1;
        amountWin_AfterShowInter = 0;
        amountLose_AfterShowInter = 0;

        gold = 0;
        level = 0;
        levelStart = 0;
        levelEnded = 0;
        levelPass = 0;
        amountWinAll = 0;
        idColor = 0; 
        username = "Player #431";

        idRandomMinigame = -1;
        isRandomMinigameNext = true;
        difficulty = 1;


        progressSkin = 0;
        currentSkinUnlock = new Skin_Save("");
        countTimeShowOffer_EndGame = 0;

        lastSkinBodyShow = new Skin_Save("");

        idSkin_Full = -1;
        idSkin_Hair = -1;
        idSkin_Hand = -1;
        idSkin_Body = 0;

        //Time
        timeLastOpen = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalDays;
        timeInstall = timeLastOpen;
        timeOfflineEarning = timeLastOpen;
        dayNoAds = 0;
        secondNoAds = 0;
        showSpinWinAll = false;
        showFistOpenSpin = true;
        //daySpin = 0;
        //secondSpin = 0;
        daysPlayed = 1;
        totalDays = 0;
        totalSession = 0;
        firebase_retent_type = new bool[8];
    }
}

[System.Serializable]
public class MiniGame_Save {
    public int id;
    public string keyID;//ID để sau này đổi thứ tự thì vẫn lấy đc data cũ
    public int[] idMapPlayed_ByRank;//VD: User đang ở Rank 1: đang chơi đến map 3 => idMapPlayed_ByRank[1] = 3. Còn Rank 0: đang chơi đến map  1 => idMapPlayed_ByRank[0] = 1. Nếu tụt về Rank 0 thì chơi map 1, chứ ko bị chơi map lặp lại.
    public MiniGame_Save(int id_Minigame, string key_Minigame) {
        id = id_Minigame;
        keyID = key_Minigame;
        idMapPlayed_ByRank = new int[Constants.MAX_RANK_EACH_MINIGAME];
    }
}

[System.Serializable]
public class Skin_Save {
    public string keyID;//ID để sau này đổi thứ tự vũ khí thì vẫn lấy đc data cũ

    //Những data sẽ thay đổi trong game
    public bool isUnlock;
    public bool isBuy;
    public int amountVideAds;//Số VideoAds đã xem để unlock 1 lần
    public bool play1Game_Unlock1Time;//Đã dùng hết unlock 1 lần chưa

    public Skin_Save(string key_Skin) {
        isUnlock = false;
        isBuy = false;
        amountVideAds = 0;
        play1Game_Unlock1Time = false;
        keyID = key_Skin;
    }
}
