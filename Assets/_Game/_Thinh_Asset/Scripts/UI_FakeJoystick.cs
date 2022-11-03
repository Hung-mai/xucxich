using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_FakeJoystick : MonoBehaviour
{
    public Image BGparent;
    public Image JoystickMain;

    private float threshHold = 30;
    private float spdMoveJoyStick = 10;
    private Vector2 lastMousePos;

    public Form_Gameplay formGameplay;
    public GameObject joystick;

    /*
     * logic : người chơi kéo tay về đâu thì joystick di chuyển theo đó
     * nếu main cách BGparent quá 1 khoảng = thresh hold thì BGparent sẽ di chuyển theo để vừa với thresh hold đó
     */
    private void Awake()
    {
        switch (formGameplay.minigameCur)
        {
            case Minigame.WackyRun:
            case Minigame.HitAndRun:
                joystick.gameObject.SetActive(false);
                break;
        }
    }
    
    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            
            BGparent.rectTransform.anchoredPosition = JoystickMain.rectTransform.anchoredPosition;
            JoystickMain.rectTransform.anchoredPosition = Vector2.Lerp(JoystickMain.rectTransform.anchoredPosition, ScreenToRectPos(Input.mousePosition, GetComponent<RectTransform>()), Time.deltaTime * spdMoveJoyStick);
            
        }

        if (Input.GetMouseButton(0))
        {
            JoystickMain.rectTransform.anchoredPosition = Vector2.Lerp(JoystickMain.rectTransform.anchoredPosition, ScreenToRectPos(Input.mousePosition, GetComponent<RectTransform>()), Time.deltaTime * spdMoveJoyStick);
        }

        if (Input.GetMouseButtonUp(0))
        {
            //BGparent.gameObject.SetActive(false);
        }

    }
    public Vector2 ScreenToRectPos(Vector2 screen_pos, RectTransform rectTransform)
    {
        if (SceneManager.ins.formCanvas.renderMode != RenderMode.ScreenSpaceOverlay && SceneManager.ins.formCanvas.worldCamera != null)
        {
            //Canvas is in Camera mode
            Vector2 anchorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screen_pos, SceneManager.ins.formCanvas.worldCamera, out anchorPos);
            return anchorPos;
        }
        else
        {
            //Canvas is in Overlay mode
            Vector2 anchorPos = screen_pos - new Vector2(rectTransform.position.x, rectTransform.position.y);
            anchorPos = new Vector2(anchorPos.x / rectTransform.lossyScale.x, anchorPos.y / rectTransform.lossyScale.y);
            return anchorPos;
        }
    }
    
    
    
}
