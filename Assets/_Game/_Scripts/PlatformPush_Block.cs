using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlatformPush_Block : MonoBehaviour {
    public int id = -1;
    public bool isFlicker = false;
    public bool isFell = false;
    public Transform tran_MeshBlock;
    public MeshRenderer meshRenderer_Block;

    public void StartEffectFlicker(Color colorFallBlock, float durationFlicker) {
        StartCoroutine(EffectFlicker(meshRenderer_Block.material.color, colorFallBlock, durationFlicker));
    }

    IEnumerator EffectFlicker(Color colorOrigin, Color colorFallBlock, float durationFlicker) {
        meshRenderer_Block.material.DOColor(colorFallBlock, durationFlicker / 3);
        yield return new WaitForSeconds(durationFlicker / 3);
        meshRenderer_Block.material.DOColor(colorOrigin, durationFlicker / 3);
        yield return new WaitForSeconds(durationFlicker / 3);
        meshRenderer_Block.material.DOColor(colorFallBlock, durationFlicker / 3);
        yield return new WaitForSeconds(durationFlicker / 3);
    }
}
