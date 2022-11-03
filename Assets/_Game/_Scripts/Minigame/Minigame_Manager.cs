using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Minigame_Manager : MonoBehaviour {
    public Minigame minigame;
    [Header("--------------- Trạng thái của Gameplay ---------------")]
    public bool isGameplay_Intro = false;
    public bool isGameplay_Start = false;
    public bool isGameplay_End = false;

    public float speedChar = 60;
    public int maxCharacter = 10;
    public Vector2 timeAI_Idle = new Vector2(0.2f, 1.5f);
    public Vector2 timeAI_Run = new Vector2(2, 6);
    public int pointIntelligence_StartMin = 20;
    public int pointIntelligence_StartRange = 60;//VD:Min:20,Range:60 -> PointIntelligence[20,80]
    public int amountAIReviveFree = 0;
    public bool isPlayerReviveFree = false;
    public bool hasCharFinishLine = false;
    public Color colorFog;
    public List<Character> listChars;
    public AIBot prefab_AIBot;
    public Transform [] nodes_Start;
    public Indicator prefab_Indicator;
    public List<Indicator> listIndicators;
    public GameObject prefab_EffectDamaged;
    public List<GameObject> list_EffectsDamaged;
    public List<Character> listCharsWon;

    public virtual void Awake() {
        if(colorFog != Color.clear) {
            RenderSettings.fog = true;
            RenderSettings.fogColor = colorFog;
        } else {
            RenderSettings.fog = false;
        }
    }


    public virtual void Create_EffectDamaged(Vector3 position){
        GameObject effectsDamaged = null;
        for (int i = 0; i < list_EffectsDamaged.Count; i++) {
            if (!list_EffectsDamaged[i].activeSelf) {
                effectsDamaged = list_EffectsDamaged [i];
                break;
            }
        }
        if(effectsDamaged == null) {
            effectsDamaged = Instantiate(prefab_EffectDamaged);
            list_EffectsDamaged.Add(effectsDamaged);
        }
        effectsDamaged.transform.position = position;
        effectsDamaged.SetActive(true);
    }

    public abstract void InitGame();

    public abstract void StartGame();

    public abstract void EndGame();
}
