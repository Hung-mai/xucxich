using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake ins;
    public float duration = 0.5f;
    public float streng = 0.02f;
    private Camera cam;
    private void Awake()
    {
        ins = this;
        cam = GetComponent<Camera>();
    }
    [ContextMenu("Shake")]
    public void Shake()
    {
        cam.DOShakePosition(duration, streng, fadeOut: true);
        cam.DOShakeRotation(duration, streng, fadeOut: true);
    }
}
