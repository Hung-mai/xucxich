using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_Sound : MonoBehaviour
{
    public int id = 0;
    public Text text_Sound;
    public AudioSource audioSource;

    public Data_Sound[] listAllSound;

    private void OnEnable() {
        text_Sound.text = id + "  " + listAllSound[id].clip.name;
    }

    public void BtnNext() {
        id++;
        id = Mathf.Clamp(id, 0, listAllSound.Length);
        text_Sound.text =  id + "  " + listAllSound[id].clip.name;
    }

    public void BtnBack() {
        id--;
        id = Mathf.Clamp(id, 0, listAllSound.Length);
        text_Sound.text = id + "  " + listAllSound[id].clip.name;
    }

    public void BtnPlay() {
        audioSource.clip = listAllSound[id].clip;
        audioSource.volume = listAllSound[id].volume*0.5f;
        audioSource.pitch = listAllSound[id].pitch;
        audioSource.loop = listAllSound[id].isLoop;
        audioSource.Play();
    }
}
