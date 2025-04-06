using SubInteractiveEnum;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CompositeCollider2D))]
public class PlatformController : MonoBehaviour
{
    #region ���
    private NewPlayerController thePlayer;
    private CompositeCollider2D thisComCol;
    private Rigidbody2D thisRB;

    #endregion

    #region ����
    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĿ������ͣ���thisPlatformType\n2���趨�ö����Ƿ��ʼ״̬is Hidden\n3����������ͬ�����趨�����鿴��ӦRelated")]
    [Space(3)]


    [Header("Platform Setting")]
    public PlatformInteractiveType thisPlatformType;
    public bool isHidden;
    private bool canBeHaulted;
    public float haultedDuration;
    [Header("Platform Info")]
    [SerializeField] private bool isHaulted;
    [SerializeField] private float haultedCounter;
    public bool hasSensored;

    [Header("Units Related")]
    private List<PlatformUnit> units = new List<PlatformUnit>();

    [Header("Movable Related")]//���ƶ�ƽ̨����������Singler������Rounder������Sensor�������ظ��ϵ���Elevator
    [Header("4������ƶ��ٶȣ���moveSpeed\n5�������յ�λ�ã���theEndPoint��λ��\n6����sensor����ӵ���ʱ��\n7����elevator����Щ���ݹ���")]
    public float moveSpeed;//elevator����
    public Transform theStartPoint;
    public Transform theEndPoint;
    public Transform theNowPoint;
    public Transform theDestinalPoint;
    public Vector3 offsetVec;//�����ƶ�ʱ����ƫ����

    private bool isDestinationOrienting;
    private bool hasArrived;//elevator,hander����
    private float hasArrivedCounter;
    public float hasArrivedDuration;
    public int handlerInput;
    public bool hasOringinPosed;
    public float perBackStepCounter;
    public float perBackStepDuration;

    [Header("Disappear Related")]//��ʧƽ̨����������Sensor����ʱRegular�������ط��϶���Pair
    [Header("4�������ʧʱ������֣���disappearDuration��reappearDuration")]
    public bool canDisappearOrReappear;//���ڷ�ֹ��Player���µ�ʱ��մ�����ƽ̨��ײ�廹û��ʧ��ʱ���ִ���һ����ʧ
    private bool needToDisappear;
    private bool needToReappear;
    private float disappearCounter;
    public float disappearDuration;
    private float reappearCounter;
    public float reappearDuration;

    [Header("Rotater Related")]//��תƽ̨�������ɿ�rotater
    [Header("4�������תʱ�䣬��\n5��������ת�е�λ�ã���theRotatePovit��λ��\n6����Ϊelevator����Щ���ݹ���")]
    public Transform theRotatePovit_ElevatorPoint;//elevator����
    public float rotationDuration;//��ת�ܼ���Ҫ��ʱ��
    private bool isRotating;
    private float nowAngle;//�ۼƽǶȣ�Ӧ��Ҫ�浵
    private float rotateStep;//�м����
    private float hadRotated;
    private int isClockwise;

    //[Header("Switchable Related")]


    #endregion


    private void Awake()
    {
        thisComCol = GetComponent<CompositeCollider2D>();
        thisRB = GetComponent<Rigidbody2D>();

        GetComponentsInChildren(units);//��������б� 
        //units = GetComponentsInChildren<PlatformUnit>();//�����������
    }

    private void Start()//�������Ͳ�ͬ���в�ͬ��ʼ��
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.movable_singler://���ƶ�ƽ̨�������ƶ�+����
                MoveablePlatform_SingleStart();
                break;
            case PlatformInteractiveType.moveable_rounder://���ƶ�ƽ̨�������ƶ�
                MoveablePlatform_RoundStart();
                break;
            case PlatformInteractiveType.moveable_sensor://���ƶ�ƽ̨�������Ϻ�������Ŀ��ص��ƶ�������뿪һ��ʱ�������
                MoveablePlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_sensor://��ʧƽ̨��Ĭ�ϣ������Ϻ����ʱ����ʧ��һ��ʱ�������
                ; DisappearPlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_regular://��ʧƽ̨��������ʧ
                DisappearPlatform_RegularStart();
                break;
            case PlatformInteractiveType.disappearable_presser:
                DisappearPlatform_PresserStart();
                break;
            case PlatformInteractiveType.switchable_elevator://���ƶ�ƽ̨������+�ɿ��ƶ�
                SwitchablePlatform_ElevatorStart();
                break;
            case PlatformInteractiveType.moveable_handler:
                MoveablePlatform_HandlerStart();
                break;
            case PlatformInteractiveType.rotateable_rotater://��תƽ̨���ɿ���ת
                RotateablePlatform_RotaterStart();
                break;
            default://Ĭ�ϣ���������������Э�����г�ʼ����ƽ̨
                //Debug.Log("����ƽ̨��ʼ��");

