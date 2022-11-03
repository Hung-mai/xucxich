using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider_ChangeCamera : MonoBehaviour
{
    public int idStateCamera = 0;
    private void OnTriggerEnter(Collider other) {
        Player character = other.GetComponent<Player>();
        if (character != null && character.isAlive) character.OnTriggerEnter_ChangeCamera(idStateCamera);
    }
}
