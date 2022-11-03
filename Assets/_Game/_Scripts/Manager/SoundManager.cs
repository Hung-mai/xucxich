using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>{
    public AudioSource audioSource;
    public Data_Sound dataMusic;

    [Header("--------------- Sound dùng nhiều --------")]
    public SoundObject sound_Click;

    public void ReloadMusic() {
        if(dataMusic != null && DataManager.ins != null && DataManager.ins.gameSave != null && dataMusic.clip != null) {
            audioSource.clip = dataMusic.clip;
            audioSource.volume = dataMusic.volume * DataManager.ins.gameSave.volumeMusic * 0.01f;
            audioSource.Play();
        }
    }
}
