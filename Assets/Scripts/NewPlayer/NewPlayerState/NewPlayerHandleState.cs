using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerHandleState : NewPlayerState, IMove_horizontally
{
    public NewPlayerHandleState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        CurrentStateCandoChange();
        HandleEnter();
    }

    public override void Exit()
    {
        base.Exit();
        HandleExit();
    }


    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        player.theHandle.HandlerUpdate();
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
        player.canTurnAround = false;
        player.canHorizontalMove = false;
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


    private void HandleEnter()
    {
        player.canTurnAround = false;
        player.thisBoxCol.enabled = true;
    }
    public void HorizontalMove()
    {
        if (player.isUncontrol)//�����ƶ�ʱ����ƶ�
        {
            //
        }
        else
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
    private void WhetherExit()
    {
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveGround();
            player.ChangeToFallState();
            return;
        }

    }

    private void HandleExit()
    {
        player.canTurnAround = true;
        player.theHandle.ClearInput();
    }

}
