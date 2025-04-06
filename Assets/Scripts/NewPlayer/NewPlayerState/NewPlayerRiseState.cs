using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerRiseState : NewPlayerState, IMove_horizontally
{
    public NewPlayerRiseState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Jump();
        RiseEnter();
        CurrentStateCandoChange();
    }
    public void Jump()
    {
        //Time.timeScale = 0.1f;
        player.releaseDuringRising = false;
        player.isPastApexThreshold = false;
        player.holdingCounter = 0f;
        player.CoyoteCounterZero();
        player.ClearYVelocity();
        player.thisRB.AddForce(Vector2.up * player.jumpForce, ForceMode2D.Impulse);
        player.thisPR.LeaveGround();
        //Debug.Log(thisRB.velocity.y);
    }
    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        WhetherExit();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();//�����������X���ƶ��ٶ���Ȼ������ͬ�ļ��ٶ�
    }
    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        //����Ծ�������߼�����������������������Ծ�ģ����Բ���ˢ�²�����ص���̨��ʱ��
        //�����п��ܳԵ�һЩ���ߣ�ʹ��ʵ�ֿ���������Ծ����������Ҫ�ж��Ƿ�����Ծ
        player.WhetherCanJumpOrWallJump();
        player.canAttack = true;

    }

    private void RiseEnter()
    {
        //if (player.keepInertia)
        //{
        //    player.thisPR.GravityLock(player.thisPR.peakGravity);
        //}
        player.thisBoxCol.enabled = false;

        player.horizontalMoveSpeedAccleration = player.airmoveAccleration;
        player.horizontalmoveThresholdSpeed = player.airmoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.airmoveSpeedMax;
        player.verticalFallSpeedMax = player.airFallSpeedMax;

    }

    private void WhetherExit()//
    {
        //if (player.thisPR.IsOnFloored())
        //{
        //    Debug.Log("???");
        //    player.ChangeToIdleState();
        //}
        //else
        if (player.thisPR.IsHead())//�ֿ�����ҪĿ�Ļ��ǿ��ǵ����ܻ��в�ͬ�����������peak״̬��
        {
            player.ClearYVelocity();
            player.ChangeToFallState();
        }
        else if (player.thisRB.velocity.y < 0)
        {

            player.ChangeToFallState();
        } 
        //if (player.thisRB.velocity.y < -player.peakSpeed) //Ҫ���peak״̬ʱ����
        //{
        //    player.ChangeToFallState();  
        //}
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }

    public void HorizontalMove()
    {
        if (player.isGameplay)
        {
            if (player.isUncontrol)//�����ƶ�ʱ����ƶ�
            {
                //
            }
            else//�������ƶ�ʱ���ƶ�
            {
                if (player.horizontalInputVec != 0)//�м�������ʱ
                {
                    if (player.faceDir != player.horizontalInputVec)//���������ﳯ����ʱ��ֱ������ԭ�ȵ��ٶȣ������뷽��ʼ�ƶ�
                    {
                        player.ClearXVelocity();
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                    }
                    else//���������ﳯ��ͬ��ʱ�����ݵ�ǰ�ٶȲ�ͬ�������������١������ƶ�
                    {
                        if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalMoveSpeedMax)
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                            {
                                //��ǰ�ٶ�С�������ٶȣ����������ٶ��ƶ�
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                            }
                            else
                            {
                                //��ǰ�ٶȴ��������ٶ�С�����٣�������ƶ�
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                        }
                        else
                        {
                            //��ǰ�ٶȳ������٣�������ǰ��
                            player.ClearXVelocity();
                            player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedMax, 0f);
                        }
                    }
                }
                else//�޼�������ʱ�����ݵ�ǰ�ٶȲ�ͬ���м��١�ֹͣ
                {
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.thisPR.IsOnWall())
                    {
                        //��ǰ�ٶ�С�ڵ��������ٶȣ���ֹͣ
                        player.ClearXVelocity();
                    }
                    else
                    {
                        //��ǰ�ٶȴ��������ٶȣ������
                        player.thisRB.velocity += new Vector2(-player.faceDir * player.horizontalMoveSpeedAccleration, 0f);
                    }
                }
            }
        }
    }
}
