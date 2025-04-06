using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using InteractiveAndInteractableEnums;

public class LevelerController : MonoBehaviour
{
    #region ���

    private Animator thisAnim;
    private CombineInteractableManager theCombineManager;
    #endregion



    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĲ��ݸ����ͣ���thisLevelerType")]
    [Space(3)]
    #region ����
    [Header("Leveler Setting")]
    public levelerType thisLevelerType;
    [Header("Leveler Info")]
    public bool isInteracted;
    public bool canBeInteracted;

    [Header("Rotater Related")]//�漰����Ϊ������תƽ̨������˳ʱ����ת����ʱ����ת��ֹͣ����״̬
    [Header("2�������תƽ̨���󣬼�rotatePlatforms\n3����Ӹ�λʱ��")]
    public PlatformController[] rotatePlatforms;
    private float canBeInteractedCounter;
    public float canBeInteractedDuration;


    [Header("Elevator Related")]//�漰����Ϊ���ݣ������������½���ֹͣ����״̬
    [Header("2����ӵ��ݶ���elevatorPlatform\n3������������Ϊ���ݵ��Ӷ���")]
    public PlatformController elevatorPlatform;



    [Header("Animator Related")]
    private const string ISINTERACTINGSTR = "isInteracting";
    private const string ISALTINTERACTINGSTR = "isAltInteracting";
    private const string CANBEINTERACTED = "canBeInteracted";
    #endregion


    #region ��ʼ�����
    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//��ȡ������Ҫ��ʵ�ֶ����֮���ͬ��
    }
    private void Start()//Leveler��ʼֻ�ڳ�ʼ״̬�����ý����κγ�ʼ��
    {

    }



    #endregion

    #region Update���
    private void Update()//Leveler��ʹ�����˶���ܿ츴λ����������������Ӧ����������ű�������
    {
        if (!canBeInteracted)
        {
            if (canBeInteractedCounter > 0)
            {
                canBeInteractedCounter -= Time.deltaTime;
            }
            else
            {
                canBeInteracted = true;
                thisAnim.SetBool(CANBEINTERACTED, true);
            }
        }
        else
        {
            //Debug.Log("�����ȴ�");
        }
    }

    #endregion

    #region Interact���
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackArea>())
        {
            switch (thisLevelerType) 
            {
                case levelerType.attackable_rotater:
                    Attackable_RotaterInteract(other.GetComponent<AttackArea>());//���ݹ����ķ���������ת����
                    break;
                case levelerType.attackable_elevator:
                    Attackable_ElevatorInteract(other.GetComponent<AttackArea>());//���ݹ����ķ�����������
                    break;
            }

        }
    }

    private void Attackable_RotaterInteract(AttackArea _theAttack)
    {
        if (canBeInteracted)
        {
            if (_theAttack.thePlayer.transform.position.x > transform.position.x)
            {
                //Debug.Log("����һ��������");
                ClockwiseRotate();
            }
            else
            {
                //Debug.Log("�����������ز�");
                AntiClockwiseRotate();
            }
        }
    }
    private void Attackable_ElevatorInteract(AttackArea _theAttack)
    {
        if (canBeInteracted)
        {
            if (_theAttack.thePlayer.transform.position.x > transform.position.x)
            {
                //Debug.Log("����һ��������");
                ElevatorUpwardMove();
            }
            else
            {
                //Debug.Log("�����������ز�");
                ElevaterDownwardMove();
            }
        }
    }

    #endregion


    #region С�������ⲿ����
    private void ClockwiseRotate()
    {
        foreach(PlatformController rotatePlatform in rotatePlatforms)
        {
            rotatePlatform.ClockwiseRotate();
        }
        theCombineManager.LevelersInteract();
    }
    private void AntiClockwiseRotate()
    {
        foreach (PlatformController rotatePlatform in rotatePlatforms)
        {
            rotatePlatform.AntiClockwiseRotate();
        }
        theCombineManager.LevelersAltInteract();
    }
    private void ElevatorUpwardMove()
    {
        if(Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theEndPoint.position) < .01F)
        {
            //Debug.Log("��������");
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theRotatePovit_ElevatorPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theEndPoint.position;
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theStartPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theRotatePovit_ElevatorPoint.position;
        }
    }
    private void ElevaterDownwardMove()
    {
        if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theEndPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theRotatePovit_ElevatorPoint.position;
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theRotatePovit_ElevatorPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theStartPoint.position;
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theStartPoint.position) < .01F)
        {
            Debug.Log("��������");
        }
    }
    public void JustInteract()
    {
        thisAnim.SetTrigger(ISINTERACTINGSTR);
        thisAnim.SetBool(CANBEINTERACTED, false);
        canBeInteracted = false;
        canBeInteractedCounter = canBeInteractedDuration;
    }

    public void JustAltInteract()
    {
        thisAnim.SetBool(CANBEINTERACTED, false);
        thisAnim.SetTrigger(ISALTINTERACTINGSTR);
        canBeInteracted = false;
        canBeInteractedCounter = canBeInteractedDuration;
    }
    #endregion
}
