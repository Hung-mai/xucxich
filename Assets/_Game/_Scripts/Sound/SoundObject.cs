using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    public int id = 0;
    public float delay = -1;
    public bool playOnAwake = false;
    public bool playRandom = false;
    public Data_Sound[] listDataSound;

    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable() {
        if (playRandom) id = Random.Range(0, listDataSound.Length);
        if (listDataSound != null && id < listDataSound.Length && listDataSound[id].clip != null && DataManager.ins != null && DataManager.ins.gameSave != null)
        {
            audioSource.clip = listDataSound[id].clip;
            audioSource.volume = listDataSound[id].volume * DataManager.ins.gameSave.volumeSound * 0.01f;
            audioSource.pitch = listDataSound[id].pitch;
            audioSource.loop = listDataSound[id].isLoop;
        }
        if (playOnAwake) {
            if (delay >0) {
                Timer.Schedule(this, delay, () => {
                    PlaySound(id);
                });
            } else {
                PlaySound(id);
            }
        }
    }

    public void PlaySound(int id = 0,float pitch = -1) {
        if(listDataSound != null && id < listDataSound.Length && listDataSound[id].clip != null && DataManager.ins != null && DataManager.ins.gameSave != null) {
            audioSource.clip = listDataSound[id].clip;
            audioSource.volume = listDataSound[id].volume * DataManager.ins.gameSave.volumeSound * 0.01f;
            audioSource.pitch = pitch >= 0 ? pitch : listDataSound[id].pitch;
            audioSource.loop = listDataSound[id].isLoop;
            audioSource.PlayDelayed(listDataSound[id].delay);
        }
    }
    public void Pause() {
        audioSource.Pause();
    }

    public void UnPause() {
        audioSource.UnPause();
    }

    public void Enable() {
        audioSource.enabled = true;
    }

    public void Disable() {
        audioSource.enabled = false;
    }
}
