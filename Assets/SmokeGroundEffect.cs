using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGroundEffect : MonoBehaviour, IObjectPooler
{
    public ParticleSystem particleSmoke;
    public void OnSpawnPool()
    {
        particleSmoke.Play();
    }
}
