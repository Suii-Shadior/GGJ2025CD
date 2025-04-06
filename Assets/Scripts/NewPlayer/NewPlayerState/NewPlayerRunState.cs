using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class NewPlayerRunState : NewPlayerState, IMove_horizontally
{
    public NewPlayerRunState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        CurrentStateCandoChange();
        MoveEnter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        //KeepInertiaCount();
        CurrentStateCandoUpdate();
        WhetherExit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.canAttack = true;
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }
    private void MoveEnter()
    {
        //if (player.keepInertia)
        //{
        //    player.thisPR.GravityLock(player.thisPR.peakGravity);
        //}
        player.thisBoxCol.enabled = true;

        player.horizontalMoveSpeedAccleration = player.normalmoveAccleration;
        //player.horizontalMoveSpeed = player.normalmoveSpeed;
        player.horizontalMoveSpeedMax = player.normalmoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.normalmoveThresholdSpeed;
        HorizontalMove();
    }



    private void WhetherExit()
    {
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveGround();
            player.ChangeToFallState();
            return;
        }
        else if (player.horizontalInputVec == 0)
        {
            //Debug.Log("ͣ��");
            player.ChangeToIdleState();
            return;
        }
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
