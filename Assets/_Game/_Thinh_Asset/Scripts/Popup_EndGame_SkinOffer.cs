using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class Popup_EndGame_SkinOffer : MonoBehaviour
{
    public enum Skin
    {
        hair,
        hand,
    }
    [HideInInspector]public Skin skin = Skin.hair;
    public Popup_EndGame endGame;
    public Data_Skin[] datasSkin;
    public Image parentMain;
    public Image main;
    public Action OnProgressComplete;
    [HideInInspector]public Data_Skin currentSkin;

    [SerializeField] private float timeRunProcess = 2.8f;

    #region skin hair
    public void ShowOffer()
    {
        #region check da so huu het hand va hair chua
        if (skin == Skin.hair || DataManager.ins.gameSave.currentSkinUnlock.keyID.Contains("Hair"))
        {
            //check xem co phai no so huu het roi ko
            bool isOwnAllHair = true;
            foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hair)
            {
                if (!s.isBuy)
                {
                    isOwnAllHair = false;
                    break;
                }
            }
            if (isOwnAllHair)
            {
                skin = Skin.hand;
                DataManager.ins.gameSave.currentSkinUnlock = new Skin_Save("");

                //check all hand and hair
                bool isOwnAll = true;
                foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hand)
                {
                    if (!s.isBuy)
                    {
                        isOwnAll = false;
                        break;
                    }
                }
                if (isOwnAll)
                {
                    parentMain.gameObject.SetActive(false);
                    main.gameObject.SetActive(false);
                    endGame.txt_progress.gameObject.SetActive(false);
                    DataManager.ins.gameSave.progressSkin = 0;
                    endGame.ShowButtonOffer();
                    return;
                }
            }
            
        }
        else if (skin == Skin.hand || DataManager.ins.gameSave.currentSkinUnlock.keyID.Contains("hand"))
        {
            //check xem co phai no so huu het roi ko
            bool isOwnAllHand = true;
            foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hand)
            {
                if (!s.isBuy)
                {
                    isOwnAllHand = false;
                    break;
                }
            }
            if (isOwnAllHand)
            {
                skin = Skin.hair;
                DataManager.ins.gameSave.currentSkinUnlock = new Skin_Save("");

                //check all hand and hair
                bool isOwnAll = true;
                foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hair)
                {
                    if (!s.isBuy)
                    {
                        isOwnAll = false;
                        break;
                    }
                }
                if (isOwnAll)
                {
                    parentMain.gameObject.SetActive(false);
                    main.gameObject.SetActive(false);
                    endGame.txt_progress.gameObject.SetActive(false);
                    DataManager.ins.gameSave.progressSkin = 0;
                    endGame.ShowButtonOffer();
                    return;
                }
            }
            
        }
        
        #endregion


        if (DataManager.ins.gameSave.currentSkinUnlock.keyID == "" )//lần đầu vào game chưa có gì
        {
            if (DataManager.ins.gameSave.countTimeShowOffer_EndGame < datasSkin.Length)//số lần show offer chưa vượt quá số skin set trước
            {//số lần mở chưa vượt quá ds xếp trước -> show theo thứ tự 
                Data_Skin d = datasSkin[DataManager.ins.gameSave.countTimeShowOffer_EndGame];
                if (!CheckDataSkinIsOwn(d))//chưa sở hữu
                {
                    parentMain.sprite = d.icon;
                    main.sprite = d.icon;
                    #region run process
                    main.fillAmount = endGame.currentProcess / 100;
                    endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                    //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                    #endregion
                    currentSkin = d;
                    SaveProcess(currentSkin.keyID);
                }
                else//đã sở hữu thì kiểm tra cái tiếp theo
                {
                    bool a = true;
                    while (a)
                    {
                        d = datasSkin[DataManager.ins.gameSave.countTimeShowOffer_EndGame];
                        a = CheckDataSkinIsOwn(d);

                        if (d.type == CharSkin_Type.Hat) skin = Skin.hair;
                        else if (d.type == CharSkin_Type.Hand) skin = Skin.hand;
                        
                        DataManager.ins.gameSave.countTimeShowOffer_EndGame++;
                        if (DataManager.ins.gameSave.countTimeShowOffer_EndGame >= datasSkin.Length)
                        {
                            break;
                        }
                    }
                    if (a)
                    {
                        //cho random vì đã sở hữu hết trong list rồi
                        List<Data_Skin> ds = SkinNotOwns(skin);
                        currentSkin = ds[UnityEngine.Random.Range(0, ds.Count)];
                        parentMain.sprite = currentSkin.icon;
                        main.sprite = currentSkin.icon;
                        #region run process
                        main.fillAmount = endGame.currentProcess / 100;
                        endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                        //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                        #endregion
                        SaveProcess(currentSkin.keyID);
                    }
                    else// lấy cái tiếp theo trong list mà chưa sở hữu
                    { //lấy d = datasSkin[DataManager.ins.gameSave.countTimeShowOffer];
                        parentMain.sprite = d.icon;
                        main.sprite = d.icon;
                        #region run process
                        main.fillAmount = endGame.currentProcess / 100;
                        endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                        //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                        #endregion
                        currentSkin = d;
                        SaveProcess(currentSkin.keyID);
                    }
                }
            }
            else //if(DataManager.ins.gameSave.currentSkinHairUnlock.keyID != "")
            {//số lần mở đã vượt quá -> random hoặc là list datasskin k có gì => mode random



                List<Data_Skin> ds = SkinNotOwns(skin);
                currentSkin = ds[UnityEngine.Random.Range(0, ds.Count)];
                parentMain.sprite = currentSkin.icon;
                main.sprite = currentSkin.icon;

                #region run process
                main.fillAmount = endGame.currentProcess / 100;
                endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                #endregion
                SaveProcess(currentSkin.keyID);
            }
        }
        else // đang có skin open dở
        {
            if (DataManager.ins.gameSave.currentSkinUnlock.keyID.Contains("Hair"))
            {
                //check xem nó đã sở hữu chưa , lỡ ng chơi có mua
                foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hair)
                {
                    if (s.keyID == DataManager.ins.gameSave.currentSkinUnlock.keyID && s.isBuy)
                    {
                        List<Data_Skin> ds = SkinNotOwns(Skin.hair);
                        
                        currentSkin = ds[UnityEngine.Random.Range(0, ds.Count)];
                        parentMain.sprite = currentSkin.icon;
                        main.sprite = currentSkin.icon;

                        #region run process
                        main.fillAmount = endGame.currentProcess / 100;
                        endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                        //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                        #endregion
                        SaveProcess(currentSkin.keyID);
                        return;
                    }
                }

                //lấy data cũ ra
                foreach (Data_Skin d in GameManager.ins.arrayDataSkin_Hair)
                {
                    if (d.keyID == DataManager.ins.gameSave.currentSkinUnlock.keyID)
                    {
                        parentMain.sprite = d.icon;
                        main.sprite = d.icon;
                        #region run process
                        main.fillAmount = endGame.currentProcess / 100;
                        endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                        //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                        #endregion
                        currentSkin = d;
                        SaveProcess(currentSkin.keyID);
                        break;
                    }
                }
            }
            else if (DataManager.ins.gameSave.currentSkinUnlock.keyID.Contains("hand"))
            {
                //check xem nó đã sở hữu chưa , lỡ ng chơi có mua
                foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hand)
                {
                    if (s.keyID == DataManager.ins.gameSave.currentSkinUnlock.keyID && s.isBuy)
                    {
                        List<Data_Skin> ds = SkinNotOwns(Skin.hand);
                        currentSkin = ds[UnityEngine.Random.Range(0, ds.Count)];
                        parentMain.sprite = currentSkin.icon;
                        main.sprite = currentSkin.icon;

                        #region run process
                        main.fillAmount = endGame.currentProcess / 100;
                        endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                        //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                        #endregion
                        SaveProcess(currentSkin.keyID);
                        return;
                    }
                }

                //lấy data cũ ra
                foreach (Data_Skin d in GameManager.ins.arrayDataSkin_Hand)
                {
                    if (d.keyID == DataManager.ins.gameSave.currentSkinUnlock.keyID)
                    {
                        parentMain.sprite = d.icon;
                        main.sprite = d.icon;
                        #region run process
                        main.fillAmount = endGame.currentProcess / 100;
                        endGame.txt_progress.text = Convert.ToInt16(endGame.currentProcess).ToString() + "%";
                        //RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin /100 , timeRunProcess);
                        #endregion
                        currentSkin = d;
                        SaveProcess(currentSkin.keyID);
                        break;
                    }
                }
            }

        }
        
    }
    private bool CheckDataSkinIsOwn(Data_Skin d)
    {
        if (d.type == CharSkin_Type.Hat)
        {
            foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hair)
            {
                if (d.keyID == s.keyID && s.isBuy)
                {
                    return true;
                }
            }
            return false;
        }
        else if (d.type == CharSkin_Type.Hand)
        {
            foreach (Skin_Save s in DataManager.ins.gameSave.listSkins_Hand)
            {
                if (d.keyID == s.keyID && s.isBuy)
                {
                    return true;
                }
            }
            return false;
        }



        else
        {
            Debug.LogError("CheckDataSkinIsOwn bi loi~");
            return false;
        }
        
    }
    private List<Data_Skin> SkinNotOwns(Skin s)
    {
        if (s == Skin.hair)
        {
            //kiểm tra skin chưa sở hữu và k phải trong list datasSkin Hair
            List<Data_Skin> dataskins = new List<Data_Skin>();
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Hair.Length; i++)
            {
                if (!DataManager.ins.gameSave.listSkins_Hair[i].isBuy)
                {
                    dataskins.Add(GameManager.ins.arrayDataSkin_Hair[i]);
                }
            }
            //return dataskins;
            List<Data_Skin> final_dataskins = new List<Data_Skin>();
            foreach (Data_Skin d in dataskins)
            {
                bool isOwn = false;
                foreach (Data_Skin d1 in datasSkin)
                {
                    if (d.keyID == d1.keyID)
                    {
                        isOwn = false;
                        break;
                    }
                    isOwn = true;
                }
                if (isOwn || datasSkin.Length == 0) final_dataskins.Add(d);
            }
            return final_dataskins;
        }
        else if (s == Skin.hand)
        {
            //kiểm tra skin chưa sở hữu và k phải trong list datasSkin Hand
            List<Data_Skin> dataskins = new List<Data_Skin>();
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Hand.Length; i++)
            {
                if (!DataManager.ins.gameSave.listSkins_Hand[i].isBuy)
                {
                    dataskins.Add(GameManager.ins.arrayDataSkin_Hand[i]);
                }
            }
            //return dataskins;
            List<Data_Skin> final_dataskins = new List<Data_Skin>();
            foreach (Data_Skin d in dataskins)
            {
                bool isOwn = false;
                foreach (Data_Skin d1 in datasSkin)
                {
                    if (d.keyID == d1.keyID)
                    {
                        isOwn = false;
                        break;
                    }
                    isOwn = true;
                }
                if (isOwn || datasSkin.Length == 0) final_dataskins.Add(d);
            }
            return final_dataskins;
        }
        else
        {
            return null;
        }
    }
    private void RunProcess(float original, float target, float time)
    {
        main.fillAmount = original; 
        endGame.txt_progress.text = Convert.ToInt16(original * 100).ToString() + "%";
        Timer.Schedule(this,1.2f,()=> {
            main.DOFillAmount(target, time).SetEase(Ease.Linear).OnUpdate(() => {
                endGame.txt_progress.text = Convert.ToInt16(main.fillAmount * 100).ToString() + "%";
            }).OnComplete(() => {
                OnProgressComplete();
            });
        });
        
    }
    public void CallRunProcess()
    {
        RunProcess(endGame.currentProcess / 100, DataManager.ins.gameSave.progressSkin / 100, timeRunProcess);
    }

    private void SaveProcess(string keyId)
    {
        if (currentSkin.keyID == "") return;
        DataManager.ins.gameSave.currentSkinUnlock = new Skin_Save(keyId);
    }
    public void GetSkin()
    {
        if (currentSkin == null) return;
        if (currentSkin.type == CharSkin_Type.Hat)
        {
            bool isGetSuccess = false;
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Hair.Length; i++)
            {
                if (DataManager.ins.gameSave.listSkins_Hair[i].keyID == currentSkin.keyID)
                {
                    DataManager.ins.gameSave.listSkins_Hair[i].play1Game_Unlock1Time = false;
                    DataManager.ins.gameSave.listSkins_Hair[i].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Hair[i].isBuy = true;

                    //mặc vào cho player luôn
                    DataManager.ins.gameSave.idSkin_Hair = i;
                    isGetSuccess = true;
                }
            }
            if (!isGetSuccess) Debug.LogError("Ko thể tìm thấy SkinOffer phù hợp");
        }
        else if(currentSkin.type == CharSkin_Type.Hand)
        {
            bool isGetSuccess = false;
            for (int i = 0; i < DataManager.ins.gameSave.listSkins_Hand.Length; i++)
            {
                if (DataManager.ins.gameSave.listSkins_Hand[i].keyID == currentSkin.keyID)
                {
                    DataManager.ins.gameSave.listSkins_Hand[i].play1Game_Unlock1Time = false;
                    DataManager.ins.gameSave.listSkins_Hand[i].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Hand[i].isBuy = true;

                    //mặc vào cho player luôn
                    DataManager.ins.gameSave.idSkin_Hand = i;
                    isGetSuccess = true;
                }
            }
            if (!isGetSuccess) Debug.LogError("Ko thể tìm thấy SkinOffer phù hợp");
        }
        
    }
    public void IncreaseDataCountTimeOffer()
    {
        DataManager.ins.gameSave.countTimeShowOffer_EndGame++;
    }
    /*
     * cuối mỗi level nó sẽ gọi ra 
     * random ra 1 skin hair trong shop
     * lưu vào trong 
     */
    #endregion

   
}
