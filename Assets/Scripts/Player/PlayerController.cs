using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region ���
    [HideInInspector] public Rigidbody2D thisRB;
    [HideInInspector] public PhysicsRelated thisPR;
    [HideInInspector] public PlayerAnimatorController thisAC;
    [HideInInspector] public PlayerFXController thisFX;
    [HideInInspector] public CharacterRelated thisCR;
    [SerializeField] private ControllerManager theCM;
    private LevelController theLevel;
    private DialogeController theDC;
    private InputController theInput;
    public Transform thisDP;

    #endregion
    #region ״̬�����
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallFallState wallFallState { get; private set; }
    public PlayerWallJumpState walljumpState { get; private set; }
    public PlayerHoldState holdState { get; private set; }
    public PlayerWallClimbState wallClimbState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerUncontrolState uncontrolState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerBabbleState babbleState { get; private set; }
    #endregion
    #region ����
    [Header("ͨ��")]
    public bool isGameplay;
    public bool canAct;//�޷����в��������ڸ������á����鶯������ȫ�޷������κβ����ĳ���
    public float canActCounter;
    public float hurtUnactDuration;
    public float deadUnactDuration;
    public bool faceRight = true;//faceRight�ǿ���ͨ����������ⲿ�������ת���Ķ���������
    public int faceDir = 1;//faceDir��ͨ��������������жϵ������ǽ��
    public bool needTurnAround;//��Ҫͨ���ñ�ʶ����ת��
    //isUncontroling���ڱ�ʾ��uncontrolState�в�����Ϊ���޵ı�ʾ�������ǽ��ʱ��Ķ�ʱ���ڲ���ת��ͨ��������һ���������״̬�ͻ�ȡ���������ֲ��ı䵱ǰ״̬��
    //uncontrolState���ڱ�ʾ��ǰ״̬�½�ɫ������Ϊ���ޡ��������˺�Ķ���ʱ����ʲô�����ܸɣ�ͨ��������һ��ʱ����״̬�ͻ�ȡ��
    //unAct����ʹ������ͣ�����߸�����鶯��ʱ���û�п������൱���Ĳ�ȷ��ʱ���ڲ��ܲ�����ɫ��ֻ���ض�����Ϊ��ɺ����ȡ��
    public bool isUncontroling;
    public int horizontalInputVec; 
    public int verticalInputVec;

    [Header("��ǰ״̬����ֵ")]
    public string nowState;
    public float horizontalMoveSpeed;
    public float horizontalMoveSpeedMax;
    public float horizontalmoveThresholdSpeed;
    public float verticalFallSpeedMax;
    public float verticalMoveSpeed;
    public float verticalMoveSpeedMax;
    public float verticalThresholdSpeed;
    
    [Header("�ж�����")]
    public bool jumpAbility;
    public bool babbleAbility;
    public bool dashAbility;
    public bool holdAbility;

    [Header("�ж�״̬")]
    public bool canHorizontalMove;
    public bool canVerticalMove;
    public bool canJump;
    public bool canDash;
    public bool canHold;
    public bool canWallFall;
    public bool canWallJump;
    public bool canWallClimb;
    public bool canAttack;

    [HideInInspector] public bool canTurnAround;
    [HideInInspector]public bool canWallClimbForward;//Ҫ��¼��ε����ô�����

    [Header("����״̬")]
    public bool canBabble;
    public bool canCooldown;    

    [Header("ˮƽ�ƶ�")]
    public float normalmoveSpeed;
    public float normalmoveSpeedMax;
    public float normalmoveThresholdSpeed;


    [Header("����")]
    public float airmoveSpeed;
    public float airmoveSpeedMax;
    public float airFallSpeedMax;

    [Header("��Ծ")]
    public float jumpForce;
    public float peakSpeed;
    public float canJumpCounter;
    public float canJumpLength;
    private float jumpBufferLength;

    [Header("���")]
    public float dashCooldownCounter;
    public float dashCooldownLength;
    public float dashSpeed;
    public float dashDurationCounter;
    public float dashDurationLength;
    public float dashDir;
    public float dashEndDecrease;
    public bool dashEnd;

    [Header("ǽ�϶���")]
    public float wallFallSpeed;
    public float wallFallSpeedMax;
    public float wallClimbSpeed;
    public float wallClimbSpeedMax;

    [Header("��ǽ��")]
    public float wallJumpForce;
    public float wallJumpPostCounter;//����walljump����в���������
    public float wallJumpPostLength;

    [Header("����")]
    public bool keepInertia;


    [Header("��ɡ")]
    public float babbleCounter;
    public float babbleDuration;
    public float babbleCooldownCounter;
    public float babbleCooldownDuration;

    [Header("����")]
    public int attackCounter;
    public float continueAttackCounter;
    public float continueAttackDuration;
    public Vector2[] attackMoveVec2s;
    public Transform attackCheckPoint;
    public float attackCheckRadius;


    [Header("���ƿ���")]//���ѽ��в�������ǿ���ƶ����������ʱʹ��
    public float konckedBackCounter;
    public float knockedBackLength;
    public float knockedBackForce;
    public bool isUncontrol;
    public float uncontrolCounter;
    public float uncontrolDuration;


    [Header("����")]
    public float invinsibleCounter;
    public float invinsibleLength;

    [Header("�����븴��")]
    public float sitCounter;
    //public float sitLength;
    
    //[Header("������")]
    //public GameObject ropePrefab;
    //public GameObject indicationPrefab;
    //public RopeController theRope;
    //public bool hadPulled;
    //public float indicationCounter;
    //public float indicationDuration;
    //public float ropeDisappearCounter;
    //public float ropeDisappearDuration;
    //public Vector2 ropeDir;

    //public PullTargetController thePullTarget;
    //public float pullingCounter;
    //public float pullingDuration;
    //public float pullingForce;
    //public float pullingSpeedMax;
    //public bool keepInertia;
    //public float keepInertiaCounter;
    //public float keepInertiaDuration;


    //public bool isPullingTowards;
    //public float pullForceMax;
    //public float pullForce;

    //public float pullCooldownCounter;
    //public float pullCooldownDuration;


    //[Header("ң����")]
    //public BoomController theBoom;
    //public float boomUncontrolDuration;
    //public float boomCooldownCounter;
    //public float boomCooldownDuration;

    //[Header("Ϩ����")]
    //[Header("Mashroom More")]

    #endregion






    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisPR = GetComponent<PhysicsRelated>();
        thisAC = GetComponentInChildren<PlayerAnimatorController>();
        thisFX = GetComponentInChildren<PlayerFXController>();
        thisCR = GetComponentInChildren<CharacterRelated>();
        //thisNorCol = GetComponent<CircleCollider2D>();
        //thisTempCol = GetComponentInChildren<CapsuleCollider2D>();

    }

    private void Start()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "isIdling");
        moveState = new PlayerMoveState(this, stateMachine, "isHorizontalMoving");
        jumpState = new PlayerJumpState(this, stateMachine, "isAiring");
        wallFallState = new PlayerWallFallState(this, stateMachine, "isWallFalling");
        walljumpState = new PlayerWallJumpState(this, stateMachine, "isAiring");//����Ӧ�ز�
        holdState = new PlayerHoldState(this, stateMachine, "isHolding");
        wallClimbState = new PlayerWallClimbState(this, stateMachine, "isWallClimbing");
        attackState = new PlayerAttackState(this, stateMachine, "isAttacking");
        dashState = new PlayerDashState(this, stateMachine, "isDashing");
        airState = new PlayerAirState(this, stateMachine, "isAiring");
        uncontrolState = new PlayerUncontrolState(this, stateMachine, "isAiring");//��ʱ��
        babbleState = new PlayerBabbleState(this, stateMachine, "isBabbling");
        deadState = new PlayerDeadState(this, stateMachine, "isDeading");

        theLevel = theCM.theLevel;
        theDC = theCM.theDC;
        theInput = theCM.theInput;
        OriginPlayerData();
    }

    // Update is called once per frame
    private void Update()
    {
        MovementVecUpdate();
        if (isGameplay)
        {
            GameplayCooldownCount();
            GameplayCount();
            CanActCount();
            
        }
    }


    private void FixedUpdate()
    {
        if (isGameplay)
        {
            thisPR.PRUpdate();
            stateMachine.currentState.Update();
            TurnAround();
            FaceDirUpdate();
        }
    }

    private void TurnAround()
    {
        if (canTurnAround&&needTurnAround)
        {
            Debug.Log("ת��");
            faceRight = !faceRight;
            needTurnAround = false;
        }
    }

    public void Unact(float _unactDuration)
    {
        canAct = false;
        canActCounter = _unactDuration;
    }



    public virtual void CanActCount()
    {
        if (canActCounter > 0 && !canAct)
        {
            canActCounter -= Time.deltaTime;
        }
        else
        {
            if (!theDC.isDialogue)
                canAct = true;
        }
    }


    private void OriginPlayerData()
    {
        thisRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        thisRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        isGameplay = true;
        canAct = false;
        if (theLevel.currentSceneName == "Level1")
        {
            airmoveSpeed = 0;

        }
        verticalFallSpeedMax = airFallSpeedMax;
        stateMachine.Initialize(airState);//������Ҫ�����������жϵ�ǰӦ�ô���ʲô״̬
        //mushroomPoint = thisPR.theGroundCheckpoint;
        SkillFresh();
    }

    private void MovementVecUpdate()
    {
        if (theCM.theInput.horizontalInputVec == faceDir && thisPR.IsOnWall())
        {
            horizontalInputVec = 0;
        }
        else
        {
            horizontalInputVec = theInput.horizontalInputVec;
        }
        verticalInputVec = theInput.verticalInputVec;
    }

    public void SkillFresh()//ǿ��ˢ�����м���
    {
        if (babbleAbility) canBabble = true;
    }

    public void StateEndSkillFresh()
    {
        if (babbleAbility && babbleCooldownCounter < 0) canBabble = true;
    }

    public void TurnOffStandBox()
    {
        //if (����Ҫ��ʱ��ײ��) thisTempCol.enabled = false;
        //thisNorCol.isTrigger = false;
    }
    public void TurnOnStandBox()
    {
        //if (��Ҫ��ʱ��ײ��) thisTempCol.enabled = true;
        //thisNorCol.isTrigger = true;
    }

    public void DisablizeColliders()
    {
        //thisTempCol.enabled = false;
        //thisNorCol.enabled = false;
    }

    public void AblizeColliders()
    {
        //thisTempCol.enabled = false;
        //thisNorCol.enabled = true;
        //thisNorCol.isTrigger = false;

    }




    #region �ⲿ�ı�״̬����
    public void StateOver()
    {
        if (thisPR.IsOnGround()) stateMachine.ChangeState(moveState);
        else stateMachine.ChangeState(airState);
    }

    public void ChangeToHorizontalMoving()
    {
        Debug.Log("����?");
        stateMachine.ChangeState(moveState);
    }

    public void ChangeToAirState()
    {
        stateMachine.ChangeState(airState);
    }
    public void ChangeToJumpState()
    {
        stateMachine.ChangeState(jumpState);

    }
    public void ChangeToBabbleState()
    {
        if (babbleAbility && canBabble)
            stateMachine.ChangeState(babbleState);
    }

    public void ChangeToUncontrolState()
    {
        stateMachine.ChangeState(uncontrolState);
    }


    public void ChangeToDeadState()
    {
        stateMachine.ChangeState(deadState);
    }

    public void ChangeToIdleState()
    {
        stateMachine.ChangeState(idleState);
    }



    public void PlayerReset()
    {
        theLevel.Respawn();
    }

    public void AirmoveRecover()
    {
        airmoveSpeed = 20;
    }
    #endregion
    #region ���Է���
    public void ActivateLevelInteractiveable()
    {
        //���������ڿ�ң�ػ���
    }
    #endregion

    #region Gameplay����
    #region Can�ж�
    public void WhetherCanJumpOrWallJump()//TD�������ǽ�����ǽ����������
    {
        if (canJumpCounter > 0f)
        {
            if (stateMachine.currentState == wallFallState || stateMachine.currentState == wallClimbState || stateMachine.currentState == holdState)
            {
                canJump = false;
                canWallJump = true;
            }
            else if(stateMachine.currentState == idleState|| stateMachine.currentState == moveState)
            {
                canJump = true;
                canWallJump = false;
            }
            if (stateMachine.currentState == attackState) 
            {
                canJump = false;
                canWallJump = false;
            }
            else
            {
                //Debug.Log("�ڴ��ж��Ƿ���Բ�ǽ��");
                //ʵ���ϣ���̺ͷǿظ������ᵽ�⣬���������ǽ�����Ծ����ʹ�ɡʱ�����̨���ж�
                canJump = true;
                canWallJump = false;
            }
            canJumpCounter -= Time.deltaTime;
        }
        else
        {
            //��̺ͷǿ�ֱ�ӽ���̨����ʱ��0������ʵ���޷���Ծ
            //����������Խ�����Ծ����ʹ�ɡʱ��Ĳ�ǽ�ж�
            canJump = false;
            canWallJump = false;
        }

    }


    public void WhetherCanHold()
    {
        if (thisPR.IsOnWall() && stateMachine.currentState != dashState) canHold = true;
        else canHold = false;
    }
    public void WhetherCanWallFall()
    {
        if (!thisPR.IsOnGround() && thisPR.IsOnWall() && thisRB.velocity.y <= 0.1 && stateMachine.currentState != dashState) canWallFall = true;
        else canWallFall = false;
    }

    public void WhetherCanDash()//Todo:���޸�
    {
        if (dashCooldownCounter < 0)
        {
            if (stateMachine.currentState == attackState) canDash = false;
            else canDash = true;
        }
        else
        {
            dashCooldownCounter -= Time.deltaTime;
            canDash = false;
        }
    }
    public void WhetherCanWallVeritalForward()//TD:�����������ƶ�����������ȥ�ж�
    {
        if (thisPR.isBackWall)
        {
            canWallClimbForward = true;
        }
        else
        {
            canWallClimbForward = false;
        }
    }
    //public void WhetherCanBladeAtttack()
    //{
    //    if (!thisPR.isWall && !isAttack && !isPosForce)
    //    {

    //    }
    //}


    public void RefreshCanJump()
    {
        canJumpCounter = canJumpLength;
    }
    public void RefreshCanDash_CooldownNeglect()
    {
        canDash = true;
    }

    #endregion 

    #region ����
    public void FaceDirUpdate()
    {
        if (stateMachine.currentState != uncontrolState)
        {

            if (horizontalInputVec > 0) faceRight = true;//MoveToDir,�����ж������Ҫ�ƶ��ķ���
            else if (horizontalInputVec < 0) faceRight = false;
            else
            {
                Debug.Log("���򲻱�");
            }

            faceDir = (int)transform.localScale.x;
            if(thisRB.velocity.x!=0) faceDir = faceRight ? 1 : -1;
            transform.localScale = new Vector3(faceDir, 1, 1);
        }
        else
        {
            //Debug.Log("���ܿ���״̬����Ȼ����ת��");
        }
    }



    public void JumpBufferCheck()
    {
        StartCoroutine(JumpBufferCheckCo(jumpBufferLength));
    }
    private IEnumerator JumpBufferCheckCo(float counter)
    {
        while (counter > 0)
        {
            //Debug.Log("��ʼ��⻺��");
            //if (canJump)
            //{
            //    if (stateMachine.currentState == holdState || stateMachine.currentState == wallfallState) stateMachine.ChangeState(walljumpState);
            //    else stateMachine.ChangeState(jumpState);
            //    //Debug.Log("������Ծ�ɹ�");
            //    break;
            //}
            yield return new WaitForSeconds(Time.deltaTime);
            //counter -= Time.deltaTime;
        }
        //if(counter<=0) Debug.Log("������⻺��");
    }
    public void Fall()
    {

        if (thisRB.velocity.y < -verticalFallSpeedMax)
        {
            thisRB.velocity += new Vector2(0, -verticalFallSpeedMax - thisRB.velocity.y);
            //Debug.Log(thisRB.velocity.y);
        }

    }

    #region WallAct���

    public void Hold()
    {
        ClearVelocity();
        verticalFallSpeedMax = 0f;
        thisPR.GravityLock(0);
        thisPR.isHolding = true;//������������PR��ʵҲ����0���� �����費��Ҫ�Ż�
        canTurnAround = false;
    }

    public void EndHold()
    {
        verticalFallSpeedMax = airFallSpeedMax;
        thisPR.GravityUnlock();
        thisPR.isHolding = false;
        canTurnAround = true;
    }




    public void ClimbPrepare()
    {
        if (stateMachine.currentState == wallClimbState)
        {

            thisPR.GravityLock(0);
            canTurnAround = false;
            thisPR.isHolding = true;
            verticalMoveSpeed = wallClimbSpeed;
            verticalMoveSpeedMax = wallClimbSpeedMax;
        }
        //else if (stateMachine.currentState == ropeClimbState)
        //{
        //    climbSpeed = wallClimbSpeed;
        //    climbSpeedMax = wallClimbSpeedMax;
        //}
    }

    public void ClimbEnd()
    {
        {
            if (stateMachine.currentState == wallClimbState)
            {

                thisPR.GravityUnlock();
                canTurnAround = true;
                thisPR.isHolding = false;
            }
            //else if (stateMachine.currentState == ropeClimbState)
            //{
            //    climbSpeed = wallClimbSpeed;
            //    climbSpeedMax = wallClimbSpeedMax;
            //}
        }
    }

    public void Climb()
    {
        if (Mathf.Abs(thisRB.velocity.y + verticalInputVec * verticalMoveSpeed * Time.deltaTime) < verticalMoveSpeedMax)//�ڿ��ǵ�������У��÷�������һ��Ч����ͬ
        {
            if (Mathf.Abs(thisRB.velocity.y) < verticalThresholdSpeed) thisRB.velocity += new Vector2(0f, verticalInputVec * (verticalThresholdSpeed + verticalMoveSpeed * Time.deltaTime));
            else
                thisRB.velocity += new Vector2(0f, verticalInputVec * verticalMoveSpeed * Time.deltaTime);
        }
    }
    #endregion


    #region Attack���



    public void AtttackOnGround()
    {
        canAttack = false;
        thisAC.currentAttack = thisAC.bladeAttackOnGoundIdentity[attackCounter];

    }



    public void BladeAttackEnd()
    {
        thisPR.GravityUnlock();
        continueAttackCounter = continueAttackDuration;
    }








    #endregion

    #region Dash���
    public void Dash()
    {
        //Debug.Log("�壡");
        ClearVelocity();
        thisPR.GravityLock(0f);
        dashEnd = false;
        thisRB.velocity += new Vector2(-thisRB.velocity.x + dashDir * dashSpeed, 0);
        //canDoubleJumpTemp = canDoubleJump;
        dashCooldownCounter = dashCooldownLength;
        dashDurationCounter = dashDurationLength;
        //isDashing = true;

    }
    public void DashKeep()
    {
        if (dashDurationCounter > 0.1f)
        {

            //thisRB.velocity += new Vector2(faceDir * dashSpeed * Time.deltaTime, -thisRB.velocity.y);
            dashDurationCounter -= Time.deltaTime;
        }
        else if (dashDurationCounter > 0f)
        {
            dashEnd = true;
            dashDurationCounter -= Time.deltaTime;
        }
        else
        {
            if (thisPR.IsOnGround())
            {
                stateMachine.ChangeState(idleState);
                return;
            }
            else if (thisPR.IsOnWall())
            {
                Debug.Log("�ж��˵�");
                WhetherHoldOrWallFall();
            }
            else
            {
                stateMachine.ChangeState(airState);
                return;
            }
        }
    }
    public void DashEnd()
    {

        //thisAC.DashTrigger();
        dashEnd = false;
        thisRB.velocity += new Vector2(dashEndDecrease * -thisRB.velocity.x, -thisRB.velocity.y);
        thisPR.GravityUnlock();
        //canDoubleJump = canDoubleJumpTemp;
    }
    #endregion


    #region
    public void DeadEnter()
    {
        canAct = false;

        theLevel.Respawn();
    }

    public void DeadEnd()
    {

    }
    public void ChangeToIdle() => stateMachine.ChangeState(idleState);
    //public void ChangeToStand() => stateMachine.ChangeState(standUpState);
    #endregion

    #region ��ʱ���õĶ���
    //public void NewAttack()
    //{
    //    ClearVelocity();
    //    Debug.Log("����ʱ����" + accumulateCounter);
    //    accumulateCounter = 0;
    //    attackCombo = 0;
    //    Debug.Log("��ʼ��������" + accumulateCounter);
    //}
    //public void Attack()
    //{

    //    if (attackCombo < 2 && canAttack)
    //    {
    //        canAttack = false;
    //        attackCombo++;
    //    }

    //    thisAC.currentAttack = thisAC.AttackIdentity[attackCombo];
    //    thisAC.AttackTrigger();
    //}
    //public void Accumulate()
    //{
    //    if (Input.GetKey(KeyCode.Z) && attackCombo == 0) accumulateCounter += Time.deltaTime;

    //}
    //public void Slash()
    //{
    //    canAttack = false;
    //    canDash = false;
    //    isSlash = true;
    //    thisAC.currentAttack = thisAC.AttackIdentity[0];
    //    thisAC.AttackTrigger();
    //}
    //public void AttackEnd()
    //{
    //    attackCombo = 0;
    //    isSlash = false;
    //    thisAC.AttackTrigger();
    //}
    //private void Slide()
    //{
    //    isCrouching = false;
    //    isSliding = true;
    //    slideSpeed = slideStartSpeed;
    //    //int slideDir = SlideDir();
    //    slideCounter = slideLength;
    //    //Debug.Log("��ʼ����" + "  "+ slideCounter);
    //    thisRB.velocity += new Vector2(-thisRB.velocity.x + slideStartSpeed * Time.deltaTime, 0f);
    //}
    //private void SlideKeep()
    //{
    //    if (slideCounter > 0f)
    //    {
    //        thisRB.velocity += new Vector2(slideSpeed * faceRight * Time.deltaTime, 0f);
    //        slideSpeed = Mathf.Lerp(slideSpeed, 0f, slideLerp);
    //        slideCounter -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        SlideEnd();
    //        ClearVelocity();
    //    }
    //}
    //private void SlideEnd()
    //{
    //    isSliding = false;
    //    if (thisPR.isGround)
    //    {
    //        WhetherCrouch();
    //    }
    //    else
    //    {
    //        isCrouching = false;
    //        //Debug.Log("��������");
    //        moveSpeed = wanderSpeed;
    //        moveSpeedMax = runSpeedMax;
    //    }
    //}
    //private void Crouch()
    //{
    //    //isCrouching = true;
    //    moveSpeed = crouchSpeed;
    //    moveSpeedMax = crouchSpeedMax;
    //}
    //private void RiseUp()
    //{
    //    //Debug.Log("����");
    //    isCrouching = false;
    //    moveSpeed = wanderSpeed;
    //    moveSpeedMax = runSpeedMax;
    //}



    #endregion
    #endregion
    #region RB��ط���
    public void ClearVelocity()
    {
        thisRB.velocity += new Vector2(-thisRB.velocity.x, -thisRB.velocity.y);
    }

    public void ClearYVelocity()//�������PlayerY���ϵ��ٶ�
    {
        thisRB.velocity += new Vector2(0, -thisRB.velocity.y);

    }
    public void ClearXVelocity()//�������PlayerX���ϵ��ٶ�
    {
        thisRB.velocity += new Vector2(-thisRB.velocity.x, 0);

    }

    public void GroundVelocityLimit()//������������ƶ��ٶ�����
    {
        if (Mathf.Abs(thisRB.velocity.x) > horizontalMoveSpeedMax)
        {
            //Debug.Log("����");
            thisRB.velocity += new Vector2(faceDir * horizontalMoveSpeedMax - thisRB.velocity.x, 0f);
        }

    }

    public void GroundInertialClear()//����������Idle״̬�µĲ�������
    {
        if (Mathf.Abs(thisRB.velocity.x) < horizontalmoveThresholdSpeed) thisRB.velocity += new Vector2(-thisRB.velocity.x, 0f);
    }
    public void InertiaXVelocity()
    {
        //Debug.Log(Mathf.Lerp(Mathf.Abs(thisRB.velocity.x), moveSpeedMax, .1f));
        thisRB.velocity = new Vector2(faceDir * Mathf.Lerp(Mathf.Abs(thisRB.velocity.x), horizontalMoveSpeedMax, .1f), 0f);
        //Debug.Log("�ڹ��Լ���" + thisRB.velocity.x);
    }
    #endregion
    #region PR��ط���

    public void WhetherHoldOrWallFall()//TD����������൱���ӣ���Ҫ���
    {

        if (stateMachine.currentState == wallFallState)
        {
            if (canAct && canHold && theInput.WhetherZPressing())
            {
                if (canVerticalMove && verticalInputVec != 0)
                {
                    stateMachine.ChangeState(wallClimbState);
                    return;
                }
                else
                {
                    stateMachine.ChangeState(holdState);
                    return;
                }
            }
            //wallJump���ͷ���ж���ʱ��canHold��һ��ʱ����false,Ϊ����ǽ�ϻ�ǽ�߰�����Ծ�ܹ�����������������Ծ˲�����ж�ץס
            else if (!canAct || !canWallFall || horizontalInputVec != faceDir)
            {
                thisAC.FlipX();
                stateMachine.ChangeState(airState);
                return;
            }
        }
        else if (stateMachine.currentState == wallClimbState)
        {
            if (canAct && canHold && theInput.WhetherZPressing())
            {
                if (!canVerticalMove || verticalInputVec == 0)
                {
                    stateMachine.ChangeState(holdState);
                    return;
                }
            }
            else if (canAct && canWallFall && horizontalInputVec == faceDir)
            {
                stateMachine.ChangeState(wallFallState);

            }
            else
            {
                thisAC.FlipX();
                stateMachine.ChangeState(airState);
                return;
            }
        }
        else if (stateMachine.currentState == holdState)
        {
            if(!canAct || !canHold || !theInput.WhetherZPressing())
            {
                thisAC.FlipX();
                stateMachine.ChangeState(airState);
                return;
            }
            else if (canVerticalMove)
            {
                if (verticalInputVec > 0)
                {
                    //if(player.canHorizontalMove)
                    //{
                    //  stateMachine.ChangeState(player.climbOnState);
                    //}
                    //else{
                    stateMachine.ChangeState(wallClimbState);
                    return;
                    //}
                }
                else if (verticalInputVec < 0)
                {
                    stateMachine.ChangeState(wallClimbState);
                    return;
                }
                else
                {
                    //Debug.Log("ά�ֵ�ǰ");
                    return;
                }
            }
            else if (canWallFall && horizontalInputVec == faceDir)
            {
                stateMachine.ChangeState(wallFallState);
                return;
            }
            else
            {
                thisAC.FlipX();
                stateMachine.ChangeState(airState);
                return;
            }
        }
        else if (stateMachine.currentState == jumpState || stateMachine.currentState == walljumpState || stateMachine.currentState == airState)
        {
            if (canAct && canHold && theInput.WhetherZPressing())
            {
                if (canVerticalMove)
                {
                    if (verticalInputVec > 0)
                    {
                        //if(player.canHorizontalMove)
                        //{
                        //  stateMachine.ChangeState(player.climbOnState);
                        //}
                        //else{
                        thisAC.FlipX();
                        stateMachine.ChangeState(wallClimbState);
                        return;
                        //}
                    }
                    else if (verticalInputVec < 0)
                    {
                        thisAC.FlipX();
                        stateMachine.ChangeState(wallClimbState);
                        return;
                    }
                    else
                    {
                        //Debug.Log("ά�ֵ�ǰ");
                        return;
                    }
                }
                else
                {
                    thisAC.FlipX();
                    stateMachine.ChangeState(holdState);
                    return;
                }
            }
            if (canAct && canWallFall && horizontalInputVec == faceDir)
            {
                thisAC.FlipX();
                stateMachine.ChangeState(wallFallState);
                return;
            }
            else
            {

                //Debug.Log("ά����״����");
                return;
            }
        }
        else
        {
            stateMachine.ChangeState(airState);
            return;
        }
    }
    public void IsPeak()//�����жϿ���Player���ڵ�λ�ã�����PR�ڿ���ʵ�������ĵ���
    {
        if (Mathf.Abs(thisRB.velocity.y) < peakSpeed)
        {
            //Debug.Log("���ڷɣ�");
            thisPR.isPeak = true;
        }
        else
        {
            thisPR.isPeak = false;
        }
    }


    #endregion
    #region �����Է���
    private void GameplayCooldownCount()
    {
        if (babbleCooldownCounter > 0)
        {
            babbleCooldownCounter -= Time.deltaTime;
        }
        else
        {
            if (canCooldown)
                canBabble = true;
        }
        if (uncontrolCounter >= 0)
        {
            uncontrolCounter -= Time.deltaTime;
        }
        else
        {
            isUncontrol = false;
        }
        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.deltaTime;
        }
        else
        {

        }

        if (invinsibleCounter > 0)
        {
            invinsibleCounter -= Time.deltaTime;
        }
    }


    private void GameplayCount()
    {
        if (continueAttackCounter > 0)
        {
            continueAttackCounter -= Time.deltaTime;
        }
    }

    public void CurrentStateOver() => stateMachine.currentState.CurrentStateEnd();
    //public void ChangeToKnockBack() => stateMachine.ChangeState(knockedBackState);



    public void KnockedBack(Vector2 _knockedBackDir)
    {
        float finalForce = knockedBackForce / thisRB.mass;
        thisRB.AddForce(finalForce * _knockedBackDir, ForceMode2D.Impulse);
        //Debug.Log("�������" + finalForce * _knockedBackDir.x);
        // SetVelocity(finalForce * _knockedBackDir.x, finalForce * _knockedBackDir.y);
        isUncontrol = true;
        uncontrolCounter = uncontrolDuration;

    }
    #endregion

    #endregion

    #region VideoPlay����


    public void VisualTurnAround()
    {

        faceRight = !faceRight;
        faceDir = -faceDir;
    }

    //private void StopAction()
    //{
    //    switch (thePlayerState)
    //    {
    //    }
    //}
    #endregion

    #region Character��ط���





    public void TakeDamage(int _damageValue)//ֻ�����������ֵ��ص�����
    {
        //thisFX.StartCoroutine(thisFX.FlashEffect());
        if (invinsibleCounter <= 0 && thisCR.currentHP > 0)
        {
            thisCR.currentHP -= _damageValue;
            if (thisCR.currentHP > 0) Hurt();
            else stateMachine.ChangeState(deadState);
        }
        //Debug.Log("��������" + _damageValue + "���˺�");
    }



    public void Hurt()
    {

        //������������Ч
        canActCounter = hurtUnactDuration;
        invinsibleCounter = invinsibleLength;

    }
    #endregion
}
