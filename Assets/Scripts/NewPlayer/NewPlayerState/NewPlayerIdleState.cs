using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerIdleState : NewPlayerState, IMove_horizontally
{
    public NewPlayerIdleState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        IdleEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        player.canCooldown = true;
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        HorizontalMove();
        WhetherExit();


    }

    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();
    }

    private void IdleEnter()
    {

        player.horizontalMoveSpeedAccleration = player.normalmoveAccleration; 
        player.horizontalMoveSpeedMax = player.normalmoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.normalmoveThresholdSpeed;
    }

    private void WhetherExit()
    {
        if (!player.thisPR.IsOnGround())
        {
            player.thisPR.LeaveGround();
            player.ChangeToFallState();
            return;
        }
        else
        if (player.horizontalInputVec != 0)
        {
            player.ChangeToHorizontalMoving();
            return;
        }
    }



    public void HorizontalMove()
    {
        if (player.isUncontrol)//�����ƶ�ʱ����ƶ�
        {
            //
        }
        else
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
