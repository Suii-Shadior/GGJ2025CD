using PlatformFactoryRelated;
using PlatformInterfaces;
using SubInteractiveEnum;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CompositeCollider2D))]
public class PlatformController : MonoBehaviour
{
    #region ���
    public NewPlayerController thePlayer;
    private CompositeCollider2D thisComCol;
    private Rigidbody2D thisRB;

    #endregion

    #region ����
    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĿ������ͣ���thisPlatformType\n2���趨�ö����Ƿ��ʼ״̬is Hidden\n3����������ͬ�����趨�����鿴��ӦRelated")]
    [Space(3)]


    [Header("Platform Setting")]
    public PlatformInteractiveType thisPlatformType;
    public PlatformFactory thisFactory;
    public bool isHidden;
    public bool canBeHaulted;
    public float haultedDuration;
    [Header("Platform Info")]
    public IPlatform currentPlatform;
    public bool isHaulted;
    public float haultedCounter;
    public bool hasSensored;

    [Header("Units Related")]
    [HideInInspector]public List<PlatformUnit> units = new List<PlatformUnit>();

    [Header("Movable Related")]//���ƶ�ƽ̨����������Singler������Rounder������Sensor�������ظ��ϵ���Elevator
    [Header("4������ƶ��ٶȣ���moveSpeed\n5�������յ�λ�ã���theEndPoint��λ��\n6����sensor����ӵ���ʱ��\n7����elevator����Щ���ݹ���")]
    public float moveSpeed;//elevator����
    public Transform theStartPoint;
    public Transform theEndPoint;
    public Transform theNowPoint;
    public Transform theDestinalPoint;
    public Vector3 offsetVec;//�����ƶ�ʱ����ƫ����

    public bool isDestinationOrienting;
    public bool hasArrived;//elevator,hander����
    public float hasArrivedCounter;
    public float hasArrivedDuration;
    public int handlerInput;
    public bool hasOringinPosed;
    public float perBackStepCounter;
    public float perBackStepDuration;

    [Header("Disappear Related")]//��ʧƽ̨����������Sensor����ʱRegular�������ط��϶���Pair
    [Header("4�������ʧʱ������֣���disappearDuration��reappearDuration")]
    public bool canDisappearOrReappear;//���ڷ�ֹ��Player���µ�ʱ��մ�����ƽ̨��ײ�廹û��ʧ��ʱ���ִ���һ����ʧ
    public bool needToDisappear;
    public bool needToReappear;
    public float disappearCounter;
    public float disappearDuration;
    public float reappearCounter;
    public float reappearDuration;

    [Header("Rotater Related")]//��תƽ̨�������ɿ�rotater
    [Header("4�������תʱ�䣬��\n5��������ת�е�λ�ã���theRotatePovit��λ��\n6����Ϊelevator����Щ���ݹ���")]
    public Transform theRotatePovit_ElevatorPoint;//elevator����
    public float rotationDuration;//��ת�ܼ���Ҫ��ʱ��
    public bool isRotating;
    public float nowAngle;//�ۼƽǶȣ�Ӧ��Ҫ�浵
    public float rotateStep;//�м����
    public float hadRotated;
    public int isClockwise;

    //[Header("Switchable Related")]


    #endregion


    private void Awake()
    {
        thisComCol = GetComponent<CompositeCollider2D>();
        thisRB = GetComponent<Rigidbody2D>();

        GetComponentsInChildren(units);//��������б� 
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.movable_singler://���ƶ�ƽ̨�������ƶ�+����
                thisFactory= new SinglerFactory();
                break;
            case PlatformInteractiveType.moveable_rounder://���ƶ�ƽ̨�������ƶ�
                thisFactory = new RounderFactory();
                break;
            case PlatformInteractiveType.moveable_toucher://���ƶ�ƽ̨�������Ϻ�������Ŀ��ص��ƶ�������뿪һ��ʱ�������
                thisFactory = new ToucherFactory();
                break;
            case PlatformInteractiveType.disappearable_sensor://��ʧƽ̨��Ĭ�ϣ������Ϻ����ʱ����ʧ��һ��ʱ�������
                thisFactory = new SensorFactory();
                break;
            case PlatformInteractiveType.disappearable_regular://��ʧƽ̨��������ʧ
                thisFactory = new RegularorFactory();
                break;
            case PlatformInteractiveType.disappearable_presser:
                thisFactory = new PresserFactory();
                break;
            case PlatformInteractiveType.switchable_elevator://���ƶ�ƽ̨������+�ɿ��ƶ�
                thisFactory = new ElevatorFactory();
                break;
            case PlatformInteractiveType.switchable_pair:
                thisFactory = new PairerFactory();
                break;
            case PlatformInteractiveType.moveable_handler:
                thisFactory = new HandlerFactory();
                break;
            case PlatformInteractiveType.rotateable_rotater://��תƽ̨���ɿ���ת
                thisFactory = new RotaterFactory();
                break;
        }
        currentPlatform = thisFactory?.CreatePlatform(this);
        //units = GetComponentsInChildren<PlatformUnit>();//�����������
    }

    private void Start()//�������Ͳ�ͬ���в�ͬ��ʼ��
    {
       
    }

    private void Update()//�������Ͳ�ͬ���в�ͬ��Update����Ҫ���жϺ��ƶ�
    {
        currentPlatform.SceneExist_Updata();
    }
    private void FixedUpdate()//�������Ͳ�ͬ���в�ͬ��FixUpdate��Ŀǰû��ʹ��
    {
    }






    #region ����С�������ⲿ����


    public void SetPlayer(NewPlayerController _player)
    {
        thePlayer = _player;
    }
    public NewPlayerController GetPlayer()
    {
        return thePlayer;
    }
    public CompositeCollider2D GetComCol()
    {
        return thisComCol;
    }

    public void CallThisPlatform(Transform _destinationPoint)//TODO��Elevatorʹ�ã�Ҫ�Ż�
    {
        theDestinalPoint.position = _destinationPoint.position;
    }

    public void ResetThisPlatform()//�������
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.rotateable_rotater:
                RotatablePlatform_AttackReset();
                break;
        }
    }


    private void RotatablePlatform_AttackReset()
    {
        transform.RotateAround(theRotatePovit_ElevatorPoint.position, Vector3.forward, -nowAngle);
        nowAngle = 0f;
    }





    #endregion

}
