using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TouchRotateSingle : MonoBehaviour
{
    //float timeDelay = 1.5f;
    public bool dragging = false;
    public Vector3 mousePos;
    public static Vector3 eulerRotation;
    public static Vector3 eulerRotation_Forward;

    private Vector3 direction;//Case trong Update lại
                              //public static Vector3 eulerRotation;

    public void Update() {
        if (Form_Gameplay.ins.minigameManager == null || !Form_Gameplay.ins.minigameManager.isGameplay_Start) return;
        /*if (timeDelay > 0) {
            timeDelay -= Time.deltaTime;
            return;
        }*/
        if (Input.GetMouseButtonDown(0) ) {
            dragging = true;
            mousePos = Input.mousePosition;
            //if (!Form_Gameplay.ins.minigameManager.isGameplay_Start)   Form_Gameplay.ins.minigameManager.StartGame();
        }
        if(Input.GetMouseButton(0)) {
            if(dragging) {
                direction = Input.mousePosition - mousePos;
                eulerRotation = new Vector3(direction.x, 0, direction.y);
                eulerRotation_Forward = new Vector3(direction.x, 0, Mathf.Abs( direction.y));
                //if (!Form_Gameplay.ins.minigameManager.isGameplay_Start)    Form_Gameplay.ins.minigameManager.StartGame();
                //Debug.LogError("??? Drag" + eulerRotation);
            } else {//Nếu chằng mau ko bắt đc ButtonDown
               
                    dragging = true;
                    mousePos = Input.mousePosition;
                
                //Debug.LogError("??? Down2" + center);
            }
        }
        if(Input.GetMouseButtonUp(0)) {
            dragging = false;
            //Debug.LogError("??? Up");
        }
    }
}
