using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerUmbrellaState : NewPlayerState, IMove_horizontally, IFall_vertically
{
    public NewPlayerUmbrellaState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        UmbrellaEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        Fall();
    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();
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
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
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

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        WhetherExit();
    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.WhetherCanJumpOrWallJump();
    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();
    }
    private void UmbrellaEnter()
    {
        player.horizontalMoveSpeedAccleration = player.umbrellaMoveAccelaration;
        player.horizontalmoveThresholdSpeed = player.umbrellaMoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.umbrellaMoveSpeedMax;
        player.verticalFallSpeedMax = player.umbrellaFallSpeedMax;
    }


    public void WhetherExit()
    {
        if (player.thisPR.IsOnFloored())
        {
            player.ChangeToIdleState();
        }

    }
    public void Fall()
    {
        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
            //Debug.Log("����ٶ�:"+player.thisRB.velocity.y);
        }
        else
        {
            //Debug.Log("��������");
        }
    }

}
