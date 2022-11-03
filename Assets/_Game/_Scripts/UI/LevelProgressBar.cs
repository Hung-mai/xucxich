using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    public bool isLoaded = false;
    public LevelProgress_Node[] nodes;
    public Color colorLine;

    //public RectTransform BGRectTransform;
    private float widthPerNode = 115.2f;

    private void Awake()
    {
        //init node theo so luong trong Constant.AMOUNT_MINIGAMES_PER_CYCLE
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].gameObject.SetActive(false);
        }
        int amountNode = (int)Mathf.Clamp(PluginManager.ins.NodeStart_WinAll + DataManager.ins.gameSave.amountWinAll * PluginManager.ins.NodeIncease_WinAll, 0, 8);
        for (int i = 0; i < amountNode; i++)
        {
            nodes[i].gameObject.SetActive(true); nodes[i].SetTextLevel(i + 1);
            if (i == amountNode - 1) nodes[i].ToogleGift(true);
            else nodes[i].ToogleGift(false);
        }
        //set width BG 
        //BGRectTransform.sizeDelta =  new Vector2(widthPerNode * Constants.AMOUNT_MINIGAMES_PER_CYCLE,BGRectTransform.sizeDelta.y);
    }
    public void RefreshLevelProgressBar(int stateWin = -1)
    {
        if (isLoaded) return;
        isLoaded = true;
        int level = DataManager.ins.gameSave.level;
        for (int i = 0; i <= level; i++)
        {

            if (i == level)
            {
                //arrows[i].SetActive(true);
                //if (stateWin == 1) nodes[i].sprite = i_level_finish;
                //if (stateWin == 0) nodes[i].sprite = i_level_Lose;
                if (stateWin == 1) nodes[i].ActiveNodeDone();
                if (stateWin == 0) nodes[i].ActiveNodeFail();
                if (stateWin == -1) nodes[i].ActiveNodeBank();
            }
            else
            {
                //arrows[i].SetActive(false);
                //nodes[i].sprite = i_level_finish;
                nodes[i].NodePass();
            }

        }
    }
}
