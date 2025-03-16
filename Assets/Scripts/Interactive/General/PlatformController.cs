using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D),typeof(CompositeCollider2D))]
public class PlatformController : MonoBehaviour
{
    #region ���
    [HideInInspector]public enum PlatformInteractiveType{ disappearable_sensor, disappearable_regular, switchable_pair, movable_single,moveable_round,moveable_sensor,adjustable,rotateable}
    private NewPlayerController thePlayer;
    private CompositeCollider2D thisComCol;
    private Rigidbody2D thisRB;

    #endregion

    #region ����
    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĿ������ͣ���thisPlatformType\n2���趨�ö����Ƿ��ʼ״̬is Hidden")]
    [Space(3)]


    [Header("Platform Setting")]
    public PlatformInteractiveType thisPlatformType;

    [Header("Platform Info")]
    public bool isHidden;

    [Header("Units Related")]
    private List<PlatformUnit> units = new List<PlatformUnit>();

    [Header("Movable Related")]
    public float moveSpeed;
    public Transform theStartPoint;
    public Transform theEndPoint;
    public Transform theNowPoint;
    public Transform theDestinalPoint;
    public bool isDestinationOrienting;
    public bool hasArrived;
    public float hasArrivedCounter;
    public float hasArrivedDuration;
    public Vector3 offsetVec;
    public bool isHorizontalMove;

    public bool isHault;
    public float haultCounter;
    public float haultDuration;



    [Header("Disappear Related")]
    public bool canDisappearOrReappear;//���ڷ�ֹ��Player���µ�ʱ��մ�����ƽ̨��ײ�廹û��ʧ��ʱ���ִ���һ����ʧ
    public bool needToDisappear;
    public bool needToReappear;
    public float disappearCounter;
    public float disappearDuration;
    public float reappearCounter;
    public float reappearDuration;
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
            case PlatformInteractiveType.movable_single://���ƶ�ƽ̨�������ƶ�+����
                MoveablePlatform_SingleStart();
                break;
            case PlatformInteractiveType.moveable_round://���ƶ�ƽ̨�������ƶ�
                MoveablePlatform_RoundStart();
                break;
            case PlatformInteractiveType.moveable_sensor://���ƶ�ƽ̨�������Ϻ�������Ŀ��ص��ƶ�������뿪һ��ʱ�������
                MoveablePlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_sensor://��ʧƽ̨��Ĭ�ϣ������Ϻ����ʱ����ʧ��һ��ʱ�������
;               DisappearPlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_regular://��ʧƽ̨��������ʧ
                DisappearPlatform_RegularStart();
                break;
            case PlatformInteractiveType.adjustable:

                break;
            case PlatformInteractiveType.rotateable:

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
            case PlatformInteractiveType.movable_single://���ƶ�ƽ̨�ĵڶ��֣������ƶ�+����
                MoveablePlatform_SingleUpdate();
                break;
            case PlatformInteractiveType.moveable_round://���ƶ�ƽ̨��Ĭ�ϣ������ƶ�
                MoveablePlatform_RoundUpdate();
                break;
            case PlatformInteractiveType.moveable_sensor:
                MoveablePlatform_SensorUpdate();
                
                break;
            case PlatformInteractiveType.disappearable_sensor://��ʧƽ̨��Ĭ�ϣ������Ϻ����ʱ����ʧ��һ��ʱ�������
                DisappearPlatform_SensorUpdate();
                break;
            case PlatformInteractiveType.disappearable_regular:
                DisappearPlatform_RegularUpdate();
                break;
            case PlatformInteractiveType.adjustable:

            case PlatformInteractiveType.rotateable:

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


    #region Interactive���

    #region Interactive_Moveable
    //Moveable��Ҫ��ȡPlayer��ʹplayer����
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.GetComponent<NewPlayerController>())
    //    {
    //        if (other.gameObject.GetComponent<NewPlayerController>().thisPR.theRaycastCol.collider == thisComCol)
    //        {
    //            Debug.Log("����");
    //            thePlayer = other.GetComponent<NewPlayerController>();
    //        }

    //    }
    //}
    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.GetComponent<NewPlayerController>())
    //    {
    //        Debug.Log("�˳�");
    //        thePlayer = null;
    //    }
    //}
    #endregion

    #region Interactive_sensor
    public void Interactive_Sensor()
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.disappearable_sensor:
                //Debug.Log("�Ӵ���");
                DisappearCount();
                break;
            case PlatformInteractiveType.moveable_sensor:
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

    #endregion

    #endregion

    #region ��ʼ�����
    private void MoveablePlatform_SingleStart()
    {
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_RoundStart()
    {
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_SensorStart()
    {
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
    }

    
    private void DisappearPlatform_SensorStart()
    {
        canDisappearOrReappear = true;
    }

    private void DisappearPlatform_RegularStart()
    {
        DisappearCount();
        canDisappearOrReappear = true;
    }


    #endregion

    #region ����ƽ̨Update�߼�
    private void MoveablePlatform_SingleUpdate()
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
                //Debug.Log(thePlayer.horizontalInputVec);
                //Debug.Log(thePlayer.thisRB.velocity.x);
            }
        }
    }

    private void MoveablePlatform_RoundUpdate()
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
            thisRB.MovePosition(transform.position += offsetVec);
            if (thePlayer != null)
            {
                //thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                thePlayer.transform.position += offsetVec;
                //Debug.Log(thePlayer.horizontalInputVec);
                //Debug.Log(thePlayer.thisRB.velocity.x);
            }
            else
            {
                //Debug.Log("������");
            }
        }
    }
    private void MoveablePlatform_SensorUpdate()
    {
        //��ʱ����
        if (hasArrived)
        {
            if(hasArrivedCounter>0)
            {
                if (thePlayer == null)
                {
                    hasArrivedCounter -=Time.deltaTime;
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
        if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
        {
            if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)//�ص����
            {
                //Debug.Log("��Ӧ��");
            }
            else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)//����Ŀ�ĵ�
            {
                HasArrivedCount();
            }
        }
        else
        {
            if (hasArrivedCounter <= 0)
            {
                offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                theNowPoint.position += offsetVec;
                thisRB.MovePosition(transform.position += offsetVec);
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                    //Debug.Log(thePlayer.horizontalInputVec);
                    //Debug.Log(thePlayer.thisRB.velocity.x);
                }
            }
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

    #endregion

    #region ����ƽ̨FixedUpdate�߼�


    #endregion

    #region ����С����

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
    public void Activate()
    {
        isDestinationOrienting = true;
        theDestinalPoint.position = theEndPoint.position;

    }

    public void SetStartDestination()
    {
        hasArrived = false;
        theDestinalPoint.position = theStartPoint.position;
    }

    public void HasArrivedCount()
    {
        if (isDestinationOrienting)
        {
            hasArrived = true;
            hasArrivedCounter = hasArrivedDuration;
            isDestinationOrienting = false;
        }
    }

    public void DisappearCount()
    {
        needToDisappear = true;
        disappearCounter = disappearDuration;
    }
    private void ReappearCount()
    {
        needToReappear = true;
        reappearCounter = reappearDuration;
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
    #endregion












}
