using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Rendering.ShadowCascadeGUI;

public class NewPlayerController : MonoBehaviour
{
    #region ���
    [HideInInspector] public Rigidbody2D thisRB;
    [HideInInspector] public PhysicsRelated thisPR;
    [HideInInspector] public PlayerAnimatorController thisAC;
    [HideInInspector] public PlayerFXController thisFX;
    [HideInInspector] public CharacterRelated thisCR;
    [SerializeField] private ControllerManager theCM;


    public ActionStat actionStats;

    private LevelController theLevel;
    private DialogeController theDC;
    private InputController theInput;
    public Transform thisDP;
    public BoxCollider2D thisBoxCol;



    #endregion
    #region ״̬�����
    public NewPlayerStateMachine stateMachine { get; private set; }
    public NewPlayerIdleState idleState { get; private set; }
    public NewPlayerRunState runState { get; private set; }
    public NewPlayerRiseState jumpState { get; private set; }
    public NewPlayerFallState fallState { get; private set; }
    public NewPlayerAttackState attackState { get; private set; }
    public NewPlayerUmbrellaState umbrellaState { get; private set; }
    public NewPlayerHandleState handleState { get; private set; }
    //public NewPlayerApexState apexState { get; private set; }

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
    public float horizontalMoveSpeedAccleration;
    public float horizontalMoveSpeed;
    public float horizontalMoveSpeedMax;
    public float horizontalmoveThresholdSpeed;
    //public float verticalFallAccleration;
    public float verticalFallSpeedMax;
    public float verticalMoveSpeed;
    public float verticalMoveSpeedMax;
    public float verticalThresholdSpeed;

    [Header("�ж�����")]
    public bool jumpAbility;
    public bool umbrellaAbility;
    public bool dashAbility;


    [Header("�ж�״̬")]
    public bool canHorizontalMove;
    public bool canVerticalMove;
    public bool canJump;
    public bool canDash;
    public bool canHold;
    public bool canWallJump;
    public bool canAttack;
    public bool canUmbrella;

    public bool canTurnAround;
    [HideInInspector] public bool canWallClimbForward;//Ҫ��¼��ε����ô�����

    [Header("����״̬")]
    public bool canBabble;
    public bool canCooldown;

    [Header("ˮƽ�ƶ�")]
    public float normalmoveAccleration;
    //public float normalmoveSpeed;
    public float normalmoveSpeedMax;
    public float normalmoveThresholdSpeed;


    [Header("����")]
    private bool isFastFalling;
    private float fastFallTime;
    private float fastfallReleaseSpeed;



    public float apexCounter;
    public float apexDuration;
    public float apexThresholdLength;
    public float holdingCounter;
    public bool isPastApexThreshold;

    public float airmoveAccleration;
    public float airmoveSpeedMax;
    public float airmoveThresholdSpeed;
    public float airFallSpeedMax;


    [Header("��Ծ")]
    public float jumpForce;
    public float peakSpeed;

    public bool releaseDuringRising;//��falseʱ����ζ���������Ծ�����Ұ�ס����Ծ������tureʱ����ζ����Ҳ�û����Ծ��������Ծ�Ѿ��½��ˡ������������������ɿ�����Ծ��
    private float coyoteJumpCounter; //coyote
    public float coyoteJumpLength;
    public float jumpBufferLength;


    [Header("��ɡ")]
    public float umbrellaMoveAccelaration;
    public float umbrellaMoveThresholdSpeed;
    public float umbrellaMoveSpeedMax;
    public float umbrellaFallSpeedMax;

    [Header("����")]
    public int attackCounter;
    public float continueAttackCounter;
    public float continueAttackDuration;
    public float attackCooldownCounter;
    public float attackCooldownDuration;
    public Vector2[] attackMoveVec2s;//��ͬ�Ĺ���������ͬ�Ĺ�������



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

    [Header("����̨")]
    public HandlerController theHandle;

