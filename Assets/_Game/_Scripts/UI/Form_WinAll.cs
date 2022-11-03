using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Form_WinAll : FormBase
{
    public static Form_WinAll ins;
    public int gold = 500;
    private int currentGold;
    public bool isOpenedSkin = false;
    public ModelChar modelChar;
    public TextMeshProUGUI text_BtnLoseIt;
    public GameObject obj_BtnLoseIt;
    public TextMeshProUGUI text_BtnClaimX3;
    public GameObject obj_PopupOfferX3;
    public GameObject obj_BtnHome;

    public GameObject confetiFx;
    public ParticleSystem[] fireworks;
    public ParticleSystem effect_ChangeSkin;
    //public ParticleSystem moneyFlows;

    [Header("--------------- Offer --------")]
    public int amountOpenGift = 0;
    public Card_GiftWinAll[] listGift;
    public Image img_BestPrice;
    public int idSkinFull = -1;
    public int[] listIDSkinFull;
    public int idSkinHand = -1;
    public int[] listIDSkinHand;
    public List<int> listIDOfferSkin_Full;
    public List<int> listIDOfferSkin_Hand;
    public Sprite sprite_Money;

    private string nameAniIdleOpenMore = "idle2";
    private string nameAniOpen = "open";

    public override void Show()
    {
        base.Show();
        SceneManager.ins.BlockInput(0.5f);
        text_BtnClaimX3.text = "" + gold *3;
        SceneManager.ins.obj_CupTxt.text = DataManager.ins.gameSave.amountWinAll.ToString();
        currentGold = DataManager.ins.gameSave.gold;
        modelChar.WearSkinByGameSave();
        modelChar.animator.SetTrigger(Enums.ins.dic_AniParams [AniParam.T_VictoryCup]);
        obj_BtnLoseIt.SetActive(false);
        obj_BtnHome.SetActive(false);
        Timer.Schedule(this, 0.3f, () => {
            MaxManager.ins.ShowInterstitial("Home_EndGame", () => {
                //moneyFlows.gameObject.SetActive(true);
                confetiFx.SetActive(true);
                //Tìm xem còn SkinFull để Offer ko?
                listIDOfferSkin_Full = new List<int>();
                for (int i = 0; i < listIDSkinFull.Length; i++) {
                    if (listIDSkinFull [i] >= 0 && listIDSkinFull [i] < DataManager.ins.gameSave.listSkins_Full.Length) {
                        if (!DataManager.ins.gameSave.listSkins_Full [listIDSkinFull [i]].isBuy && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Full [listIDSkinFull [i]].HideOnIOS)) {
                            listIDOfferSkin_Full.Add(listIDSkinFull [i]);
                        }
                    }
                }
                //Nếu các SkinFull cố định đã Offer hết thì Offer những cái chưa mua còn lại
                if (listIDOfferSkin_Full.Count < 1) {
                    for (int i = 0; i < DataManager.ins.gameSave.listSkins_Full.Length; i++) {
                        if (!DataManager.ins.gameSave.listSkins_Full [i].isBuy  && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Full [i].HideOnIOS)) {
                            listIDOfferSkin_Full.Add(i);
                        }
                    }
                }
                idSkinFull = listIDOfferSkin_Full [Random.Range(0, listIDOfferSkin_Full.Count)];

                //Tìm xem còn SkinHand để Offer ko?
                listIDOfferSkin_Hand = new List<int>();
                for (int i = 0; i < listIDSkinHand.Length; i++) {
                    if (listIDSkinHand [i] >= 0 && listIDSkinHand [i] < DataManager.ins.gameSave.listSkins_Hand.Length) {
                        if (!DataManager.ins.gameSave.listSkins_Hand [listIDSkinHand [i]].isBuy  && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Hand [listIDSkinHand [i]].HideOnIOS)) {
                            listIDOfferSkin_Hand.Add(listIDSkinHand [i]);
                        }
                    }
                }
                //Nếu các SkinHand cố định đã Offer hết thì Offer những cái chưa mua còn lại
                if (listIDOfferSkin_Hand.Count < 1) {
                    for (int i = 0; i < DataManager.ins.gameSave.listSkins_Hand.Length; i++) {
                        if (!DataManager.ins.gameSave.listSkins_Hand [i].isBuy && (!GameManager.ins.isIOS || !GameManager.ins.arrayDataSkin_Hand [i].HideOnIOS)) {
                            listIDOfferSkin_Hand.Add(i);
                        }
                    }
                }
                idSkinHand = listIDOfferSkin_Hand [Random.Range(0, listIDOfferSkin_Hand.Count)];

                if (idSkinFull >= 0) {
                    img_BestPrice.sprite = GameManager.ins.arrayDataSkin_Full [idSkinFull].icon;
                } else if (idSkinHand >= 0) {
                    img_BestPrice.sprite = GameManager.ins.arrayDataSkin_Hand [idSkinHand].icon;
                } else {
                    img_BestPrice.sprite = sprite_Money;
                }

                /*Timer.Schedule(this, 0.5f, () => {
                    //if (!moneyFlows.isPlaying) moneyFlows.Play();
                    foreach (ParticleSystem p in fireworks) {
                        p.Play();
                    }
                });*/
                FirebaseManager.ins.ads_reward_offer("OpenMoreGift_WinAll", "");
            });
        });
    }

    #region button
    public void Btn_OpenGift(int idGift) {
        SoundManager.ins.sound_Click.PlaySound();
        if (amountOpenGift == 0) {
            amountOpenGift++;
            bool isRandomToSkinHand = idSkinHand >= 0 && DataManager.ins.gameSave.amountWinAll > 1 && Random.Range(0, 10) > 6;
            if (isRandomToSkinHand) {
                DataManager.ins.gameSave.listSkins_Hand[idSkinHand].isBuy = true;
                DataManager.ins.gameSave.listSkins_Hand[idSkinHand].isUnlock = true;
                DataManager.ins.gameSave.listSkins_Hand [idSkinHand].play1Game_Unlock1Time = false;
                if (DataManager.ins.gameSave.idSkin_Full >= 0) {
                    DataManager.ins.gameSave.idSkin_Full = -1;
                    DataManager.ins.gameSave.idSkin_Body = 0;
                }
                DataManager.ins.gameSave.idSkin_Hand = idSkinHand;
                listGift [idGift].obj_Open.SetActive(true);
                listGift[idGift].obj_Close.SetActive(false);
                listGift[idGift].img_IconSkinHand.sprite = GameManager.ins.arrayDataSkin_Hand[idSkinHand].icon;
                listGift[idGift].img_IconSkinHand.gameObject.SetActive(true);
                for (int i = 0; i < listGift.Length; i++) {
                    listGift[i].obj_BtnOpen.SetActive(false);
                    listGift [i].obj_BtnOpenMore.SetActive(true);
                    listGift [i].aniGift.AnimationName = nameAniIdleOpenMore;
                }
                listGift [idGift].aniGift.loop = false;
                listGift [idGift].aniGift.AnimationName = nameAniOpen;
                isOpenedSkin = true;
                Timer.Schedule(this, 2f, () => {
                    modelChar.WearSkinByGameSave();
                    effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                    effect_ChangeSkin.Play();
                    Timer.Schedule(this, 1.5f, () => {
                        obj_BtnLoseIt.SetActive(true);
                        obj_BtnHome.SetActive(true);
                    });
                });
            } else {
                listGift[idGift].obj_Open.SetActive(true);
                listGift[idGift].obj_Close.SetActive(false);
                listGift[idGift].txt_Money.text = "" + gold;
                listGift[idGift].txt_Money.gameObject.SetActive(true);
                for (int i = 0; i < listGift.Length; i++) {
                    listGift [i].obj_BtnOpen.SetActive(false);
                    listGift [i].obj_BtnOpenMore.SetActive(true);
                    listGift [i].aniGift.AnimationName = nameAniIdleOpenMore;
                }
                listGift [idGift].aniGift.loop = false;
                listGift [idGift].aniGift.AnimationName = nameAniOpen;
                SceneManager.ins.BlockInput(true);
                Timer.Schedule(this, 2f, () => {
                    SceneManager.ins.effectGem.ShowEffect(gold, Camera.main.WorldToScreenPoint(listGift [idGift].txt_Money.transform.position), SceneManager.ins.iconGold.transform,
                    () => {
                        SceneManager.ins.BlockInput(false);
                        DataManager.ins.ChangeGold(gold, "OpenGift_WinAll", true, false);
                        SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                        });
                        Timer.Schedule(this, 1.5f, () => {
                            obj_BtnLoseIt.SetActive(true);
                            obj_BtnHome.SetActive(true);
                        });
                    });
                });
            }
        } else if (amountOpenGift == 1) {
            MaxManager.ins.ShowRewardedAd("Open_WinAll", "WinAll_Gift_" + (amountOpenGift + 1), () => {
                amountOpenGift++;
                listGift[idGift].obj_Open.SetActive(true);
                listGift[idGift].obj_Close.SetActive(false);
                listGift [idGift].aniGift.loop = false;
                listGift [idGift].aniGift.AnimationName = nameAniOpen;
                if (idSkinHand >= 0 && DataManager.ins.gameSave.listSkins_Hand [idSkinHand].isBuy == false) {
                    listGift[idGift].img_IconSkinHand.sprite = GameManager.ins.arrayDataSkin_Hand[idSkinHand].icon;
                    listGift[idGift].img_IconSkinHand.gameObject.SetActive(true);
                    DataManager.ins.gameSave.listSkins_Hand[idSkinHand].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Hand[idSkinHand].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Hand [idSkinHand].play1Game_Unlock1Time = false;
                    if (DataManager.ins.gameSave.idSkin_Full >= 0) {
                        DataManager.ins.gameSave.idSkin_Full = -1;
                        DataManager.ins.gameSave.idSkin_Body = 0;
                    }
                    DataManager.ins.gameSave.idSkin_Hand = idSkinHand;
                    isOpenedSkin = true;
                    Timer.Schedule(this, 2f, () => {
                        modelChar.WearSkinByGameSave();
                        effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                        effect_ChangeSkin.Play();
                    });
                } else {
                    listGift[idGift].txt_Money.text = "" + gold;
                    listGift[idGift].txt_Money.gameObject.SetActive(true);
                    DataManager.ins.ChangeGold(gold, "OpenGift_WinAll", true, false);
                    SceneManager.ins.BlockInput(true);
                    Timer.Schedule(this, 2f, () => {
                        SceneManager.ins.effectGem.ShowEffect(gold, Camera.main.WorldToScreenPoint(listGift [idGift].txt_Money.transform.position), SceneManager.ins.iconGold.transform,
                        () => {
                            SceneManager.ins.BlockInput(false);
                            DataManager.ins.ChangeGold(gold, "OpenGift_WinAll", true, false);
                            SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                            });
                        });
                    });
                }
            });
        } else {
            MaxManager.ins.ShowRewardedAd("Open_WinAll", "WinAll_Gift_" + (amountOpenGift + 1), () => {
                amountOpenGift++;
                if (amountOpenGift >= 2) text_BtnLoseIt.text = "Continue";
                listGift[idGift].obj_Open.SetActive(true);
                listGift[idGift].obj_Close.SetActive(false);
                listGift [idGift].aniGift.loop = false;
                listGift [idGift].aniGift.AnimationName = nameAniOpen;
                if (idSkinFull >= 0) {
                    listGift[idGift].img_IconSkinFull.sprite = GameManager.ins.arrayDataSkin_Full[idSkinFull].icon;
                    listGift[idGift].img_IconSkinFull.gameObject.SetActive(true);
                    DataManager.ins.gameSave.listSkins_Full[idSkinFull].isBuy = true;
                    DataManager.ins.gameSave.listSkins_Full[idSkinFull].isUnlock = true;
                    DataManager.ins.gameSave.listSkins_Full [idSkinFull].play1Game_Unlock1Time = false;
                    DataManager.ins.gameSave.idSkin_Full= idSkinFull;
                    isOpenedSkin = true;
                    Timer.Schedule(this, 2f, () => {
                        modelChar.WearSkinByGameSave();
                        effect_ChangeSkin.startColor = GameManager.ins.listColorsChar [modelChar.idColor];
                        effect_ChangeSkin.Play();
                    });
                } else {
                    listGift[idGift].txt_Money.text = "" + gold;
                    listGift[idGift].txt_Money.gameObject.SetActive(true);
                    SceneManager.ins.BlockInput(true);
                    Timer.Schedule(this, 2f, () => {
                        SceneManager.ins.effectGem.ShowEffect(gold, Camera.main.WorldToScreenPoint(listGift [idGift].txt_Money.transform.position), SceneManager.ins.iconGold.transform,
                       () => {
                           SceneManager.ins.BlockInput(false);
                           DataManager.ins.ChangeGold(gold, "OpenGift_WinAll", true, false);
                           SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                           });
                       });
                    });
                }
            });
        }
    }

    public void Btn_ExtraMoney() {
        MaxManager.ins.ShowRewardedAd("ExtraMoney_WinAll", "ExtraMoney_WinAll", () => {
            SceneManager.ins.BlockInput(true);
            SceneManager.ins.effectGem.ShowEffect(gold * 3, Camera.main.WorldToScreenPoint(text_BtnClaimX3.transform.position), SceneManager.ins.iconGold.transform,
                () => {
                    DataManager.ins.ChangeGold(gold * 3, "ExtraMoney_WinAll", true, false);
                    SceneManager.ins.OpenTweenGem(SceneManager.ins.txt_UIMoney, currentGold, () => {
                        SceneManager.ins.BlockInput(false);
                        SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
                    });
                });
        });
    }
    public void BtnLoseIt_ExtraMoney() {
        SoundManager.ins.sound_Click.PlaySound();
        MaxManager.ins.ShowInterstitial("Continue_WinAll", () => {
            SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
        }, MaxManager.ins.isInterLastSuccess);
    }

    public void BtnLoseIt()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (amountOpenGift <= 1) {
            if (isOpenedSkin) {
                MaxManager.ins.ShowInterstitial("Continue_WinAll", () => {
                    SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
                }, MaxManager.ins.isInterLastSuccess);
            }else{
                modelChar.gameObject.SetActive(false);
                obj_PopupOfferX3.SetActive(true);
            }
        } else {
            SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
        }
    }

    public void Btn_home()
    {
        SoundManager.ins.sound_Click.PlaySound();
        if (amountOpenGift <= 1) {
            MaxManager.ins.ShowInterstitial("Home_WinAll", () => {
                SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
            }, MaxManager.ins.isInterLastSuccess);
        } else {
            SceneManager.ins.ChangeForm(FormUI.Form_Home.ToString(), 0);
        }
    }
    #endregion
}
