using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CMRelatedEnum;

public class PhysicsRelated : MonoBehaviour
{
    #region ���������
    private Rigidbody2D thisRB;
    private Collider2D thisCol;
    public PhysicsConfig physicsConfig;
    #endregion
    [Header("Physical State")]
    public PlayerPhysicalState nowGrivatyState;

    [Header("Physics Setting")]
    public Transform theGroundCheckpoint;
    public Transform theWallCheckpoint;
    public Transform theForwardCheckpoint;
    public Transform theBackWallCheckpoint;
    public Transform theHeadCheckpoint;
    [Tooltip("ͨ������")]
    public LayerMask theCeil;
    [Tooltip("������Ҫ�϶�Ϊ���Ա�վ����ƽ̨���߶���Ӧ�ð�������")]
    public LayerMask theFloor;
    [Tooltip("ͨ�����棿")]
    public LayerMask theWall;
    //public LayerMask theTrueGround;

    [Header("PhysicsCheck")]
    [SerializeField] private bool isRaycastGround;
    private bool wasRaycastGround;
    [SerializeField] private bool isWall;
    private bool wasWall;
    [SerializeField] private bool isForward;
    [SerializeField] private bool isBackWall;
    private bool wasBackWall;
    [SerializeField]private bool isHead;
    private bool wasHead;
    private RaycastHit2D theRaycastCol;




    [Header("PhysicsChange")]
    public PhysicsMaterial2D normalMat;
    public PhysicsMaterial2D slipperMat;
    public bool gravityLock;
    //public bool needGroundChange;
    //public bool needWallChange;
    //public bool needAirChange;
    [Header("PhysicsChange Detail")]
    private bool riseGravityed;
    private bool fallGravityed;


    [Header("Other")]
    public bool hasWall;
    public float hasWallCounter;
    private bool hasForward;
    public float hasForwardCounter;
    public bool hasRaycastGround;
    public float hasRaycastGroundCounter;



    private void Awake()
    {
        /*
         * Work1.�����ȡ
         * 
         * 
         */
        thisRB = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
    }
    public void PRUpdata()
    {
        /* ������������������⼰����������ݣ������ڽű���ͨ֡
         * 
         * TODO������һ�׿��Է����ڲ�ͬ����Controller�ϵ�PR�ű�
         * 
         * Step1.��������������
         * Step2.���������⼰�˶�״̬�ж���ǰ������״̬
         * Step3.��������״̬
         * 
         */
        PhysicsRelated_Check();
        PhysicsRelated_StateJudgement();
        PhysicsRelated_ParaUpdata();
    }


    //private void GroundedCheck()
    //{
    //    WhetherHadGrounded();//�趨���ʱ�䣬�������������˲��ĵ���ͼ���������
    //    wasGrounded = isGrounded;
    //    isGrounded = hasGrounded && Physics2D.OverlapCircle(theGroundCheckpoint.position, physicsConfig.theGroundCheckRadius, theTrueGround);//ͨ��ͼ���⼰ǰ�����ʱ���жϱ�֡�Ĵ������
    //}

    #region �����ط���
    private void RaycastGroundCheck()
    {
        /* ���������������ߵ����⣬�����ڽű�֡����
         * 
         * Step1.ͨ����ʱ����������ʱ��ⷽ���ڷ�Χ����Ȼ�ܹ���⵽�ķ������ڵ�����
         * Step2.�ж��Ƿ�Ӧ�ý������߼��
         *  a.���ǣ������߷ֱ��⣬����������Ϊ������ȼ�����ȡ�Ƿ��⵽���棬�Լ���ȡ��������ײ��
         *  b.���񣬲����м�⣬ֱ���趨Ϊ��δ��⵽���棬ͬʱ��������ײ���ÿ�
         */

        WhetherHadRaycastGround();

        wasRaycastGround = isRaycastGround;
        if (hasRaycastGround)
        {
            Vector2 centerPos = new Vector2(
                theGroundCheckpoint.position.x + physicsConfig.raysCheckStep,
                theGroundCheckpoint.position.y
            );
            Vector2 leftPos = new Vector2(
                theGroundCheckpoint.position.x - physicsConfig.raysCheckStep,
                theGroundCheckpoint.position.y
            );
            Vector2 rightPos = new Vector2(
                theGroundCheckpoint.position.x + physicsConfig.raysCheckStep,
                theGroundCheckpoint.position.y
            );
            RaycastHit2D theBodyRaycastCheckCol = Physics2D.Raycast(centerPos, Vector2.down, physicsConfig.raycastLength, theFloor);
            RaycastHit2D theLeftRaycastCheckCol = Physics2D.Raycast(leftPos, Vector2.down, physicsConfig.raycastLength, theFloor);
            RaycastHit2D theRightRaycastCheckCol = Physics2D.Raycast(rightPos, Vector2.down, physicsConfig.raycastLength, theFloor);

            if (theBodyRaycastCheckCol.collider!=null){
                isRaycastGround = true;
                theRaycastCol = theBodyRaycastCheckCol;
            }
            else if(theLeftRaycastCheckCol.collider != null)
            {
                isRaycastGround = true;
                theRaycastCol = theLeftRaycastCheckCol;
            }
            else if (theRightRaycastCheckCol.collider != null)
            {
                isRaycastGround = true;
                theRaycastCol = theRightRaycastCheckCol;
            }
            else
            {
                isRaycastGround = false;
                theRaycastCol = new RaycastHit2D();
            }

        }
        else
        {
            isRaycastGround = false;
            theRaycastCol = new RaycastHit2D();
        }
    }

