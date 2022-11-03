using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {
    public int amountRelease = 0;
    int maxRelease = 3;

    public GameObject obj_All;
    public CanvasGroup canvasGroup;

    private void Start() {
        if (DataManager.ins.gameSave.levelEnded >= PluginManager.ins.NodeStart_WinAll + DataManager.ins.gameSave.amountWinAll*PluginManager.ins.NodeIncease_WinAll) {
            maxRelease = 1;
        } else{
            maxRelease = 3;
        }
    }

    private void Update() {
        if (!Form_Gameplay.ins.minigameManager.isGameplay_Start) return;
        canvasGroup.alpha = SceneManager.ins.IsOpenPopUp()  || Form_Gameplay.ins.minigameManager.listChars[0].isFinish ? 0 : 1;
        if (amountRelease < maxRelease) {
            if (Input.GetMouseButtonDown(0)) {
                amountRelease++;
                obj_All.SetActive(false);
            }
            if (Input.GetMouseButton(0)) {
                obj_All.SetActive(false);
            }
            if (Input.GetMouseButtonUp(0)) {
                obj_All.SetActive(true);
            }
        }
        if (amountRelease >= maxRelease) {
            gameObject.SetActive(false);
            return;
        }
    }
}
