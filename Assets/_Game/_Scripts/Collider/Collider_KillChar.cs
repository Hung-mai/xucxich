using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_KillChar : MonoBehaviour
{
    public ColliderKillChar colliderKillChar;
    public bool isGetComponent_Collider_AttackChar = false;
    public Action action;
    public Collider collider;

    private void OnTriggerEnter(Collider other) {
        if (isGetComponent_Collider_AttackChar) {
            Collider_AttackChar collider_AttackChar = other.GetComponent<Collider_AttackChar>();
            if (collider_AttackChar != null && collider_AttackChar.charAttacker.timeUndying <= 0) {
                collider_AttackChar.charAttacker.OnTriggerEnter_Die(colliderKillChar);
                action?.Invoke();
            }
        } else {
            Character character = other.GetComponent<Character>();
            if (character != null) {
                if(colliderKillChar == ColliderKillChar.Water || colliderKillChar == ColliderKillChar.Bomb || character.timeUndying < 0) {
                    character.OnTriggerEnter_Die(colliderKillChar);
                    action?.Invoke();
                } 
            }
        }
    }

    public void AniEvent_EnableCollider() {
        collider.enabled = true;
    }

    public void AniEvent_DisableCollider() {
        collider.enabled = false;
    }
}
