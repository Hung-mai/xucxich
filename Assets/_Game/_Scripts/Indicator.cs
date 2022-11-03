using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Indicator : MonoBehaviour
{
    public LocationIndicator locationIndicator;
    public Character data;
    public Image[] imageRenders;

    //public Text txt_Name;
    //public Text txt_Kill;
    public Image iconPlayer;
    public Image img_BGScore;
    //public Image img_Rank;
    public GameObject obj_Crown1;
    public GameObject obj_Crown2;
    public TextMeshProUGUI txt_Score;
    public TextMeshProUGUI txt_NamePlayer;
    //public TweenAlpha twn_Alpha;
    //public Text txt_HP;
    //public Image img_HPCore;

    private void Start()
    {
        //transform.position = new Vector3(0,0,8000);

    }
    public void setUpIndicator(Character Char)
    {
        this.data = Char;
        Char.indicator = this;
        locationIndicator.targetTransform = Char.tran_IndicatorTarget;
        iconPlayer.gameObject.SetActive(Char.idChar == 0);
        txt_NamePlayer.gameObject.SetActive(Char.idChar != 0);
        if (!locationIndicator.enabled) locationIndicator.enabled = true;
        //ChangeColor
        foreach (var item in imageRenders) {
            item.color = GameManager.ins.listColorsChar [Char.modelChar.idColor];
        }
        txt_NamePlayer.color = GameManager.ins.listColorsChar [Char.modelChar.idColor];
        txt_NamePlayer.text = "" + data.username;
        /*
        img_Rank.sprite = GameManager.instance.sprite_Ranks[Char.rank];
        //Hiển thị đúng vị trí icon Rank
        Timer.Schedule(this, 0.05f, () => {
            img_Rank.gameObject.SetActive(true);
            img_Rank.rectTransform.localPosition = new Vector3(-txt_NamePlayer.rectTransform.sizeDelta.x / 2 - 10f, img_Rank.rectTransform.localPosition.y, img_Rank.rectTransform.localPosition.z);
        });*/
        //UpdateTop1();
        gameObject.SetActive(true);
    }

    public void OnDeath()
    {
        //twn_Alpha.PlayZToA();
        //txt_NamePlayer.gameObject.SetActive(false);
        //img_Rank.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnReborn() {
        //twn_Alpha.PlayAToZ();
        //txt_NamePlayer.gameObject.SetActive(data.idChar != 0);
        setUpIndicator(data);
        Timer.Schedule(this, 0.1f, () => {
            gameObject.SetActive(true);
        });
    }
    // Use this for initialization
    public void UpdateTop1(bool isTop1 = false)
    {
        obj_Crown1.SetActive(isTop1);
        obj_Crown2.SetActive(isTop1);
        //txt_Score.text = "" + (int)data.score;
        //img_BGScore.rectTransform.sizeDelta = new Vector2(340 + 40* (data.score / 1000 >= 1 ? 6 : (data.score / 100 >= 1 ? 3 : (data.score / 10 >= 1 ? 1 : 0))), img_BGScore.rectTransform.sizeDelta.y);
        //txt_HP.text = this.data.Hp + "/" + this.data.HpMax;
        //img_HPCore.fillAmount = this.data.Hp / (float)this.data.HpMax;
        //txt_NamePlayer.text = "" + data.namePlayer;
        //  Debug.Log("Changed Text " + onScreenText.text);
    }
}
