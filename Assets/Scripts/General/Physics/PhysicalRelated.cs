using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsRelated : MonoBehaviour
{
    private Rigidbody2D thisRB;
    private Collider2D thisCol;

    [Header("PhysicsCheck")]
    private bool isGround;
    public bool wasGround;
    public Transform theGroundCheckpoint;
    private bool isWall;
    public bool wasWall;
    public Transform theWallCheckpoint;
    private bool isForward;
    public Transform theForwardCheckpoint;
    public bool isBackWall;
    public Transform theBackWallCheckpoint;
    [SerializeField]private bool isHead;
    public Transform theHeadCheckpoint;

    [Header("3RaycastGroundCheck")]
    public bool isRaycastGround;
    public bool wasRaycastGround;
    public RaycastHit2D theRaycastCol;
    public float footCheckPointOffset;
    public float raycastLength;

    public LayerMask theGround;
    public float theGroundCheckRadius;
    public float theWallCheckRadius;
    public float theForwardCheckRadius;
    public float theBackWallCheckRadius;
    public float theHeadCheckRadius;

    [Header("PhysicsChange-")]
    public PhysicsMaterial2D normalMat;
    public PhysicsMaterial2D slipperMat;
    public bool isPeak;//new
    public bool isHolding;
    public bool isRising;
    public bool isFalling;
    public bool gravityLock;
    public bool needGroundChange;
    public bool needWallChange;
    public bool needAirChange;
    [Header("PhysicsChange Detail")]
    private bool riseGravityed;
    private bool fallGravityed;
    public float normalGravity = 2;
    public float peakGravity = 3f;
    public float riseGravity = 4;
    public float risegravityMultiplier = 1.02f;
    public float fallGravity = 4.5f;
    public float fallGravityMultiplier = 1.2f;
    public float normalDrag = 0;
    public float airDrag = 1;

    [Header("Other")]
    //public bool hasDisGround;
    //public float hasDisGroundCounter;
    //public float hasDisGroundLength;
    public bool hasGround;
    public float hasGroundCounter;
    public float hasGroundLength = .1f;
    //public bool hasDisWall;
    //public float hasDisWallCounter;
    //public float hasDisWallLength;
    public bool hasWall;
    public float hasWallCounter;
    public float hasWallLength = .1f;
    private bool hasForward;
    public float hasForwardCounter;
    public float hasForwardLength = .1f;

    public bool hasRaycastGround;
    public float hasRaycastGroundCounter;
    public float hasRaycastGoundLength = .1f;


    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
    }

    public void PRUpdate()//Ϊ�˽����ͬ�ű���UPdate�Ⱥ����⣬������������PC���ý���Update
    {
        GroundCheck();
        RaycastGroundCheck();
        WallCheck();
        ForwardCheck();
        BackWallCheck();
        HeadCheck();
    }
    public void PRFixUpdate()
    {
        MaterialAndGravityUpdate();
    }

    private void GroundCheck()//����ʵ�ֵ����ж��ʹ���������������ǽ��ض����ڵ�ǰ״̬��Update�������£��Ҳ���Ψһ������������ifNeed�ж��ڴ����ò���
    {
        WhetherHadGround();//�趨���ʱ�䣬�������������˲��ĵ���ͼ���������
        isGround = hasGround && Physics2D.OverlapCircle(theGroundCheckpoint.position, theGroundCheckRadius, theGround);//ͨ��ͼ���⼰ǰ�����ʱ���жϱ�֡�Ĵ������


    }

    private void RaycastGroundCheck()
    {
        WhetherHadRaycastGround();
        RaycastHit2D theBodyRaycastCheckCol = Physics2D.Raycast(new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y), Vector2.down, raycastLength, theGround);
        RaycastHit2D theLeftRaycastCheckCol = Physics2D.Raycast(new Vector2(theGroundCheckpoint.position.x - footCheckPointOffset, theGroundCheckpoint.position.y), Vector2.down, raycastLength, theGround);
        RaycastHit2D theRightRaycastCheckCol = Physics2D.Raycast(new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y), Vector2.down, raycastLength, theGround);

        if (theBodyRaycastCheckCol){
            isRaycastGround = true;
            theRaycastCol = theBodyRaycastCheckCol;
        }
        else if(theLeftRaycastCheckCol)
        {
            isRaycastGround = true;
            theRaycastCol = theLeftRaycastCheckCol;
        }
        else if (theRightRaycastCheckCol)
        {
            isRaycastGround = true;
            theRaycastCol = theRightRaycastCheckCol;
        }
        else
        {
            isRaycastGround = false;
            theRaycastCol = theBodyRaycastCheckCol;
        }
        if(hasRaycastGround&& isRaycastGround)
        {
            isRaycastGround = true;
        }
        else
        {
            isRaycastGround = false;
            theRaycastCol = theBodyRaycastCheckCol;
        }
    }

    private void WallCheck()//����ʵ��ǽ���ж��ʹ���������������ǽ��ض����ڵ�ǰ״̬��Update�������£��Ҳ���Ψһ������������ifNeed�ж��ڴ����ò���
    {
        WhetherHadWall();
        isWall = hasWall && Physics2D.OverlapCircle(theWallCheckpoint.position, theWallCheckRadius, theGround);

    }

    private void ForwardCheck()
    {
        //WhetherHadWall();
        isForward = Physics2D.OverlapCircle(theForwardCheckpoint.position, theForwardCheckRadius, theGround);
    }

    private void BackWallCheck()
    {
        isBackWall = Physics2D.OverlapCircle(theBackWallCheckpoint.position, theBackWallCheckRadius, theGround);
    }


    private void HeadCheck()
    {
        isHead = Physics2D.OverlapCircle(theHeadCheckpoint.position, theHeadCheckRadius, theGround);
    }
    private void MaterialAndGravityUpdate()
    {
        if (!isGround)
        {
            if (isHolding)
            {
                HoldingMaterialAndGravityPara();
            }
            else
            {
                AiringMaterialAndGravityPara();
            }
        }
        else
        {
            GroundingMaterialAndGravityPara();
        }
    }

    private void HoldingMaterialAndGravityPara()
    {
        thisCol.sharedMaterial = slipperMat;
        if (!gravityLock) thisRB.gravityScale = 0f;
        thisRB.drag = airDrag;
    }

    private void AiringMaterialAndGravityPara()
    {
        thisCol.sharedMaterial = slipperMat;
        {
            if (!gravityLock)
            {
                if (isPeak)
                {
                    thisRB.gravityScale = peakGravity;
                }
                else if (isRising)
                {
                    if(!riseGravityed)
                    {
                        thisRB.gravityScale = riseGravity;
                        riseGravityed = true;
                    }
                    else
                    {
                        thisRB.gravityScale += thisRB.gravityScale * risegravityMultiplier;
                    }
                }
                else if (isFalling)
                {
                    if(!fallGravityed)
                    {
                        thisRB.gravityScale = fallGravity;
                        fallGravityed = true;
                    }
                    else
                    {
                        thisRB.gravityScale += thisRB.gravityScale * fallGravityMultiplier;
                        //Debug.Log(thisRB.gravityScale);
                    }
                }
                    
                
            }
        }
        thisRB.drag = airDrag;
    }

    private void GroundingMaterialAndGravityPara()
    {
        thisCol.sharedMaterial = normalMat;
        if (!gravityLock) thisRB.gravityScale = normalGravity;
        thisRB.drag = normalDrag;
    }


    #region ���������жϷ���

    private void WhetherHadGround()
    {
        if (!hasGround)
        {
            if (hasGroundCounter > 0f) hasGroundCounter -= Time.deltaTime;
            else
            {
                hasGround = true;
            }
        }
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

    #endregion



    #region �ⲿ����

    public bool IsOnGround()
    {
        return isGround;
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

    public void LeaveGround()
    {
        hasGround = false;
        hasGroundCounter = hasGroundLength;
        riseGravityed = false;
        fallGravityed = false;

        hasRaycastGround = false;
        hasRaycastGroundCounter = hasRaycastGoundLength;
    }

    public void LeaveWall()
    {
        hasWall = false;
        hasWallCounter = hasWallLength;
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

    #endregion



    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(theGroundCheckpoint.position, theGroundCheckRadius);
        Gizmos.DrawWireSphere(theWallCheckpoint.position, theWallCheckRadius);
        Gizmos.DrawWireSphere(theForwardCheckpoint.position, theForwardCheckRadius);
        Gizmos.DrawWireSphere(theBackWallCheckpoint.position, theBackWallCheckRadius);
        Gizmos.DrawWireSphere(theHeadCheckpoint.position, theHeadCheckRadius);
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y - raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x - footCheckPointOffset, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x - footCheckPointOffset, theGroundCheckpoint.position.y - raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y - raycastLength));
    }
}
