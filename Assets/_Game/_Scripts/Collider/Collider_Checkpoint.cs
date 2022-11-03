using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_Checkpoint : MonoBehaviour
{
    public int idCheckpoint = 0;
    private void OnTriggerEnter(Collider other) {
        Character character = other.GetComponent<Character>();
        if (character != null && character.isAlive) character.OnTriggerEnter_Checkpoint(idCheckpoint);
    }
}
