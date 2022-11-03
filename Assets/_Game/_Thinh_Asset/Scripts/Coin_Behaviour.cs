using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Behaviour : MonoBehaviour
{
    public Rigidbody rb;
    public float spdFall;
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(rb.velocity.x,-spdFall);
    }
    public void Start_Kine()
    {
        StartCoroutine(I_Kine());
    }
    IEnumerator I_Kine()
    {
        yield return new WaitForSeconds(2f);
        rb.isKinematic = true;
    }
}
