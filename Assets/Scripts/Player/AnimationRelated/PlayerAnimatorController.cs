using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    #region ���
    private SpriteRenderer thisSR;
    private PlayerController player;
    public Animator thisAnim;
    #endregion
    #region ����
    public GameObject currentAttack;
    public GameObject[] bladeAttackOnGoundIdentity;
    public GameObject axeAttackOnGoundIdentity;
    private GameObject lostAttack;
    public Vector2 moveVec2;
    #endregion

    private void Awake()
    {
        thisSR = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<PlayerController>();
        thisAnim = GetComponent<Animator>();

    }
    private void Start()
    {
        thisSR.sortingLayerName = "Player";

    }
    void Update()//��ǰ״̬����û����ҪUpdate������
    {
        thisAnim.SetFloat("velocityY", player.thisRB.velocity.y);
    }



    #region ״̬������
    public void SetVelocityY()
    {
        thisAnim.SetFloat("velocityY", player.thisRB.velocity.y);
    }
    public void DashTrigger()
    {

        thisAnim.SetBool("DashEnd", player.dashEnd);
    }
    public void AttackTrigger()//�����ڹ��������и�����һ���Ķ���
    {
        thisAnim.SetInteger("attackCounter", player.attackCounter);
    }
    public void StopChipPlay()//�������߼���ֹͣ��ǰ�����Ĳ���
    {
        thisAnim.speed = 0f;
    }

    public void ContinueChipPlay()//�������߼��м�����ǰ�����Ĳ���
    {
        thisAnim.speed = 1f;
    }

    public void TBool(string _boolname)//������״̬������˳�ʱ���ж����л�
    {
        thisAnim.SetBool(_boolname, true);
        player.nowState = _boolname;
    }
    public void FBool(string _boolname)
    {
        thisAnim.SetBool(_boolname, false);
    }
    #endregion

    #region �¼�����
    public void PlayerAnimEnd()//�����ڶ�����ʵ��ֱ����ֹ��ǰ״̬
    {
        player.StateOver();
    }
    public void DeadAnimEnd()//������������ʵ���������
    {
        player.PlayerReset();
    }

    #endregion
}
