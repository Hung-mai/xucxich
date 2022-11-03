using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : Singleton<Enums>
{
    public Dictionary<AniParam, int> dic_AniParams = new Dictionary<AniParam, int>();//List chứa id của các Animation

    public override void Awake()
    {
        base.Awake();
        //Lấy ra list chứa id của các Animation GamePlay
        dic_AniParams.Add(AniParam.None, Animator.StringToHash(AniParam.None.ToString()));
        dic_AniParams.Add(AniParam.B_Damaged, Animator.StringToHash(AniParam.B_Damaged.ToString()));
        dic_AniParams.Add(AniParam.B_Drown, Animator.StringToHash(AniParam.B_Drown.ToString()));
        dic_AniParams.Add(AniParam.B_Lose, Animator.StringToHash(AniParam.B_Lose.ToString()));
        dic_AniParams.Add(AniParam.B_Win, Animator.StringToHash(AniParam.B_Win.ToString()));
        dic_AniParams.Add(AniParam.F_RunForm, Animator.StringToHash(AniParam.F_RunForm.ToString()));
        dic_AniParams.Add(AniParam.F_Speed, Animator.StringToHash(AniParam.F_Speed.ToString()));
        dic_AniParams.Add(AniParam.T_Attack, Animator.StringToHash(AniParam.T_Attack.ToString()));
        dic_AniParams.Add(AniParam.T_BackHit, Animator.StringToHash(AniParam.T_BackHit.ToString()));
        dic_AniParams.Add(AniParam.T_FrontHit, Animator.StringToHash(AniParam.T_FrontHit.ToString()));
        //Lấy ra list chứa id của các Animation Home
        dic_AniParams.Add(AniParam.B_StyleDance, Animator.StringToHash(AniParam.B_StyleDance.ToString()));
        dic_AniParams.Add(AniParam.T_Dances, Animator.StringToHash(AniParam.T_Dances.ToString()));
        dic_AniParams.Add(AniParam.T_IdleHome, Animator.StringToHash(AniParam.T_IdleHome.ToString()));
        dic_AniParams.Add(AniParam.T_IdleScratchHead, Animator.StringToHash(AniParam.T_IdleScratchHead.ToString()));
        dic_AniParams.Add(AniParam.T_IdleWaveHand, Animator.StringToHash(AniParam.T_IdleWaveHand.ToString()));
        dic_AniParams.Add(AniParam.T_VictoryCup, Animator.StringToHash(AniParam.T_VictoryCup.ToString()));
    }
}

public enum FormUI {//Ko nên xóa mà nên thêm 
    None,
    Form_Loading,
    Form_Home,
    Form_GamePlay,
    Form_EndGame,
}

public enum PopupUI {//Ko nên xóa mà nên thêm 
    None,
    Popup_Rate,
    Popup_EndGame,
    Popup_Revive,
    Popup_Settings,
    Popup_Skin,
    Popup_Offer,
    Popup_SelectMiniGame,
    Popup_OutFit,
    Popup_Earning,
    Popup_OfferFreeSkin,
    Popup_NoAds,
}

public enum AniParam{
    None,
    B_Damaged,
    B_Drown,
    B_Lose,
    B_Win,
    F_RunForm,
    F_Speed,
    T_Attack,
    T_BackHit,
    T_FrontHit,

    B_StyleDance,
    T_Dances,
    T_IdleHome,
    T_IdleScratchHead,
    T_IdleWaveHand,
    T_VictoryCup
}

public enum Char_State {
    None,
    Attack,
    Damaged,
    Death,
    Idle,
    Ragdoll,
    Run,
    Win,
    Lose,
}
public enum Tag {
    None,
    KillZone,
    Character,
    Player,
}

public enum ColliderKillChar {//Ko nên xóa mà nên thêm 
    None,
    HorizontalSlice,
    VerticalSlice,
    Smash,
    Water,
    Bomb,
}

public enum Minigame {//Ko đc xóa mà chỉ đc thêm 
    None,
    SausageWarIO,
    WackyRun,
    SnakeBlock,
    PlatformPush,
    HitAndRun,
    SidestepSlope,
    SkewerScurry,
    GhostChaser,
    OnTheCuttingBoard,
    Slither
}

public enum TypeValue {
    None,
    Bool,
    Integer,
    Long,
    Float,
    Double,
    String,
}
public enum SkewerScurry_TrapPos
{
    Top,
    Down,
    Left,
    Right,
}

public enum CharSkin_Skeleton {
    None,
    Head,
    R_Hand,
    L_Hand,
    L_Foot,
    R_Foot,
    Spine,
    Spine2,
    L_Shoulder,
    L_Arm,
    L_ForeArm,
}

public enum CharSkin_Type {
    None,
    Hat,
    Body,
    Hand,
    Face,
    Set
}
public enum BossState
{
    Idle,
    LeftHandUp,
    LeftHit,
    RightHandUp,
    RightHit,
}

