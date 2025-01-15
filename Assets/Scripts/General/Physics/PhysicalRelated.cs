using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRelated : MonoBehaviour
{
    private Rigidbody2D thisRB;
    private Collider2D thisCol;

    [Header("PhysicsCheck")]
    private bool isGround;
    public bool wasGround;
    public Transform theGroundCheckpoint;
    public bool isWall;
    public bool wasWall;
    public Transform theWallCheckpoint;
    public bool isForward;
    public Transform theForwardCheckpoint;
    public bool isBackWall;
    public Transform theBackWallCheckpoint;
    public LayerMask theGround;
    public float theGroundCheckRadius;
    public float theWallCheckRadius;
    public float theForwardCheckRadius;
    public float theBackWallCheckRadius;

    [Header("PhysicsChange-")]
    public PhysicsMaterial2D normalMat;
    public PhysicsMaterial2D slipperMat;
    public bool isPeak;//new
    public bool isHolding;
    public bool gravityLock;
    public bool needGroundChange;
    public bool needWallChange;
    public bool needAirChange;
    [Header("PhysicsChange Detail")]
    public float normalGravity = 2;
    public float peakGravity = 3f;
    public float riseGravity = 4;
    public float fallGravity = 4.5f;
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



    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
    }

    public void PRUpdate()//Ϊ�˽����ͬ�ű���UPdate�Ⱥ����⣬������������PC���ý���Update
    {
        GroundCheck();
        WallCheck();
        ForwardCheck();
        BackWallCheck();
        MaterialAndGravityUpdate();
    }

    private void GroundCheck()//����ʵ�ֵ����ж��ʹ���������������ǽ��ض����ڵ�ǰ״̬��Update�������£��Ҳ���Ψһ������������ifNeed�ж��ڴ����ò���
    {
        WhetherHadGround();//�趨���ʱ�䣬�������������˲��ĵ���ͼ���������
        isGround = hasGround && Physics2D.OverlapCircle(theGroundCheckpoint.position, theGroundCheckRadius, theGround);//ͨ��ͼ���⼰ǰ�����ʱ���жϱ�֡�Ĵ������


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
                if (isPeak) thisRB.gravityScale = peakGravity;
                else
                {
                    if (thisRB.velocity.y > 0) thisRB.gravityScale = riseGravity;
                    else thisRB.gravityScale = fallGravity;
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

    #endregion



    #region �ⲿ����

    public bool IsOnGround()
    {
        return isGround;
    }


    public void LeaveGround()
    {
        hasGround = false;
        hasGroundCounter = hasGroundLength;
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
    }
}
