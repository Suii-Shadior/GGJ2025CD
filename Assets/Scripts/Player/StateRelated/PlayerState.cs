public class PlayerState
{
    #region ״̬�����
    protected PlayerStateMachine stateMachine;
    protected PlayerController player;
    #endregion
    #region ���

    #endregion
    #region ����
    protected string animBoolName;
    protected bool stateEnd;
    #endregion

    public PlayerState(PlayerController _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.thisAC.TBool(animBoolName);
        stateEnd = false;
    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {
        player.thisAC.FBool(animBoolName);
    }
    public void CurrentStateEnd()//�����жϿ��ж�״̬����̻򹥻�
    {
        stateEnd = true;
    }
    protected virtual void CurrentStateCandoChange()
    {

    }
    protected virtual void CurrentStateCandoUpdate()
    {

    }
}
