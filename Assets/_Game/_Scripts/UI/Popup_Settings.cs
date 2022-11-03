using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Popup_Settings : PopupBase
{
    [Header("-----------Settings--------")]
    public Sprite on;
    public Sprite off;
    public Button btnVibrate;
    public Button btnMusic;
    public Button btnSound;

    [Header("---------Cheat---------")]
    //Cheat
    public GameObject obj_Cheat;
    public Toggle toggle_HideUI;
    public TMP_Dropdown dropdown_Minigame;
    public TMP_InputField input_Rank;
    public TMP_InputField input_Map;
    public TMP_InputField input_Level;
    public Toggle toggle_CheatGold;
    public GameObject obj_Home;
    public GameObject obj_Gameplay_BtnHomeOnMid;
    public GameObject obj_Gameplay_BtnHomeOnTop;
    public GameObject obj_Gameplay_BtnHomeDisable;

    public override void Show()
    {
        base.Show();

        if (SceneManager.ins.formGameplay != null) { 
            SceneManager.ins.isPause = SceneManager.ins.isPauseSound = true;
            obj_Home.SetActive(false);
            switch (PluginManager.ins.Setting_BtnHome) {
                case 0:
                    obj_Gameplay_BtnHomeOnMid.SetActive(true);
                    obj_Gameplay_BtnHomeOnTop.SetActive(false);
                    obj_Gameplay_BtnHomeDisable.SetActive(false);
                    break;
                case 1:
                    obj_Gameplay_BtnHomeOnMid.SetActive(false);
                    obj_Gameplay_BtnHomeOnTop.SetActive(true);
                    obj_Gameplay_BtnHomeDisable.SetActive(false);
                    break;
                case 2:
                    obj_Gameplay_BtnHomeOnMid.SetActive(false);
                    obj_Gameplay_BtnHomeOnTop.SetActive(false);
                    obj_Gameplay_BtnHomeDisable.SetActive(true);
                    break;
            }
        } else {
            obj_Home.SetActive(true);
            obj_Gameplay_BtnHomeOnMid.SetActive(false);
            obj_Gameplay_BtnHomeOnTop.SetActive(false);
            obj_Gameplay_BtnHomeDisable.SetActive(false);
        }
        
        if (GameManager.ins.isEnableCheat && obj_Cheat != null)
        {
            obj_Cheat.gameObject.SetActive(true);
            //Ản UI
            toggle_HideUI.isOn = GameManager.ins.isHideUIGameplay;
            input_Rank.text = (GameManager.ins.rankMinigame + 1) + "";
            input_Map.text = (GameManager.ins.mapMinigame + 1) + "";
            //Hiện Level cheat
            input_Level.text = (DataManager.ins.gameSave.level + 1) + "";
            toggle_CheatGold.isOn = false;
        }
        else if (obj_Cheat != null)
        {
            obj_Cheat.gameObject.SetActive(false);
        }

        btnSound.GetComponent<Image>().sprite = DataManager.ins.gameSave.volumeSound > 0 ? on : off;
        btnVibrate.GetComponent<Image>().sprite = DataManager.ins.gameSave.isVibrate ? on : off;
        btnMusic.GetComponent<Image>().sprite = DataManager.ins.gameSave.volumeMusic > 0 ? on : off;
    }

    #region BUTTON
    public void BtnReloadMusic()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (DataManager.ins.gameSave.volumeMusic > 0)
        {
            DataManager.ins.gameSave.volumeMusic = 0;
            btnMusic.image.sprite = off;
        }
        else
        {
            DataManager.ins.gameSave.volumeMusic = 80;
            btnMusic.image.sprite = on;
        }
        DataManager.ins.SaveGame();
        if (SoundManager.ins != null)
            SoundManager.ins.ReloadMusic();
    }


    public void BtnReloadSound()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (DataManager.ins.gameSave.volumeSound > 0)
        {
            DataManager.ins.gameSave.volumeSound = 0;
            btnSound.image.sprite = off;
        }
        else
        {
            DataManager.ins.gameSave.volumeSound = 80;
            btnSound.image.sprite = on;
        }
        DataManager.ins.SaveGame();
    }

    public void BtnVibration()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (DataManager.ins.gameSave.isVibrate)
        {
            btnVibrate.image.sprite =off;
            DataManager.ins.gameSave.isVibrate = false;
        }
        else
        {
            btnVibrate.image.sprite = on;
            DataManager.ins.gameSave.isVibrate = true;
        }
    }

    public void BtnClose()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if(SceneManager.ins.formGameplay != null) SceneManager.ins.isPause = SceneManager.ins.isPauseSound = false;
            Close();
        DataManager.ins.SaveGame();
    }

    public void BtnHome()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (SceneManager.ins.formGameplay != null) SceneManager.ins.isPause = SceneManager.ins.isPauseSound = false;
        MaxManager.ins.ShowInterstitial("Home_Setting", () => {
            SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
        });
    }
    #endregion
    #region Cheat
    public void Btn_HideUI()
    {
       GameManager.ins.isHideUIGameplay = toggle_HideUI.isOn;
        //if (SceneManager.ins.formCurrent != null) SceneManager.ins.formCurrent.GetComponent<CanvasGroup>().alpha = GameManager.ins.isHideUIGameplay ? 0 : 1;
        //if (SceneManager.ins.loadingCanvas != null) SceneManager.ins.loadingCanvas.GetComponent<CanvasGroup>().alpha = GameManager.ins.isHideUIGameplay ? 0 : 1;
    }
    public void Btn_CheatGold() {
        if (toggle_CheatGold.isOn &&DataManager.ins.gameSave.gold <99999 ) DataManager.ins.ChangeGold(99999,"",false);
        //if (SceneManager.ins.formCurrent != null) SceneManager.ins.formCurrent.GetComponent<CanvasGroup>().alpha = GameManager.ins.isHideUIGameplay ? 0 : 1;
        //if (SceneManager.ins.loadingCanvas != null) SceneManager.ins.loadingCanvas.GetComponent<CanvasGroup>().alpha = GameManager.ins.isHideUIGameplay ? 0 : 1;
    }

    public void Btn_CheatMinigame() {
        try {
            int minigame = int.Parse(dropdown_Minigame.options [dropdown_Minigame.value].text);
            GameManager.ins.levelCheat = (Minigame) minigame;
        } catch (System.Exception e) {
            Debug.LogError("Loi cheat Minigame:" + e.ToString());
        }
    }
    public void Btn_CheatRank() {
        try {
            int curRank = int.Parse(input_Rank.text) - 1;
            GameManager.ins.rankMinigame = curRank;
        } catch (System.Exception e) {
            Debug.LogError("Loi cheat Rank:" + e.ToString());
        }
    }
    public void Btn_CheatMap() {
        try {
            int curMap = int.Parse(input_Map.text) - 1;
            GameManager.ins.mapMinigame = curMap;
        } catch (System.Exception e) {
            Debug.LogError("Loi cheat Map:" + e.ToString());
        }
    }

    public void Btn_CheatLevel()
    {
        try
        {
            int curLevel = int.Parse(input_Level.text) - 1;
            DataManager.ins.gameSave.level = curLevel;
            int amountNode = (int)Mathf.Clamp(PluginManager.ins.NodeStart_WinAll + DataManager.ins.gameSave.amountWinAll * PluginManager.ins.NodeIncease_WinAll, 0, 8);
            if (curLevel >= amountNode) {
                DataManager.ins.gameSave.level = (int)amountNode - 1;
                input_Level.text = "" + (DataManager.ins.gameSave.level +1);
            }
            if (curLevel < 0) {
                DataManager.ins.gameSave.level = 0;
                input_Level.text = "" + (DataManager.ins.gameSave.level +1);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Loi cheat Level:" + e.ToString());
        }
    }
    #endregion
    #region btn new
    //[Header("------NEW UI------")]
    //public Button btn_sound;
    //public Button btn_vibra;
    //public Button btn_music;
    public void Btn_Sound()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (DataManager.ins.gameSave.volumeSound > 0)
        {
            DataManager.ins.gameSave.volumeSound = 0;
            //btn_sound.image.sprite = soundOff;
        }
        else
        {
            DataManager.ins.gameSave.volumeSound = 80;
            //btn_sound.image.sprite = soundOn;
        }
        DataManager.ins.SaveGame();
    }
    public void Btn_Vibration()
    {
        Debug.Log("dasdasfqw");
        SoundManager.ins.sound_Click.PlaySound();
        if (DataManager.ins.gameSave.isVibrate)
        {
            //btn_vibra.image.sprite = vibraOff;
            DataManager.ins.gameSave.isVibrate = false;
        }
        else
        {
            //btn_vibra.image.sprite = vibraOn;
            DataManager.ins.gameSave.isVibrate = true;
        }
    }
    public void Btn_Music()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (DataManager.ins.gameSave.volumeMusic > 0)
        {
            DataManager.ins.gameSave.volumeMusic = 0;
            //btn_music.image.sprite = musicOff;
        }
        else
        {
            DataManager.ins.gameSave.volumeMusic = 80;
            //btn_music.image.sprite = musicOn;
        }
        DataManager.ins.SaveGame();
        if (SoundManager.ins != null)
            SoundManager.ins.ReloadMusic();
    }
    #endregion
}
