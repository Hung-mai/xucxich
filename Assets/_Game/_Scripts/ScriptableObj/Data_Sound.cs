using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data_Sound", menuName = "Assets/Data_Sound", order = 0)]
public class Data_Sound : ScriptableObject {
    public AudioClip clip;
    public float volume = 1;
    public float delay = 0;
    public float pitch = 1;
    public bool isLoop = false;
}
