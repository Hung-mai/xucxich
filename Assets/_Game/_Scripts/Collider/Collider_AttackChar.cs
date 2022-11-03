using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_AttackChar : MonoBehaviour
{
    float lifeTime = 0.1f;
    [HideInInspector]
    public Character charAttacker;
    private void OnEnable() {
        Timer.Schedule(this, lifeTime, () => { gameObject.SetActive(false); });
    }

    private void OnTriggerEnter(Collider other) {
        Character charDamaged = other.GetComponent<Character>();
        if (charDamaged != null)  
            charDamaged.OnTriggerEnter_Damaged(charAttacker);
    }
}