                break;
        }
    }

    private void Update()//�������Ͳ�ͬ���в�ͬ��Update����Ҫ���жϺ��ƶ�
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.movable_singler:
                MoveablePlatform_SingleUpdate();
                break;
            case PlatformInteractiveType.moveable_rounder:
                MoveablePlatform_RoundUpdate();
                break;
            case PlatformInteractiveType.moveable_sensor:
                MoveablePlatform_SensorUpdate();
                break;
            case PlatformInteractiveType.moveable_handler:
                MoveablePlatform_HandlerUpdate();
                break;
            case PlatformInteractiveType.disappearable_sensor:
                DisappearPlatform_SensorUpdate();
                break;
            case PlatformInteractiveType.disappearable_regular:
                DisappearPlatform_RegularUpdate();
                break;
            case PlatformInteractiveType.switchable_elevator:
                SwitchablePlatform_ElevatorUpdate();
                break;
            case PlatformInteractiveType.rotateable_rotater:
                RotateablePlatform_AttackUpdate();
                break;
            default://Ĭ�ϣ�����������й��ɱ仯��ƽ̨
                //Debug.Log("����ƽ̨��������");
                //Debug.Log("���ƶ�ƽ̨��������"��;
                break;
        }
    }
    private void FixedUpdate()//�������Ͳ�ͬ���в�ͬ��FixUpdate��Ŀǰû��ʹ��
    {
        switch (thisPlatformType)
        {

            default://Ĭ�ϣ�������������ƶ���ƽ̨
                //Debug.Log("����ƽ̨��������");
                //Debug.Log("��ʧƽ̨ƽ̨��������"��;
                break;
        }

    }




    #region ��ʼ�����
    private void MoveablePlatform_SingleStart()
    {

        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_RoundStart()
    {
        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_SensorStart()
    {
        canBeHaulted = true;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }

    private void MoveablePlatform_HandlerStart()
    {
        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }
    private void DisappearPlatform_SensorStart()
    {
        canBeHaulted = false;
        canDisappearOrReappear = true;
    }
    private void DisappearPlatform_RegularStart()
    {
        DisappearCount();
        canBeHaulted = false;
        canDisappearOrReappear = true;
    }

    private void DisappearPlatform_PresserStart()
    {
        canBeHaulted = false;
        canDisappearOrReappear = true;
    }
    private void SwitchablePlatform_ElevatorStart()
    {
        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }
    private void RotateablePlatform_RotaterStart()
    {
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }


    #endregion

    #region ����ƽ̨Update�߼�
    private void MoveablePlatform_SingleUpdate()
    {
        if (!isHaulted)
        {
            if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
            {
                //��ƽ̨�����յ�󣬼���ƫ���������ƽ̨�����ƫ����������ϣ�ʵ�����ߵ�����
                Vector3 changeVec = theStartPoint.position - theNowPoint.position;
                theNowPoint.position += changeVec;
                transform.position += changeVec;
            }
            else
            {
                offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                theNowPoint.position += offsetVec;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;

                }
            }
        }
        else
        {
            if (haultedCounter > 0)
            {
                haultedCounter -= Time.deltaTime;
            }
            else
            {
                isHaulted = false;
            }
        }
    }

    private void MoveablePlatform_RoundUpdate()
    {
        if (!isHaulted)
        {
            if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
            {
                if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)
                {
                    theDestinalPoint.position = theEndPoint.position;
                }
                else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)
                {
                    theDestinalPoint.position = theStartPoint.position;
                }
            }
            else
            {
                offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                theNowPoint.position += offsetVec;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("������");
                }
            }
        }
        else
        {
            if (haultedCounter > 0)
            {
                haultedCounter -= Time.deltaTime;
            }
            else
            {
                isHaulted = false;
            }
        }
    }
    private void MoveablePlatform_SensorUpdate()
    {
        //��ʱ����
        if (hasArrived)
        {
            if (hasArrivedCounter > 0)
            {
                if (thePlayer == null)
                {
                    hasArrivedCounter -= Time.deltaTime;
                }
                else
                {
                    hasArrivedCounter = hasArrivedDuration;
                }
            }
            else
            {
                SetStartDestination();
            }
        }
        else
        {
            if (isDestinationOrienting)
            {
                //Debug.Log("��������")
            }
            else
            {
                //Debug.Log("�����½�")
            }
        }

        //�˶��жϲ���
        if (!isHaulted)
        {
            if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
            {
                if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)//�ص����
                {
                    if (!hasOringinPosed)
                    {
                        offsetVec = theStartPoint.position - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                        hasOringinPosed = true;
                    }
                    else
                    {
                        //Debug.Log("��Ӧ��");

                    }
                }
                else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)//����Ŀ�ĵ�
                {
                    HssArrivedCount();
                }
            }
            else
            {
                if (hasArrivedCounter <= 0)
                {
                    offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                    theNowPoint.position += offsetVec;
                    transform.position += offsetVec;
                    if (thePlayer != null)
                    {
                        //thePlayer.ClearYVelocity();
                        thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        //Debug.Log(thePlayer.horizontalInputVec);
                        //Debug.Log(thePlayer.thisRB.velocity.x);
                    }
                }
            }

        }
        else
        {
            if (haultedCounter > 0)
            {
                haultedCounter -= Time.deltaTime;
            }
            else
            {
                isHaulted = false;
            }
        }
    }
    private void MoveablePlatform_HandlerUpdate()
    {

        switch (handlerInput)
        {
            case 1:
                if (!hasArrived)
                {
                    hasOringinPosed = false;
                    if (Vector2.Distance(theNowPoint.position, theEndPoint.position) < .01F)
                    {
                        //��ƽ̨�����յ�󣬼���ƫ���������ƽ̨�����ƫ����������ϣ�ʵ�����ߵ�����
                        offsetVec = theEndPoint.position - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                        hasArrived = true;
                    }
                    else
                    {
                        offsetVec = Vector3.MoveTowards(theNowPoint.position, theEndPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                    }


                }
                else
                {
                    Debug.Log("�Ѿ�������");
                }
                break;
            case -1:
                if (!hasOringinPosed)
                {
                    hasArrived = false;
                    if (Vector2.Distance(theNowPoint.position, theStartPoint.position) < .01F)
                    {
                        //��ƽ̨�����յ�󣬼���ƫ���������ƽ̨�����ƫ����������ϣ�ʵ�����ߵ�����
                        offsetVec = theStartPoint.position - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                        hasOringinPosed = true;
                    }
                    else
                    {
                        offsetVec = Vector3.MoveTowards(theNowPoint.position, theStartPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                    }
                }
                else
                {
                    Debug.Log("�Ѿ�������");
                }
                break;
            case 0:
                if (!hasOringinPosed)
                {
                    hasArrived = false;
                    if (perBackStepCounter >= 0)
                    {
                        perBackStepCounter -= Time.deltaTime;
                    }
                    else
                    {
                        if (Vector2.Distance(theNowPoint.position, theStartPoint.position) < .01F)
                        {
                            //Debug.Log(Vector2.Distance(theNowPoint.position, theStartPoint.position));
                            //��ƽ̨�����յ�󣬼���ƫ���������ƽ̨�����ƫ����������ϣ�ʵ�����ߵ�����
                            offsetVec = theStartPoint.position - theNowPoint.position;
                            theNowPoint.position += offsetVec;
                            transform.position += offsetVec;
                            if (thePlayer != null)
                            {
                                thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                            }
                            hasOringinPosed = true;
                        }
                        else
                        {
                            offsetVec = Vector3.MoveTowards(theNowPoint.position, theStartPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                            theNowPoint.position += offsetVec;
                            transform.position += offsetVec;
                            if (thePlayer != null)
                            {
                                thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                            }
                        }
                        perBackStepCounter = perBackStepDuration;
                    }
                }
                else
                {
                    //Debug.Log("��Ӧ��");
                }
                break;
        }
    }
    private void DisappearPlatform_SensorUpdate()
    {
        if (needToDisappear)
        {
            if (disappearCounter > 0)
            {
                disappearCounter -= Time.deltaTime;
            }
            else
            {
                HideThisPlatform();
                canDisappearOrReappear = false;
                needToDisappear = false;
                ReappearCount();
            }
        }
        else if (needToReappear)
        {
            if (reappearCounter > 0)
            {
                reappearCounter -= Time.deltaTime;
            }
            else
            {
                ReappearThisPlatform();
                needToReappear = false;
                canDisappearOrReappear = true;
            }
        }
        else
        {
            //Debug.Log("��Ӧ��");
        }
    }
    private void DisappearPlatform_RegularUpdate()
    {
        if (needToDisappear)
        {
            if (disappearCounter > 0)
            {
                disappearCounter -= Time.deltaTime;
            }
            else
            {
                foreach (PlatformUnit unit in units)
                {
                    unit.Hide();
                }
                needToDisappear = false;
                ReappearCount();
            }
        }
        else if (needToReappear)
        {
            if (reappearCounter > 0)
            {
                reappearCounter -= Time.deltaTime;
            }
            else
            {
                foreach (PlatformUnit unit in units)
                {
                    unit.Appear();
                }
                needToReappear = false;
                DisappearCount();
            }
        }
        else
        {
            Debug.Log("������");
        }
    }
    private void RotateablePlatform_AttackUpdate()
    {
        if (isRotating)
        {
            transform.RotateAround(theRotatePovit_ElevatorPoint.position, Vector3.forward, isClockwise * rotateStep * Time.deltaTime);
            hadRotated += rotateStep * Time.deltaTime;
            if (hadRotated > 90f)
            {
                isRotating = false;
                nowAngle += isClockwise * 90f;
                transform.RotateAround(theRotatePovit_ElevatorPoint.position, Vector3.forward, (-hadRotated + 90f) * isClockwise * Time.deltaTime);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                AntiClockwiseRotate();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                ClockwiseRotate();
            }
        }
    }
    private void SwitchablePlatform_ElevatorUpdate()
    {
        if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
        {
            if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)
            {
                offsetVec = theStartPoint.position - theNowPoint.position;
                theNowPoint.position = theStartPoint.position;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("û�˰�");
                }
            }
            else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)
            {
                offsetVec = theEndPoint.position - theNowPoint.position;
                theNowPoint.position = theEndPoint.position;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("û�˰�");
                }
            }
            else if (Vector2.Distance(theRotatePovit_ElevatorPoint.position, theNowPoint.position) < .01F)
            {
                offsetVec = theRotatePovit_ElevatorPoint.position - theNowPoint.position;
                theNowPoint.position = theRotatePovit_ElevatorPoint.position;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("û�˰�");
                }
            }
            else
            {
                Debug.Log("������");
            }
            hasArrived = true;

        }
        else
        {
            offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
            theNowPoint.position += offsetVec;
            transform.position += offsetVec;
            if (thePlayer != null)
            {
                thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
            }
            else
            {
                //Debug.Log("û�˰�");
            }
        }
    }
    #endregion

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
    public void HideThisPlatform()
    {
        foreach (PlatformUnit unit in units)
        {
            unit.Hide();
        }
    }

    public void ReappearThisPlatform()
    {
        foreach (PlatformUnit unit in units)
        {
            unit.Appear();
        }
    }
    public void ClockwiseRotate()
    {
        isRotating = true;
        rotateStep = 90f / rotationDuration;
        isClockwise = 1;
        hadRotated = 0;
    }

    public void AntiClockwiseRotate()
    {
        isRotating = true;
        rotateStep = 90f / rotationDuration;
        isClockwise = -1;
        hadRotated = 0;
    }
    public void CallThisPlatform(Transform _destinationPoint)
    {
        theDestinalPoint.position = _destinationPoint.position;
    }

    public bool WhetherHasArrived()
    {
        return hasArrived;
    }
    public void Interactive_Sensor()
    {
        switch (thisPlatformType)
        {

            case PlatformInteractiveType.disappearable_sensor:
                //Debug.Log("�Ӵ���");
                DisappearCount();
                break;
            case PlatformInteractiveType.moveable_sensor:
                HaulThisPlatform();
                if (hasArrived)
                {
                    hasArrivedCounter = hasArrivedDuration;
                }
                else
                {
                    Activate();
                }
                break;
        }
    }
    private void Activate()
    {
        isDestinationOrienting = true;
        theDestinalPoint.position = theEndPoint.position;
    }

    private void SetStartDestination()
    {
        hasArrived = false;
        theDestinalPoint.position = theStartPoint.position;
    }

    private void HssArrivedCount()
    {
        if (isDestinationOrienting)
        {
            hasArrived = true;
            hasArrivedCounter = hasArrivedDuration;
            isDestinationOrienting = false;
        }
    }


    private void DisappearCount()
    {
        needToDisappear = true;
        disappearCounter = disappearDuration;
    }
    private void ReappearCount()
    {
        needToReappear = true;
        reappearCounter = reappearDuration;
    }


    private void HaulThisPlatform()
    {
        if (canBeHaulted)
        {
            isHaulted = true;
            haultedCounter = haultedDuration;
        }
    }


    #endregion

}
