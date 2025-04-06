using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallJumpState : PlayerState//TD����Ҫ��Air״̬���кϲ�����WallJump���������ж����������ȥ
{
    public PlayerWallJumpState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.thisAC.FlipX();
        WallJump();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
        WallJumpdEnd();
    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        WallJumpCount();
        Move();
        WhetherExit();

    }
    private void Move()
    {
        if (player.isGameplay)
        {
            if (!player.isUncontrol)
            {
                if (Mathf.Abs(player.thisRB.velocity.x + player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime) < player.horizontalMoveSpeedMax)//�ڿ��ǵ�������У��÷�������һ��Ч����ͬ
                {
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                    {
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * (player.horizontalmoveThresholdSpeed + player.horizontalMoveSpeed * Time.deltaTime), 0f);
                    }
                    else
                    {
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime, 0f);
                    }
                }
                else
                {
                    int Temp = (player.horizontalInputVec != 0) ? ((player.horizontalInputVec == player.faceDir) ? 1 : -1) : 0;
                    if (Temp < 0)
                    {
                        player.thisRB.velocity += new Vector2(player.horizontalMoveSpeed * Temp * Time.deltaTime, 0f);
                        //Debug.Log("����״̬�¼���");
                    }
                    else
                    {
                        //Debug.Log("�����ټ���");
                    }
                }
            }
            else
            {
                if (player.canHorizontalMove)
                {
                    if (Mathf.Abs(player.thisRB.velocity.x + player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime) < player.horizontalMoveSpeedMax)//�ڿ��ǵ�������У��÷�������һ��Ч����ͬ
                    {
                        switch (player.horizontalInputVec)
                        {
                            case 0:
                                if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                                {
                                    player.ClearXVelocity();
                                }
                                break;
                            case 1:
                                if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.horizontalInputVec != player.faceDir)
                                {
                                    player.thisRB.velocity += new Vector2(player.horizontalInputVec * (player.horizontalmoveThresholdSpeed + player.horizontalMoveSpeed * Time.deltaTime), 0f);
                                }
                                else
                                    player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime, 0f);
                                break;
                            case -1:
                                if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.horizontalInputVec != player.faceDir)
                                {
                                    player.thisRB.velocity += new Vector2(player.horizontalInputVec * (player.horizontalmoveThresholdSpeed + player.horizontalMoveSpeed * Time.deltaTime), 0f);
                                }
                                else
                                    player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeed * Time.deltaTime, 0f);
                                break;
                            default:
                                Debug.Log("��Ӧ�ó����������");
                                break;
                        }
                    }
                    else
                    {
                        Debug.Log("�����ټ���");
                    }
                }
                else
                {
                    Debug.Log("ǿ�Ƶ�ǽ�ڼ�");
                }
            }
        }
    }
    protected override void CurrentStateCandoChange()
    {
        base.CurrentStateCandoChange();
        player.canHorizontalMove = false;
        player.canVerticalMove = false;
        //����Ծ�������߼�����������������������Ծ�ģ����Բ���ˢ�²�����ص���̨��ʱ��
        //�����п��ܳԵ�һЩ���ߣ�ʹ��ʵ�ֿ���������Ծ����������Ҫ�ж��Ƿ�����Ծ
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanDash();
        player.WhetherCanHold();
        player.canWallFall = false;
        player.canAttack = true;

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanHold();
        player.WhetherCanWallFall();
        player.WhetherCanDash();

    }
    public void WallJump()
    {
        player.wallJumpPostCounter = player.wallJumpPostLength;
        player.thisPR.GravityLock(2f);
        player.thisRB.AddForce(new Vector2(-player.faceDir, 2) * player.wallJumpForce, ForceMode2D.Impulse);
        player.needTurnAround = true;
        player.faceRight = !player.faceRight;
        player.thisPR.LeaveWall();
    }
    public void WallJumpdEnd()
    {
        player.thisPR.GravityUnlock();
        player.canHorizontalMove = true;
        player.WhetherCanHold();
    }
    private void WallJumpCount()
    {
        if (player.wallJumpPostCounter > 0) player.wallJumpPostCounter -= Time.deltaTime;
        else
        {
            WallJumpdEnd();
        }

    }

    private void WhetherExit()
    {

        if (player.thisRB.velocity.y < -player.peakSpeed)
        {
            stateMachine.ChangeState(player.airState);
            return;
        }
        else if (player.thisPR.IsOnFloored())
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        else
        {
            player.WhetherHoldOrWallFall();
        }
    }
}
