using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelProgress_Node : MonoBehaviour
{
    public GameObject arrowUp;
    public Image line;
    public Image node;
    public GameObject gift;

    public TextMeshProUGUI txtLevel;

    public Color colorBlank;
    public Color colorDone;
    public Color colorFail;

    public void ActiveNodeBank() {
        arrowUp.SetActive(true);
        line.color = colorDone;
        node.color = colorBlank;
    }

    public void ActiveNodeDone()
    {
        arrowUp.SetActive(true);
        line.color = colorDone;
        node.color = colorDone;
    }
    public void ActiveNodeFail()
    {
        arrowUp.SetActive(true);
        line.color = colorDone;
        node.color = colorFail;
    }
    public void NodePass()
    {
        arrowUp.SetActive(false);
        line.color = colorDone;
        node.color = colorDone;
    }
    public void ToogleGift(bool isGift)
    {
        txtLevel.gameObject.SetActive(!isGift);
        node.gameObject.SetActive(!isGift);
        gift.SetActive(isGift);
    }
    public void SetTextLevel(int i)
    {
        txtLevel.text = i.ToString();
    }
}
