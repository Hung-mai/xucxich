using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_Balloon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Collider_AttackChar collider_AttackChar = other.GetComponent<Collider_AttackChar>();
        if (collider_AttackChar != null && collider_AttackChar.charAttacker.isAlive) collider_AttackChar.charAttacker.OnTriggerEnter_Balloon();
    }
}
