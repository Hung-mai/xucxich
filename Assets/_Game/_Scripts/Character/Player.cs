using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Player : Character {
    [Header("--------------- Player ---------------")]
    public bool isMouseDown = false;
    public float offsetFootToBody;
    public float radiusSphereCheck;
    public Camera cameraPlayer;
    public override void Awake() {
        base.Awake();
        player = this;
    }

    public override void Main() {
        if (!gameObject.activeSelf || !Form_Gameplay.ins.minigameManager.isGameplay_Start) {
            return; 
        }
        if (Form_Gameplay.ins.minigameManager.minigame == Minigame.HitAndRun) {
            if (state_New == Char_State.Win || state_New == Char_State.Lose) {
            } else if (isAlive) {
                if (Input.GetMouseButton(0)) {
                    if(!isMouseDown) state_New = Char_State.Attack;
                    isMouseDown = true;
                } else {
                    isMouseDown = false;
                    state_New = Char_State.Idle;
                }
            } else {
                if (state_Apply != Char_State.Death) {
                    state_New = Char_State.Death;
                } else {
                    return;
                }
            }
        } else {
            if (state_New == Char_State.Win || state_New == Char_State.Lose) {
            } else if (isAlive) {
                if(state_New == Char_State.Damaged) {

                } else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.Slither ||( Input.GetMouseButton(0) && (state_Apply == Char_State.Idle || state_Apply == Char_State.Run))) {
                    state_New = Char_State.Run;
                } else {
                    if (state_Apply == Char_State.Run) {
                        state_New = Char_State.Attack;
                    }
                }
            } else {
                if (state_Apply != Char_State.Death) {
                    state_New = Char_State.Death;
                } else {
                    return;
                }
            }
        }
        ReloadAnimation();
    }

    public override void ReloadAnimation() {
        if (state_New == Char_State.Run) {
            if (GameManager.ins.minigameManager.minigame == Minigame.SidestepSlope) {
                if (TouchRotateSingle.eulerRotation != Vector3.zero) {
                    tran_Rotate.localRotation = Quaternion.LookRotation(TouchRotateSingle.eulerRotation_Forward);
                }
                transform.Translate(tran_Rotate.TransformDirection(Vector3.forward) * Time.fixedDeltaTime* speed * (isBooster ? 1.5f : 1));
            } else if (GameManager.ins.minigameManager.minigame != Minigame.HitAndRun) {
                if (GameManager.ins.minigameManager.minigame == Minigame.WackyRun) {

                } else if (TouchRotateSingle.eulerRotation != Vector3.zero) {
                    tran_Rotate.localRotation = Quaternion.LookRotation(TouchRotateSingle.eulerRotation);
                }
                float velocityY = GameManager.ins.minigameManager.minigame == Minigame.SnakeBlock ? -0.5f : 0;
                transform.Translate(tran_Rotate.TransformDirection(new Vector3(0, velocityY, 1)) * Time.fixedDeltaTime* speed* (isBooster ? 1.5f : 1));
            }
        }

        if (state_New == Char_State.Damaged && isVelocityDamaged)
        {
            //fixed update
            Vector3 direction = transform.position - charAttacker.transform.position;
            myRigidbody.velocity = direction.normalized * speedBack;
        }

        if (state_Apply != state_New) {
            state_Apply = state_New;
            switch (state_Apply) {
                case Char_State.Attack:
                    modelChar.animator.SetTrigger(Enums.ins.dic_AniParams [AniParam.T_Attack]);
                    break;
                case Char_State.Damaged:
                    isDamaged = true;
                    isVelocityDamaged = true;
                    modelChar.idBrown = 2;
                    modelChar.idEye = 1;
                    modelChar.idLid = -1;
                    modelChar.idMounth = 1;
                    if (DataManager.ins.gameSave.isVibrate && SceneManager.ins.popup_EndGame == null) MoreMountains.NiceVibrations.MMVibrationManager.Vibrate();
                    Vector3 direction = transform.position - charAttacker.transform.position;
                    //direction.y = 0;
                    float angleDiff = Vector3.Angle(direction, tran_Rotate.TransformDirection(Vector3.forward));
                    Form_Gameplay.ins.minigameManager.Create_EffectDamaged(transform.position);
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams [AniParam.B_Damaged], true);
                    modelChar.animator.SetTrigger(angleDiff < 90 ? Enums.ins.dic_AniParams [AniParam.T_FrontHit] : Enums.ins.dic_AniParams [AniParam.T_BackHit]);
                    Timer.Schedule(this, 0.1f, () => {
                        isVelocityDamaged = false;
                    });
                    Timer.Schedule(this, 0.65f, () => {
                        isDamaged = false;
                        if (state_Apply == Char_State.Damaged) state_New = Char_State.Idle;
                        modelChar.animator.SetBool(Enums.ins.dic_AniParams [AniParam.B_Damaged], false);
                    });
                    break;
                case Char_State.Death:
                    isAnimationDyning = true;
                    indicator.OnDeath();
                    if (DataManager.ins.gameSave.isVibrate && SceneManager.ins.popup_EndGame == null) MoreMountains.NiceVibrations.MMVibrationManager.Vibrate();
                    //myCollider.enabled = false;
                    if (DataManager.ins.gameSave.volumeSound > 0) {
                        //audio_Die.clip = GUIManager.instance.list_audio_Die [Random.Range(0, GUIManager.instance.list_audio_Die.Length)];
                        //audio_Die.Play();
                    }
                    if (PlayerPrefs.GetInt("Vibrate") == 1) Vibration.Vibrate(GameManager.ins.isIOS ? 6 : 25);
                    if (colliderKillChar == ColliderKillChar.Water) {
                        sound_DieWater.PlaySound();
                        obj_Shadow.SetActive(false);
                        modelChar.animator.SetBool(Enums.ins.dic_AniParams [AniParam.B_Drown], true);
                    } else {
                        myCollider.enabled = false;
                        myRigidbody.isKinematic = true;
                        obj_Shadow.SetActive(false);
                        effectBlood.SetActive(true);
                        modelChar.obj_Animation.SetActive(false);
                        modelChar.obj_Death.SetActive(true);
                        modelChar.ReloadModelDeath(colliderKillChar);
                    }
                    Timer.Schedule(this, Form_Gameplay.ins.minigameManager.minigame == Minigame.HitAndRun ? 1.2f : 1.5f, () => {
                        if (SceneManager.ins.popup_EndGame == null) {
                            modelChar.LockModelDeath(colliderKillChar);
                            if (Form_Gameplay.ins.minigameManager.isPlayerReviveFree) {
                                Reborn();
                            } else {
                                SceneManager.ins.ShowPopup_Revive(Reborn);
                            }
                        }
                    });
                    break;
                case Char_State.Idle:
                    modelChar.idBrown = 0;
                    modelChar.idEye = 0;
                    modelChar.idLid = -1;
                    modelChar.idMounth = 7;
                    isVelocityDamaged = false;
                    isDamaged = false;
                    modelChar.animator.SetFloat(Enums.ins.dic_AniParams [AniParam.F_Speed], 0);
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Drown], false);
                    break;
                case Char_State.Ragdoll:
                    break;
                case Char_State.Run:
                    modelChar.animator.SetFloat(Enums.ins.dic_AniParams[AniParam.F_Speed], 1);
                    break;
                case Char_State.Win:
                    modelChar.idBrown = 4;
                    modelChar.idEye = 0;
                    modelChar.idLid = 0;
                    modelChar.idMounth = 2;
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Win], true);
                    break;
                case Char_State.Lose:
                    modelChar.idBrown = 0;
                    modelChar.idEye = 0;
                    modelChar.idLid = 2;
                    modelChar.idMounth = 0;
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams [AniParam.B_Lose], true);
                    break;
                default:
                    Debug.LogError("Lỗi ReloadAnimation() Player:" + state_Apply.ToString());
                    break;
            }
            modelChar.ReloadFace();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * offsetFootToBody ,radiusSphereCheck);
    }
}