    [Header("�������")]
    public IInteract theInteractable;
    #endregion


    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisPR = GetComponent<PhysicsRelated>();
        thisAC = GetComponentInChildren<PlayerAnimatorController>();
        thisFX = GetComponentInChildren<PlayerFXController>();
        thisCR = GetComponentInChildren<CharacterRelated>();
        thisBoxCol = GetComponent<BoxCollider2D>();

    }
    // Start is called before the first frame update
    private void Start()
    {
        stateMachine = new NewPlayerStateMachine();
        idleState = new NewPlayerIdleState(this, stateMachine, "isIdling");
        runState = new NewPlayerRunState(this, stateMachine, "isHorizontalMoving");
        jumpState = new NewPlayerRiseState(this, stateMachine, "isJumping");
        fallState = new NewPlayerFallState(this, stateMachine, "isFalling");
        attackState =new NewPlayerAttackState(this, stateMachine, "isAttacking");
        umbrellaState = new NewPlayerUmbrellaState(this, stateMachine, "isUmbrellaing");
        handleState = new NewPlayerHandleState(this, stateMachine, "isIdling");
        //apexState = new NewPlayerApexState(this, stateMachine, "isFalling");
        theLevel = theCM.theLevel;
        theDC = theCM.theDC;
        theInput = theCM.theInput;
        OriginPlayerData();
    }

    // Update is called once per frame
    private void Update()
    {
        InputProcess();
        if (isGameplay)
        {
            thisPR.PRUpdate();
            GameplayCount();
            GameplayCooldownCount();
            CanActCount();
            stateMachine.currentState.Update();

        }
    }

    private void FixedUpdate()
    {
        if (isGameplay)
        {
            thisPR.PRFixUpdate();
            PR_GravityRelatedUpdate();
            stateMachine.currentState.FixedUpdate();
            //TurnAround();
            FaceDirUpdate();
        }

    }

    private void OriginPlayerData()
    {
        thisRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        thisRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        isGameplay = true;
        canAct = false;
        verticalFallSpeedMax = airFallSpeedMax;
        stateMachine.Initialize(fallState);//������Ҫ�����������жϵ�ǰӦ�ô���ʲô״̬
        //mushroomPoint = thisPR.theGroundCheckpoint;
    }


    public void Unact(float _unactDuration)
    {
        canAct = false;
        canActCounter = _unactDuration;
    }
    #region ���������


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<FloorController>())
        {
            FloorController theFloor = other.GetComponent<FloorController>();
            theFloor.SetPlayer(this);
            theFloor.Floor_Enter();
        }
    }


    private void OnTriggerStay2D(Collider2D other)//Enter��˲�����ComCol��û��⵽
    {
        if (other.GetComponent<PlatformController>())
        {
            PlatformController thePlatform = other.GetComponent<PlatformController>();
            if (//thePlatform.GetPlayer() == null && 
                thePlatform.GetComCol() == thisPR.RayHit().collider)
            {
                thePlatform.SetPlayer(this);
                if (!thePlatform.hasSensored)
                {
                    //Debug.Log("����");
                    thePlatform.Interactive_Sensor();
                    thePlatform.hasSensored = true;
                }

            }

        }else if (other.GetComponent<FloorController>())
        {
            FloorController theFloor = other.GetComponent<FloorController>();
            theFloor.Floor_Stay(); 
        }
    }






    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlatformController>())
        {
            PlatformController thePlatform = other.GetComponent<PlatformController>();
                //Debug.Log("�˳�");
            //if (
            //    //thePlatform.GetPlayer() == this&& 
            //    stateMachine.currentState==jumpState)
            {
                thePlatform.SetPlayer(null);

                //this.transform.parent=null;
            }
            thePlatform.hasSensored = false;
        }
        else if(other.GetComponent<FloorController>()) 
        {
            other.GetComponent<FloorController>().ClearPlayer();
            //theFloor.Floor_Exit();
        }
    }
    #endregion

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

    private void TurnAround()
    {

    }
    #region �ı�״̬�Ĵ���
    public void StateOver()
    {
        if (thisPR.IsOnFloored()) ChangeToIdleState();
        else stateMachine.ChangeState(fallState);
    }
    public NewPlayerState CurrentState()
    {
        return stateMachine.currentState;
    }
    public void ChangeToIdleState()
    {
        stateMachine.ChangeState(idleState);
    }


    public void ChangeToHorizontalMoving()
    {
        stateMachine.ChangeState(runState);
    }

    //public void ChangeToApexState()
    //{
    //    stateMachine.ChangeState(apexState);
    //}
    public void ChangeToFallState()
    {
        stateMachine.ChangeState(fallState);
    }
    public void ChangeToJumpState()
    {
        stateMachine.ChangeState(jumpState);

    }
    public void ChangeToAttackState()
    {
        stateMachine.ChangeState(attackState);
    }

    public void ChangeToHandleState()
    {
        stateMachine.ChangeState(handleState);
    }

    //public void ChangeToUmbrellaState()
    //{
    //    if (umbrellaAbility && canBabble)
    //        stateMachine.ChangeState(babbleState);
    //}

    //public void ChangeToUncontrolState()
    //{
    //    stateMachine.ChangeState(uncontrolState);
    //}


    //public void ChangeToDeadState()
    //{
    //    stateMachine.ChangeState(deadState);
    //}
    public void PlayerReset()
    {
        theLevel.Respawn();
    }
    #endregion


    #region Gameplay����
    #region Can�ж�
    public void WhetherCanJumpOrWallJump()//��Ϊ��ͨ���͵�ǽ��������Ծ�������⣬������һ���ж�
    {
        if (coyoteJumpCounter > 0f)
        {

            if (!thisPR.IsOnFloored())
            {
                if (thisPR.IsOnWall())
                {
                    canJump = false;
                    canWallJump = true;
                }
                else
                {
                    canJump = true;
                    canWallJump = false;
                }
                coyoteJumpCounter -= Time.deltaTime;
                //Debug.Log(coyoteJumpCounter);
            }
            else
            {
                canJump = true;
                canWallJump = false;
            }
        }
        else
        {
            //��̨����ʱ��0������ʵ���޷���Ծ
            //����������Խ�����Ծ����ʹ�ɡʱ��Ĳ�ǽ�ж�
            canJump = false;
            canWallJump = false;
        }

    }

    public void CoyoteCounterZero()
    {
        coyoteJumpCounter = 0f;
    }

    public void WhetherCanAttack()
    {
        if (attackCooldownCounter<=0)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }


    public void RefreshCanJump()
    {
        coyoteJumpCounter = coyoteJumpLength;
    }

    #endregion 

    #region ����
    public void FaceDirUpdate()
    {
        if (isGameplay)
        {

            if (canTurnAround)
            {
                faceDir = (int)transform.localScale.x;
                if (horizontalInputVec > 0)
                {
                    faceRight = true;
                    faceDir = faceRight ? 1 : -1;
                }
                else if (horizontalInputVec < 0)
                {
                    faceRight = false;
                    faceDir = faceRight ? 1 : -1;
                }
                transform.localScale = new Vector3(faceDir, 1, 1);

            }
            else
            {
                //Debug.Log("����ת��");
            }
        }
        else
        {
            if (canTurnAround && needTurnAround)
            {
                Debug.Log("ת��");
                faceRight = !faceRight;
                needTurnAround = false;
            }

        }
    }



    public void JumpBufferCheck()
    {
       Coroutine jumpBuffer=  StartCoroutine(JumpBufferCheckCo(jumpBufferLength));
    }
    private IEnumerator JumpBufferCheckCo(float counter)
    {
        while (counter > 0)
        {
            //Debug.Log("��ʼ��⻺��");
            if (canJump)
            {
                
                    Debug.Log("������Ծ�ɹ�");
                    ChangeToJumpState();
                    break;//����û����return��������Break�������ѭ����
            }
            counter -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //if(counter<=0) Debug.Log("������⻺��");
    }
    





    #endregion

    //private void Crouch()
    //{
    //    //isCrouching = true;
    //    moveRatio = crouchSpeed;
    //    moveSpeedMax = crouchSpeedMax;
    //}
    //private void RiseUp()
    //{
    //    //Debug.Log("����");
    //    isCrouching = false;
    //    moveRatio = wanderSpeed;
    //    moveSpeedMax = runSpeedMax;
    //}



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
    public void HalfYVelocity()
    {
        thisRB.velocity += new Vector2(0, -thisRB.velocity.y / 2);
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


    public void PR_GravityRelatedUpdate()//�����жϿ���Player���ڵ�λ�ã�����PR�ڿ���ʵ�������ĵ����������������д������������
    {
        if (thisPR.IsOnFloored())
        {
            thisPR.isRising = false;
            thisPR.isFalling = false;
            thisPR.isPeak = false;
        }
        else
        {
            if (thisRB.velocity.y > peakSpeed)
            {
                thisPR.isRising = true;
                thisPR.isFalling = false;
                thisPR.isPeak = false;
            }
            else if (thisRB.velocity.y < -peakSpeed)
            {
                thisPR.isRising = false;
                thisPR.isFalling = true;
                thisPR.isPeak = false;
            }
            else
            {
                thisPR.isRising = false;
                thisPR.isFalling = false;
                thisPR.isPeak = true;
            }
        }
    }


    #endregion
    #region �����Է���
    private void GameplayCooldownCount()
    {

        if (uncontrolCounter >= 0)
        {
            uncontrolCounter -= Time.deltaTime;
        }
        else
        {
            isUncontrol = false;
        }

        if (attackCooldownCounter >= 0)
        {
            attackCooldownCounter -= Time.deltaTime;
        }
        else
        {
            canAttack = true;
        }
    }


    private void GameplayCount()
    {
        if (continueAttackCounter > 0&&stateMachine.currentState !=attackState)
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

    public bool isStandOnPlatform()
    {
        if (thisPR.RayHit().collider)
        {
            if (thisPR.RayHit().collider.GetComponent<PlatformController>())
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }



    #endregion


    #region Input��ط���

    private void InputProcess()
    {
        if(stateMachine.currentState == handleState)
        {
            HnadleVecUpdate();
        }
        else
        {
            MovementVecUpdate();
        }
    }

    private void MovementVecUpdate()
    {
        if (isGameplay)
        {
            if (canAct)
            {
                if (thisPR.IsOnFloored())
                {
                    if (theCM.theInput.horizontalInputVec == faceDir && thisPR.IsOnWall())
                    {
                        horizontalInputVec = 0;
                    }
                    else
                    {
                        horizontalInputVec = theInput.horizontalInputVec;
                    }
                }
                else
                {
                    if (theCM.theInput.horizontalInputVec == faceDir && (thisPR.IsOnWall() || thisPR.IsForwad()))
                    {
                        horizontalInputVec = 0;
                    }
                    else
                    {
                        horizontalInputVec = theInput.horizontalInputVec;
                    }
                }
                verticalInputVec = theInput.verticalInputVec;
            }
        }
    }
    private void HnadleVecUpdate()
    {
        if (isGameplay) 
        {
            if (canAct)
            {
            horizontalInputVec = theInput.horizontalInputVec;
            verticalInputVec = theInput.verticalInputVec;

            }
        }
    }

    #endregion

}

