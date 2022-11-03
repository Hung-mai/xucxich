using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class AIBot : Character
{
    [Header("--------------- AIBot ---------------")]
    public int pointIntelligence = 0;//0->100
    public int idBlockTarget = -1;
    public int amountRevive = 0;
    public float timeIdle = 0;
    public float timeRun = 0;
    public bool isState_CanAttack = false;
    public bool isState_WaitingAttack = false;
    public Slither_Food foodTarget;

    #region skewer Scurry AI
    public bool isMove = false;//skewerscurry
    public bool isDanger = true;
    public SkewerScurry_Block blockOn;
    public float timeAttack = 3f;
    private bool isRunSmart = false;
    public float idleTime = 2f;
    private bool isOneTime = false;
    private int rand;
    #endregion

    #region GhostChaserAI
    private GhostChaser_BossManager bossNearest;
    private bool isCheck_GhostChaser = false;
    #endregion

    public Vector3 pos_TargetMove;
    public NavMeshAgent agent;

    [Header("---------sphere check---------")]
    public float offsetFootToBody;
    public float radiusSphereCheck;
    public float force;
    public override void Awake()
    {
        base.Awake();
        AIBot = this;
    }

    public override void Main()
    {
        if (!gameObject.activeSelf || !Form_Gameplay.ins.minigameManager.isGameplay_Start) {
            return;
        }
        if (Form_Gameplay.ins.minigameManager.minigame == Minigame.HitAndRun)
        {
            if (timeIdle > 0) timeIdle -= Time.fixedDeltaTime;
            if (state_New == Char_State.Win || state_New == Char_State.Lose)
            {
            }
            else if (isAlive)
            {
                //Khi mà bóng vừa hiện ra thì IQ càng cao thì đánh càng nhanh
                if (HitAndRun_Manager.ins.hitAndRun_Map.listBoats[idChar].isShowingBalloon)
                {
                    if (!isState_CanAttack)
                    {
                        isState_CanAttack = true;
                        isState_WaitingAttack = true;
                        timeIdle = (100 - pointIntelligence + Random.Range(-10, 10)) / 100f * HitAndRun_Manager.ins.hitAndRun_Map.timeBallStanding;
                    }
                }
                else if (HitAndRun_Manager.ins.hitAndRun_Map.listBoats[idChar].isShowingBomb)
                {
                    if (!isState_CanAttack)
                    {
                        isState_CanAttack = true;
                        if (Random.Range(0, 100) <= (100 - pointIntelligence) * 0.3f)
                        {
                            state_New = Char_State.Attack;
                        }
                    }
                }
                else
                {
                    isState_CanAttack = false;
                }
                if (timeIdle <= 0 && isState_WaitingAttack)
                {
                    state_New = Char_State.Attack;
                    isState_WaitingAttack = false;
                }
            }
            else
            {
                if (state_Apply != Char_State.Death)
                {
                    state_New = Char_State.Death;
                }
            }
        }
        else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.SkewerScurry)
        {
            //if (timeIdle > 0) timeIdle -= Time.fixedDeltaTime;
            //if (timeRun > 0) timeRun -= Time.fixedDeltaTime;
             
            if (timeAttack > 0) timeAttack -= Time.fixedDeltaTime;
            if (state_New == Char_State.Win || state_New == Char_State.Lose)
            {

            }
            else if (isAlive)
            {
                if (state_New == Char_State.Damaged)
                {
                    //Tự động Damaged và trờ về State Idle sau khi Damaged xong (Các Event gắn vào Animmation)
                }
                else if (state_New == Char_State.Attack)
                {
                    //Tự động Attack và trờ về State Idle sau khi Attack xong (Các Event gắn vào Animmation)
                }
                else if (state_New == Char_State.Idle)
                {
                    if (state_Apply == Char_State.Idle)
                    {
                        float minDis = 99999;
                        foreach (RowBlocks row in SkewerScurry_Manager.ins.mapManager.Rows)
                        {
                            foreach (SkewerScurry_Block b in row.Blocks)
                            {
                                if (Vector3.Distance(b.transform.position,transform.position) <= minDis)
                                {
                                    minDis = Vector3.Distance(b.transform.position, transform.position);
                                    blockOn = b;
                                }
                            }
                        }
                        if (SkewerScurry_Manager.ins.mapManager.isActive && blockOn.isDanger)
                        {//trap on
                            int i = Random.Range(0, 100);
                            if (i <= pointIntelligence)
                            {//run  
                                state_New = Char_State.Run; isRunSmart = true;
                            }
                            else
                            {
                                state_New = Char_State.Run; isRunSmart = false;
                            }
                        }
                        else if (SkewerScurry_Manager.ins.mapManager.isActive && !blockOn.isDanger)
                        {//trap off
                            if (!isOneTime)
                            {
                                rand = Random.Range(0, 100);
                                isOneTime = true;
                            }
                            if (rand <= pointIntelligence)
                            {//run  
                                state_New = Char_State.Run; isRunSmart = true;
                                isOneTime = false;
                            }
                            else
                            {
                                if (idleTime > 0) idleTime -= Time.fixedDeltaTime;
                            }
                            if (idleTime <= 0)
                            {
                                isOneTime = false;
                                state_New = Char_State.Run; isRunSmart = true;
                                idleTime = 2f;
                                timeAttack = Random.Range(1f,4f);
                            }
                        }
                        else if (!SkewerScurry_Manager.ins.mapManager.isActive)
                        {
                            state_New = Char_State.Run;
                        }

                    }
                }
                //else if (state_New == Char_State.Run || timeRun <= 0)
                //{
                //    if (state_Apply == Char_State.Run && timeRun <= 0)
                //    {
                //        state_New = Char_State.Attack;
                //    }
                //}
            }
            else
            {
                if (state_New != Char_State.Death)
                {
                    state_New = Char_State.Death;
                }
            }



            #region AI skewer Scurry
            /*
             * 1 con AI thì có độ thông minh
             * độ thông minh sẽ ảnh hưởng ntn tới hành đọng của con AI đó
             * con AI càng thông minh càng né giỏi
             * di chuyển tới 1 vị trí an toàn để ẩn nấp sau những lần trap kích hoạt
             * trap kích hoạt thì đứng yên 
             * trap chưa kích hoạt thì có chạy lung tung cả lên để đẩy nhau vs người chơi khác(tạo hành vi giống người cho AI)
             * những chỉ số ảnh hưởng AI
             * - độ ổn định : intel càng cao càng ổn định , khi trap kích hoạt thì random intel, hên xui cho chạy vào vị trí an toàn hoặc vị trí random trên ma trận
             * +đứng im cho tới khi trap deactive , random liên tục xem có nên chạy tiếp k . intel càng cao => càng đứng vững
             * - độ nhạy bén : intel càng cao càng nhạy, trap kích hoạt thì random độ intel, hên xui cho thời gian chạy trước khi trap kích hoạt random sớm hay chậm
             * - khi trap hết thì cho chạy ltung như ở dưới reload animation
             * QL : -thời điểm trap on ********** biến SkewerScurry_map.isReadyTrap;
             *  - thời điểm trap off
             */

            #endregion
        }
        
        else
        {
            if (timeIdle > 0) timeIdle -= Time.fixedDeltaTime;
            if (timeRun > 0) timeRun -= Time.fixedDeltaTime;
            if (state_New == Char_State.Win || state_New == Char_State.Lose)
            {

            }
            else if (isAlive)
            {
                if (state_New == Char_State.Damaged)
                {
                    //Tự động Damaged và trờ về State Idle sau khi Damaged xong (Các Event gắn vào Animmation)
                }
                else if (state_New == Char_State.Attack)
                {
                    //Tự động Attack và trờ về State Idle sau khi Attack xong (Các Event gắn vào Animmation)
                }else if (Form_Gameplay.ins.minigameManager.minigame == Minigame.Slither ) {
                    state_New = Char_State.Run;
                } else if (state_New == Char_State.Idle)
                {
                    if (state_Apply == Char_State.Idle && timeIdle <= 0)
                    {
                        state_New = Char_State.Run;
                    }
                }
                else if (state_New == Char_State.Run || timeRun > 0)
                {
                    if (state_Apply == Char_State.Run && timeRun <= 0)
                    {
                        state_New = Char_State.Attack;
                    }
                }
            }
            else
            {
                if (state_New != Char_State.Death)
                {
                    state_New = Char_State.Death;
                }
            }
        }
        ReloadAnimation();
    }

    public override void ReloadAnimation()
    {
        if (state_New == Char_State.Run)
        {
            if (GameManager.ins.minigameManager.minigame == Minigame.HitAndRun)
            {

            }
            else if (GameManager.ins.minigameManager.minigame == Minigame.WackyRun)
            {
                transform.position = new Vector3(WackyRun_Manager.ins.nodes_Start[idChar].position.x, transform.position.y, transform.position.z);
                agent.enabled = false;
                transform.Translate(((new Vector3(pos_TargetMove.x, transform.position.y, pos_TargetMove.z) - transform.position).normalized) * Time.fixedDeltaTime * speed, Space.World);
            }
            else
            {
                if (GameManager.ins.minigameManager.minigame == Minigame.SnakeBlock)
                {
                }
                else if (GameManager.ins.minigameManager.minigame == Minigame.PlatformPush)
                {
                    if (Vector3.Distance(transform.position, pos_TargetMove) <= 3f || PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[idBlockTarget].isFlicker)
                    {
                        TargetBlockNext();
                    }
                } else if (GameManager.ins.minigameManager.minigame == Minigame.Slither) {
                    if (Vector3.Distance(transform.position, pos_TargetMove) <= 3f || (foodTarget != null && foodTarget.myChar != null)) {
                        TargetFoodNext();
                    }
                } else if (GameManager.ins.minigameManager.minigame == Minigame.SidestepSlope)
                {
                    if (Vector3.Distance(transform.position, pos_TargetMove) <= 10f)
                    {
                        idBlockTarget = Mathf.Clamp(idBlockTarget + 1, 0, SidestepSlope_Manager.ins.listTargetMove.Count - 1);
                        pos_TargetMove = new Vector3(SidestepSlope_Manager.ins.nodes_Start[idChar].position.x * 1.5f, SidestepSlope_Manager.ins.listTargetMove[SidestepSlope_Manager.ins.listChars[idChar].AIBot.idBlockTarget].transform.position.y, SidestepSlope_Manager.ins.listTargetMove[SidestepSlope_Manager.ins.listChars[idChar].AIBot.idBlockTarget].transform.position.z);
                    }
                }
                else if (GameManager.ins.minigameManager.minigame == Minigame.SkewerScurry)
                {
                    if (Vector3.Distance(transform.position, pos_TargetMove) <= 3f)
                    {
                        SkewerScurryTargetNextBlock();
                    }

                }
                else if (GameManager.ins.minigameManager.minigame == Minigame.OnTheCuttingBoard)
                {
                    if (Vector3.Distance(transform.position, pos_TargetMove) <= 10f)
                    {
                        OnTheCuttingBoardTargetNextPosMove();
                    }
                }
                else if (GameManager.ins.minigameManager.minigame == Minigame.GhostChaser)
                {
                    if (Vector3.Distance(transform.position, pos_TargetMove) <= 10f)
                    {
                        GhostChaserTargetNextPos();
                        isCheck_GhostChaser = false;
                    }
                    else if (GhostChaserCheckDistanceALlBoss() && !isCheck_GhostChaser)
                    {
                        GhostChaserTargetNextPosFarFormBoss(bossNearest);
                        isCheck_GhostChaser = true;
                    }

                }
                if (IsGrounded())
                {
                    if (GameManager.ins.minigameManager.minigame == Minigame.SidestepSlope && isFinish)
                    {
                        agent.enabled = false;
                        transform.Translate(((new Vector3(pos_TargetMove.x, transform.position.y, pos_TargetMove.z) - transform.position).normalized) * Time.fixedDeltaTime * speed, Space.World);
                    }
                    else
                    {
                        agent.enabled = true;
                        agent.speed = speed;
                        agent.SetDestination(pos_TargetMove);
                    }
                }
                else
                {
                    agent.enabled = false;
                    if (GameManager.ins.minigameManager.minigame == Minigame.PlatformPush)
                    {
                        transform.LookAt(new Vector3(pos_TargetMove.x, transform.position.y, pos_TargetMove.z));
                    }
                    transform.Translate(((new Vector3(pos_TargetMove.x, transform.position.y, pos_TargetMove.z) - transform.position).normalized - transform.up * 0.2f) * Time.fixedDeltaTime * speed, Space.World);
                }
            }
        }
        else
        {
            agent.enabled = false;
        }

        //state damaged
        if (state_New == Char_State.Damaged && isVelocityDamaged)
        {
            //fixed update
            Vector3 direction = transform.position - charAttacker.transform.position;
            myRigidbody.velocity = direction.normalized * speedBack * (charAttacker.isBooster ? 1.5f : 1);
        }

        if (state_Apply != state_New)
        {
            state_Apply = state_New;
            switch (state_Apply)
            {
                case Char_State.Attack:
                    modelChar.animator.SetTrigger(Enums.ins.dic_AniParams[AniParam.T_Attack]);
                    break;
                case Char_State.Damaged:
                    isDamaged = true;
                    isVelocityDamaged = true;
                    modelChar.idBrown = 2;
                    modelChar.idEye = 1;
                    modelChar.idLid = -1;
                    modelChar.idMounth = 1;
                    Vector3 direction = transform.position - charAttacker.transform.position;
                    //direction.y = 0;
                    float angleDiff = Vector3.Angle(direction, tran_Rotate.TransformDirection(Vector3.forward));
                    Form_Gameplay.ins.minigameManager.Create_EffectDamaged(transform.position);
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Damaged], true);
                    modelChar.animator.SetTrigger(angleDiff < 90 ? Enums.ins.dic_AniParams[AniParam.T_FrontHit] : Enums.ins.dic_AniParams[AniParam.T_BackHit]);
                    Timer.Schedule(this, 0.1f, () =>
                    {
                        isVelocityDamaged = false;
                    });
                    Timer.Schedule(this, 0.8f, () =>
                    {
                        isDamaged = false;
                        timeIdle = Random.Range(0.1f, 0.65f);
                        if (state_Apply == Char_State.Damaged) state_New = Char_State.Idle;
                        modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Damaged], false);
                    });
                    break;
                case Char_State.Death:
                    isAnimationDyning = true;
                    isFinish = false;
                    indicator.OnDeath();
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Win], false);
                    if (colliderKillChar == ColliderKillChar.Water)
                    {
                        sound_DieWater.PlaySound();
                        obj_Shadow.SetActive(false);
                        modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Drown], true);
                    }
                    else
                    {
                        myCollider.enabled = false;
                        myRigidbody.isKinematic = true;
                        obj_Shadow.SetActive(false);
                        effectBlood.SetActive(true);
                        modelChar.obj_Animation.SetActive(false);
                        modelChar.obj_Death.SetActive(true);
                        modelChar.ReloadModelDeath(colliderKillChar);
                    }
                    if (amountRevive > 0)
                    {
                        Timer.Schedule(this, 2f, () => { Reborn(); });
                    }
                    else
                    {
                        Timer.Schedule(this, 2.5f, () =>
                        {
                            isAnimationDyning = false;
                            modelChar.LockModelDeath(colliderKillChar);
                        });
                    }
                    break;
                case Char_State.Idle:
                    modelChar.idBrown = 0;
                    modelChar.idEye = 0;
                    modelChar.idLid = -1;
                    modelChar.idMounth = 7;
                    isVelocityDamaged = false;
                    isDamaged = false;
                    timeIdle = Random.Range(Form_Gameplay.ins.minigameManager.timeAI_Idle.x, Form_Gameplay.ins.minigameManager.timeAI_Idle.y);
                    modelChar.animator.SetFloat(Enums.ins.dic_AniParams[AniParam.F_Speed], 0);
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Drown], false);
                    break;
                case Char_State.Ragdoll:
                    break;
                case Char_State.Run:
                    modelChar.animator.SetFloat(Enums.ins.dic_AniParams[AniParam.F_Speed], 1);
                    timeRun = Random.Range(Form_Gameplay.ins.minigameManager.timeAI_Run.x, Form_Gameplay.ins.minigameManager.timeAI_Run.y);
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
                    modelChar.animator.SetBool(Enums.ins.dic_AniParams[AniParam.B_Lose], true);
                    break;
                default:
                    Debug.LogError("Lỗi ReloadAnimation() Player:" + state_Apply.ToString());
                    break;
            }
            modelChar.ReloadFace();
        }
    }
    private bool GhostChaserCheckDistanceALlBoss()
    {
        foreach (GhostChaser_BossManager b in _GhostChaser_Manager.ins.boss)
        {
            if (Vector3.Distance(b.transform.position,transform.position) <= b.rangeDetectAI)
            {
                bossNearest = b;
                return true;
            }
        }
        return false;
    }
    private void GhostChaserTargetNextPos()
    {
        //random 1 diem ngau nhien tren nav mesh
        //fai cach xa boss ít nhất range = rangeAttack cua boss
        //
        
        float minZoneX = _GhostChaser_Manager.ins.map.zoneNavMesh.position.x - (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.x / 2);
        float maxZoneX = _GhostChaser_Manager.ins.map.zoneNavMesh.position.x + (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.x / 2);

        float minZoneZ = _GhostChaser_Manager.ins.map.zoneNavMesh.position.z - (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.z / 2);
        float maxZoneZ = _GhostChaser_Manager.ins.map.zoneNavMesh.position.z + (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.z / 2);

        pos_TargetMove = new Vector3(Random.Range(minZoneX, maxZoneX),
            transform.position.y,
            Random.Range(minZoneZ, maxZoneZ));
    }
    private void GhostChaserTargetNextPosFarFormBoss(GhostChaser_BossManager bossNear_est)
    {
        float minZoneX = _GhostChaser_Manager.ins.map.zoneNavMesh.position.x - (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.x / 2);
        float maxZoneX = _GhostChaser_Manager.ins.map.zoneNavMesh.position.x + (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.x / 2);

        float minZoneZ = _GhostChaser_Manager.ins.map.zoneNavMesh.position.z - (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.z / 2);
        float maxZoneZ = _GhostChaser_Manager.ins.map.zoneNavMesh.position.z + (_GhostChaser_Manager.ins.map.zoneNavMesh.localScale.z / 2);

        Vector3 direction = new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f)).normalized;
        Vector3 TargetMove = bossNearest.transform.position + direction * Random.Range(150f,500f);
        pos_TargetMove = new Vector3(Mathf.Clamp(TargetMove.x,minZoneX,maxZoneX)
            ,TargetMove.y
            ,Mathf.Clamp(TargetMove.z,minZoneZ,maxZoneZ));
        
    }
    private void OnTheCuttingBoardTargetNextPosMove()
    {
        //move linh tinh ca len mien~ la` ở trong cái board đó
        Transform board = OnTheCuttingBoard_Manager.ins.mapManager.Board;
        float xMax = board.position.x + board.localScale.x / 2;
        float xMin = board.position.x - board.localScale.x / 2;
        float zMax = board.position.x + board.localScale.z / 2;
        float zMin = board.position.x - board.localScale.z / 2;
        float y = board.position.y + board.localScale.y / 2;

        Vector3 target = new Vector3(Random.Range(xMin, xMax), y, Random.Range(zMin, zMax));
        pos_TargetMove = target;
    }
    private void SkewerScurryTargetNextBlock()
    {

        ////random 1 trong những ô an toàn và đi vào
        if (timeAttack <= 0)
        {
            state_New = Char_State.Attack;
            timeAttack = Random.Range(1, 4f);
        }

        SkewerScurry_Block blockTarget = SkewerScurry_Manager.ins.mapManager.blocksClear[Random.Range(0, SkewerScurry_Manager.ins.mapManager.blocksClear.Count)];
        //random 1 vị trí bất kỳ thuộc cái block này
        //pos_TargetMove = new Vector3(Random.Range(blockTarget.transform.position.x - (SkewerScurry_Manager.ins.mapManager.widthBlock / 2), blockTarget.transform.position.x + (SkewerScurry_Manager.ins.mapManager.widthBlock / 2))//x
        //    , blockTarget.transform.position.y//y
        //    , Random.Range(blockTarget.transform.position.z - (SkewerScurry_Manager.ins.mapManager.heightBlock / 2), blockTarget.transform.position.z + (SkewerScurry_Manager.ins.mapManager.heightBlock / 2)));//z

        if (isRunSmart)
        {
            //check xem block clear nào gần nhất
            float minDis = 99999f;
            foreach (SkewerScurry_Block b in SkewerScurry_Manager.ins.mapManager.blocksClear)
            {
                if (Vector3.Distance(b.transform.position, blockOn.transform.position) <= minDis)
                {
                    minDis = Vector3.Distance(b.transform.position, blockOn.transform.position);
                    blockTarget = b;
                }
            }
            pos_TargetMove = new Vector3(Random.Range(blockTarget.transform.position.x - (SkewerScurry_Manager.ins.mapManager.widthBlock / 2), blockTarget.transform.position.x + (SkewerScurry_Manager.ins.mapManager.widthBlock / 2))//x
                , blockTarget.transform.position.y//y
                , Random.Range(blockTarget.transform.position.z - (SkewerScurry_Manager.ins.mapManager.heightBlock / 2), blockTarget.transform.position.z + (SkewerScurry_Manager.ins.mapManager.heightBlock / 2)));//z
        }
        else
        {
            pos_TargetMove = SkewerScurry_Manager.ins.mapManager.blocksClear[Random.Range(0, SkewerScurry_Manager.ins.mapManager.blocksClear.Count)].transform.position;
        }
        
    }
    IEnumerator I_waittrap()
    {
        yield return new WaitUntil(() => !SkewerScurry_Manager.ins.mapManager.isReadyTrap);
        isMove = false;
    }

    public void TargetBlockNext()
    {
        int orderBlockStand = PlatformPush_Manager.ins.platformPush_Map.FindIDBlockByPosition(transform.position);
        int orderList = orderBlockStand;
        List<int> listIDBlocksNeighbor = PlatformPush_Manager.ins.platformPush_Map.FindIDBlocksNeighborNoFall(orderBlockStand);
        if (listIDBlocksNeighbor != null && listIDBlocksNeighbor.Count > 0)
        {
            orderList = listIDBlocksNeighbor[Random.Range(0, listIDBlocksNeighbor.Count)];
        }
        else if (Vector3.Distance(transform.position, pos_TargetMove) > 3f)
        {
            return;
        }
        if (orderList < 0)
        {
            Debug.LogError("orderList = -1");
            return;
        }
        idBlockTarget = PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[orderList].id;
        pos_TargetMove = PlatformPush_Manager.ins.platformPush_Map.list_BlocksAll[orderList].transform.position;
        float radiusBlock = PlatformPush_Manager.ins.platformPush_Map.widthBlock / 2f;
        pos_TargetMove = new Vector3(pos_TargetMove.x + Random.Range(-radiusBlock, radiusBlock), pos_TargetMove.y, pos_TargetMove.z + Random.Range(-radiusBlock, radiusBlock));
    }

    public void TargetFoodNext() {//Đi ăn food gần nhất
        int idNearest = 0;
        float distanceMin = Slither_Manager.ins.slither_Map.listFood_NotUsed.Count > 0 ? (transform.position - Slither_Manager.ins.slither_Map.listFood_NotUsed [idNearest].transform.position).sqrMagnitude : 99999;
        for (int i = 1; i < Slither_Manager.ins.slither_Map.listFood_NotUsed.Count; i++) {
            float distanceCur = (transform.position - Slither_Manager.ins.slither_Map.listFood_NotUsed [i].transform.position).sqrMagnitude;
            if (distanceMin > distanceCur) {
                idNearest = i;
                distanceMin = distanceCur;
            }
        }
        foodTarget = Slither_Manager.ins.slither_Map.listFood_NotUsed [idNearest];
        pos_TargetMove = foodTarget.transform.position;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(pos_TargetMove,10f);
    }
}
