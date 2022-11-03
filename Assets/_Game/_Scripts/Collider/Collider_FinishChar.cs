using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_FinishChar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        Character character = other.GetComponent<Character>();
        if (character != null)  character.OnTriggerEnter_Finish();
    }
}
