using AttackInterfaces;
using EnemyEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogeController : MonoBehaviour,IPhysicalAttack
{
    #region ���
    private Animator thisAnim;
    private Collider2D thisCol;
    #endregion


    [Header("Setting")]
    public int hpMax;
    [Header("Infor")]
    public HedgehogState thisHedgehogeState;
    public int hpCur;
    [Header("Physical Check")]
    public Transform theGroundCheckPoint;
    public Transform theForwardCheckPoint;
    public Transform theWallCheckPoint;
    public bool isGrounded;
    public bool isForwarded;
    public bool isWalled;
    public LayerMask theGround;
    public float theGroundCheckRadius;
    [Header("Movement")]
    public float walkSpeed;
    public int faceDir = 1;
    [Header("Dead Related")]
    [Header("Animation Related")]
    private const string WALKSTR = "WALK";
    private const string DEADSTR = "Die";



    private void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisCol = GetComponent<Collider2D>();
    }

    private void Update()
    {
        switch (thisHedgehogeState)
        {
            case HedgehogState.walkstate:
                transform.position += (Vector3)(faceDir*Vector2.right * walkSpeed * Time.deltaTime);//�����泯����walkspeedǰ��
                PhysicsCheck();
                MoveTowardChange();
                break;
            case HedgehogState.deadstate:

                break;
        }
    }

    public void PhysicsCheck()//���ǰ���������ǽ��
    {
        isGrounded = Physics2D.OverlapCircle(theGroundCheckPoint.position, theGroundCheckRadius, theGround);
        isForwarded = Physics2D.OverlapCircle(theForwardCheckPoint.position, theGroundCheckRadius, theGround);
        isWalled = Physics2D.OverlapCircle(theWallCheckPoint.position, theGroundCheckRadius, theGround);
    }
    private void MoveTowardChange()//��ǰ��û�е������ǽ��ʱ��ͷ
    {
        if (!isForwarded || isWalled)
        {
            transform.localScale = new Vector3(-1*transform.localScale.x, .5f, .5f);
            faceDir *= -1;
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackArea>())
        {
            BePhysicalAttacked();
        }
    }
    public void BePhysicalAttacked()
    {
        thisHedgehogeState = HedgehogState.deadstate;
        thisCol.enabled = false;
        //thisAnim.SetBool(WALKSTR,false);
        //thisAnim.SetBool(DEADSTR,true);

    }
    
    
    
    #region С�������ⲿ����
    public void ResetThis()
    {
        thisHedgehogeState = HedgehogState.walkstate;
        thisCol.enabled = true;
        //thisAnim.SetBool(DEADSTR,false);
        //thisAnim.SetBool(WALKSTR,true);
    }
    #endregion
}
