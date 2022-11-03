using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkewerScurry_Ref : MonoBehaviour
{
    public static SkewerScurry_Ref ins;
    private void Awake()
    {
        ins = this;
    }
    public Character player;
}
