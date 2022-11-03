using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationIndicator : MonoBehaviour
{

    public Indicator indicator;
    public Transform targetTransform;
    public float rateResolution;//Tỷ lệ độ phân giải
    public float standardHeight;//Độ cao chuẩn tính theo tỷ lệ 16/9
    public float highBanner;//Độ cao Banner tính theo độ phân giải và tỷ lệ màn hình
    /* 3:4 640x853 -> 70
     * 9:16 480x853 -> 70
     * 9:21 480x1120 -> 70
     * 3:4 1920x2560 -> 207
     * 9:16 1440x2560 -> 207
     * 9:21 1440x3360 -> 207
     */


    public Transform UiRotational;
    public Transform UiStational;

    public Transform objRotational;

    private Vector3 pos, pos_Display;

    public bool isOnScreen = false;
    float lastUpdateTime;

    void Start()
    {
        if (targetTransform == null) return;
        //Resize 16/9 -> 21 /9
        if ((float)Screen.height / Screen.width > 16.5f / 9) {
            standardHeight = Screen.width * 16f / 9;
        } else {//Resize 3/4 -> 16 /9
            standardHeight = Screen.height;
        }
        highBanner = standardHeight / 2560 * 207;
        rateResolution = Screen.height / 960f;
        UiRotational.localScale = rateResolution* Vector3.one;
        UiStational.localScale = rateResolution * Vector3.one;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetTransform == null) return;
        //isOnScreen = OnScreen(WorldToScreen(targetTransform));
        //Quy đổi vị trí 3D thành vị trí trên Camera
        if(targetTransform == null) {
            Debug.Log("Attached to " + gameObject.name);
        }
        pos = Camera.main.WorldToScreenPoint(targetTransform.position);
        pos_Display = indicator.data.idChar == 0 ? new Vector3(pos.x, pos.y + rateResolution * (30 + GameManager.ins.minigameManager.listChars[0].level), 0) : new Vector3(pos.x, pos.y + rateResolution*(30 + indicator.data.level - GameManager.ins.minigameManager.listChars [0].level), 0);
        //Có nằm trong Sceen ko
        isOnScreen = (pos_Display.x < (Screen.width) && pos_Display.x > 0 && pos_Display.y < (Screen.height) && pos_Display.y > (!MaxManager.ins.isBannerShowing|| DataManager.ins.gameSave.isNoAds ? 0 : highBanner));
        UiStational.gameObject.SetActive(true);
        if (isOnScreen)//Khi Char đang hiển thị trên màn hình
        {
            UiRotational.gameObject.SetActive(false);
            UiStational.gameObject.SetActive(true);
            objRotational.localEulerAngles = new Vector3(0, 0, 180);//Mũi tên chỉ thẳng xuống đầu Char
            UiStational.localPosition = pos_Display;//Mũi tên chỉ thẳng xuống đầu Char
            indicator.UpdateTop1(indicator.data.top == 0);
        }
        else
        {
            UiRotational.gameObject.SetActive(true);
            UiStational.gameObject.SetActive(false);
            UpdateOffScreen(pos_Display, Camera.main.transform.InverseTransformPoint(targetTransform.position).z >= 0);
            indicator.UpdateTop1(indicator.data.top == 0);
        }

    }
    private void UpdateOffScreen(Vector3 targetPosOnScreen, bool isFontCamera = true)
    {

        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;


        Vector3 newIndicatorPos = targetPosOnScreen - screenCenter;
        //Nếu Enemy ở đằng sau camera thì đảo ngược lại vị trí
        if(!isFontCamera)
            newIndicatorPos = new Vector3(-newIndicatorPos.x, -newIndicatorPos.y, newIndicatorPos.z);


        float angle = Mathf.Atan2(newIndicatorPos.y, newIndicatorPos.x);
        angle -= 90 * Mathf.Deg2Rad;

        //  y = mx + b (intercept forumla)
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        float m = cos / sin;//Tỷ lệ y/x


        Vector3 screenBounds = new Vector3(screenCenter.x - rateResolution*32, screenCenter.y - rateResolution*32);


        if (cos > 0)
            newIndicatorPos = new Vector2(-screenBounds.y / m, screenBounds.y);
        else
            newIndicatorPos = new Vector2(screenBounds.y / m, -screenBounds.y+ (!MaxManager.ins.isBannerShowing || DataManager.ins.gameSave.isNoAds ? 0 : highBanner));


        if (newIndicatorPos.x > screenBounds.x)
            newIndicatorPos = new Vector2(screenBounds.x, -screenBounds.x * m);
        else if (newIndicatorPos.x < -screenBounds.x)
            newIndicatorPos = new Vector2(-screenBounds.x, screenBounds.x * m);


        newIndicatorPos += screenCenter;
        UiStational.localPosition = newIndicatorPos;
        UiRotational.localPosition = newIndicatorPos;
        objRotational.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

    }

}