    private void WallCheck()//����ʵ��ǽ���ж��ʹ���������������ǽ��ض����ڵ�ǰ״̬��Update�������£��Ҳ���Ψһ������������ifNeed�ж��ڴ����ò���
    {
        /*
         * ����������ǰ��ǽ�ڼ�⣬�����ڽű�֡����
         * 
         * Step1.ͨ����ʱ��Э���������ǽ��ʱ��ⷽ�����ڵ�����
         * Step2.����������
         * �ڶ�����ʵ��ͬ��if(hasWall)isWall = Physics2D.OverlapCircle(theWallCheckpoint.position, physicsConfig.theWallCheckRadius, theWall);else isWall = false;
         */
        WhetherHadWall();
        wasWall = isWall;
        isWall = hasWall && Physics2D.OverlapCircle(theWallCheckpoint.position, physicsConfig.theWallCheckRadius, theWall);
        

    }

    private void ForwardCheck()
    {
        // ����������ǰ�������⣬�����ڽű�֡����,����Ҫ���ڶ�����֧/�������������߼����ã��ʲ��ÿ������ʱ����֡�ȶԵ�����

        isForward = Physics2D.OverlapCircle(theForwardCheckpoint.position, physicsConfig.theForwardCheckRadius, theFloor);
    }

    private void BackWallCheck()
    {
        //���������ڱ���ʵ���⣬�����ڽű�֡���У�����Ҫ���ڼ�ѹ״̬�жϣ�������Ҫ������֡�ȶ�����
        wasBackWall = isBackWall;
        isBackWall = Physics2D.OverlapCircle(theBackWallCheckpoint.position, physicsConfig.theBackWallCheckRadius, theFloor);
    }
    private void HeadCheck()
    {
        //����������ͷ��ʵ���⣬�����ڽű�֡����
        wasHead = isHead;
        isHead = Physics2D.OverlapCircle(theHeadCheckpoint.position, physicsConfig.theHeadCheckRadius, theCeil);
    }
    private void WhetherHadWall()
    {
        if (!hasWall)
        {
            if (hasWallCounter > 0f) hasWallCounter -= Time.deltaTime;
            else
            {
                hasWall = true;
            }
        }
    }
    private void WhetherHadForward()
    {
        if (!hasForward)
        {
            if (hasForwardCounter > 0f) hasForwardCounter -= Time.deltaTime;
            else
            {
                hasForward = true;
            }
        }
    }
    private void WhetherHadRaycastGround()
    {
        if (!hasRaycastGround)
        {
            if (hasRaycastGroundCounter > 0f) hasRaycastGroundCounter -= Time.deltaTime;
            else
            {
                hasRaycastGround = true;
            }
        }
    }
 
    //private void WhetherHadGrounded()
    //{
    //    if (!hasGrounded)
    //    {
    //        if (hasGroundedCounter > 0f) hasGroundedCounter -= Time.deltaTime;
    //        else
    //        {
    //            hasGrounded = true;
    //        }
    //    }
    //}
    #endregion

    #region ����������·���
    private void PhysicsRelated_Check()
    {
        //��������������������ĸ���
        //GroundedCheck();
        RaycastGroundCheck();
        WallCheck();
        ForwardCheck();
        BackWallCheck();
        HeadCheck();
        PhysicsRelated_StateJudgement();
        PhysicsRelated_ParaUpdata();
    }

    private void PhysicsRelated_StateJudgement()
    {
        //���������ڵ�ǰ����״̬���ж�
        if (isRaycastGround)
        {
            nowGrivatyState = PlayerPhysicalState.isStanding;
        }
        else
        {
            if (thisRB.velocity.y > physicsConfig.peakSpeed)
            {
                nowGrivatyState = PlayerPhysicalState.isRising;
            }
            else if (thisRB.velocity.y < -physicsConfig.peakSpeed)
            {
                nowGrivatyState = PlayerPhysicalState.isFalling;
            }
            else
            {
                nowGrivatyState = PlayerPhysicalState.isPeak;
            }
        }

    }

