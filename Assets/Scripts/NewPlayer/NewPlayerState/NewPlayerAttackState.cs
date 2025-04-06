using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerAttackState : NewPlayerState,IMove_horizontally, IFall_vertically
{
    public NewPlayerAttackState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        CurrentStateCandoChange();
        AttackEnter();

    }

    public override void Exit()
    {
        base.Exit();
        AttackExit();
    }

    public override void Update()
    {
        base.Update();
        //KeepInertiaCount();//��ע�͵������Ը�
        CurrentStateCandoUpdate();
        Fall();
        BoxColUpdate();
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
        //����Ծ�������߼�����������������������Ծ�ģ����Բ���ˢ�²�����ص���̨��ʱ��
        //�����п��ܳԵ�һЩ���ߣ�ʹ��ʵ�ֿ���������Ծ����������Ҫ�ж��Ƿ�����Ծ
        player.WhetherCanJumpOrWallJump();

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }



    private void AttackEnter()
    {
        //Debug.Log("�水��");
        player.canAttack = false;

    }


    private void WhetherExit()
    {
        if (player.thisAC.isAttackingPlaying() && player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).normalizedTime%1 >= .9f)
        {
            //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorClipInfo(1)[0].clip.name);
            player.StateOver();
        }
        else
        {
            //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1);
            //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).shortNameHash);
        }
    }
    private void AttackExit()
    {
        if (player.attackCounter == 1)
        {
            player.continueAttackCounter = player.continueAttackDuration;
        }
        else
        {
            player.continueAttackCounter = 0;
        }
        player.attackCooldownCounter = player.attackCooldownDuration;
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

    public void BoxColUpdate()
    {
        if (player.thisRB.velocity.y < 0)
        {

            player.thisBoxCol.enabled = true;
        }
        else
        {
            player.thisBoxCol.enabled = false;

        }
    }

    public void Fall()
    {
        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
        }
        else
        {
            //Debug.Log("??");
        }
    }

}
