using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject_Controll : MonoBehaviour
{
    public AudioSource audioSource;
    public float delayView = 0;
    public float volume = -1;

    public void PlaySound(Data_Sound dataSound) {
        if(dataSound != null && dataSound.clip != null && DataManager.ins != null && DataManager.ins.gameSave != null) {
            audioSource.clip = dataSound.clip;
            audioSource.volume = (volume > 0 ? volume : dataSound.volume) * DataManager.ins.gameSave.volumeSound * 0.01f;
            delayView = dataSound.delay;
            audioSource.pitch = dataSound.pitch;
            audioSource.loop = dataSound.isLoop;
            audioSource.PlayDelayed(delayView);
        } else {
            audioSource.Stop();
        }
    }

    public void StopSound() {
        audioSource.Stop();
    }
}