    private void PhysicsRelated_ParaUpdata()
    {
        //������������������ĸ���
        switch (nowGrivatyState)
        {
            case PlayerPhysicalState.isRising:
                thisCol.sharedMaterial = slipperMat;
                if (!riseGravityed)
                {
                    thisRB.gravityScale = physicsConfig.riseGravity;
                    riseGravityed = true;
                }
                else
                {
                    if(thisRB.gravityScale< physicsConfig.fallMaxGravity)
                    {
                        thisRB.gravityScale += thisRB.gravityScale * physicsConfig.risegravityMultiplier;

                    }
                    else
                    {
                        thisRB.gravityScale = physicsConfig.fallMaxGravity;
                    }
                }
                thisRB.drag = physicsConfig.airDrag;
                break;
            case PlayerPhysicalState.isFalling:
                thisCol.sharedMaterial = slipperMat;
                if (!fallGravityed)
                {
                    thisRB.gravityScale = physicsConfig.fallGravity;
                    fallGravityed = true;
                }
                else
                {
                    if (thisRB.gravityScale < physicsConfig.fallMaxGravity)
                    {
                        thisRB.gravityScale += thisRB.gravityScale * physicsConfig.fallGravityMultiplier;

                    }
                    else
                    {
                        thisRB.gravityScale = physicsConfig.fallMaxGravity;
                    }
                }
                thisRB.drag = physicsConfig.airDrag;
                break;
            case PlayerPhysicalState.isPeak:
                thisCol.sharedMaterial = slipperMat;
                thisRB.gravityScale = physicsConfig.peakGravity;
                thisRB.drag = physicsConfig.airDrag;
                break;
            case PlayerPhysicalState.isStanding:
                thisCol.sharedMaterial = normalMat;
                if (!gravityLock) thisRB.gravityScale = physicsConfig.normalGravity;
                thisRB.drag = physicsConfig.normalDrag;
                break;
        }   
    }

    #endregion

    #region �ⲿ����

    public bool IsOnFloored()
    {
        //return isFloored;
        return isRaycastGround;
    }
    public bool WasOnFloored()
    {
        //return wasFloored;
        return wasRaycastGround;
    }
    public bool IsOnWall()
    {
        return isWall;
    }
    public bool IsForwad()
    {
        return isForward;
    }
    public bool IsHead()
    {
        return isHead;
    }

    //public void LeaveGround()
    //{

    //    hasGrounded = false;
    //    hasGroundedCounter = physicsConfig.hasGroundDuration;
    //    riseGravityed = false;
    //    fallGravityed = false;

    //    hasRaycastGround = false;
    //    hasRaycastGroundCounter = physicsConfig.hasRaycastGoundDuration;
    //}

    public void LeaveFloor()
    {
        //hasFloored = false;
        //hasFlooredCounter = physicsConfig.hasFlooredDuration;
        riseGravityed = false;
        fallGravityed = false;
        hasRaycastGround = false;
        hasRaycastGroundCounter = physicsConfig.hasRaycastGoundDuration;
    }

    public void LeaveWall()
    {
        hasWall = false;
        hasWallCounter = physicsConfig.hasWallDuration;
    }
    public void GravityLock(float gravityScale)
    {
        //Debug.Log("����");
        thisRB.gravityScale = gravityScale;
        gravityLock = true;
    }
    public void GravityUnlock()
    {

        gravityLock = false;
    }

    public RaycastHit2D RayHit()
    {
        return theRaycastCol;
    }


    #endregion



    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(theGroundCheckpoint.position, physicsConfig.theGroundCheckRadius);
        Gizmos.DrawWireSphere(theWallCheckpoint.position, physicsConfig.theWallCheckRadius);
        Gizmos.DrawWireSphere(theForwardCheckpoint.position, physicsConfig.theForwardCheckRadius);
        Gizmos.DrawWireSphere(theBackWallCheckpoint.position, physicsConfig.theBackWallCheckRadius);
        Gizmos.DrawWireSphere(theHeadCheckpoint.position, physicsConfig.theHeadCheckRadius);
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x + physicsConfig.raysCheckStep, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x + physicsConfig.raysCheckStep, theGroundCheckpoint.position.y - physicsConfig.raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x - physicsConfig.raysCheckStep, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x - physicsConfig.raysCheckStep, theGroundCheckpoint.position.y - physicsConfig.raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y - physicsConfig.raycastLength));
    }
}
