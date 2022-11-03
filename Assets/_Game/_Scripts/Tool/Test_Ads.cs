using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Ads : FormBase
{
    // Start is called before the first frame update
    void Start()
    {
        DataManager.ins.LoadData();
    }

    public void BtnLoadInter() {
        MaxManager.ins.ShowInterstitial();
    }

    public void BtnLoadBanner() {
        MaxManager.ins.ShowBanner();
    }
}
