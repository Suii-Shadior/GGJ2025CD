using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubInteractiveEnum;

public class DoorController : MonoBehaviour
{
    #region ���������
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;

    #endregion



    [Header("��������ʹ��˵����������\n1��ѡ�����õ������ͣ�thisDoorType")]
    [Space(3)]
    [Header("��������������������Door Setting��������������������")]
    public DoorInteractType thisDoorType;
    public bool isOpened;
    [Header("��������������������Door Info��������������������")]
    [Header("Opener Related")]//ͨ����Ʒ�����򿪣�����buttonָ�ɷֿ���������Ŀ�������debrisָ�ɷֿ�����������һ�뱣��Ŀ�������keyָ���ڲ�ͬ�����зֿ���������Ŀ�����
    [Header("2����Key�⣬�����Ӧ�Ŀ��Ŷ���\n3���ֶ���ӿ�����Ҫ��������needToOpen")]
    public OpenerController[] Openers;
    public int needToOpen;

    [Header("Eventer Related")]//ͨ���¼��򿪣�����localָ�����ڵ��¼������ϴ�����Ӧ���ݣ�recordָ�浵�ڵ����ݣ�����Ӧ�����ڼ���ʱ�ı�


    [Header("Animator Related")]
    private const string OPENEDSTR = "isOpened";
    private const string CLOSEDSTR = "isClosed";
    private const string OPENNINGSTR = "isOpenning";
    private const string CLOSINGSTR = "isClosing";

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
        SceneLoadSetting_OpenerControllers();//����Door���漰Button��Debris������г�ʼ��������Awake��Ҫ��Openner������Start���б���ĳ�ʼ��
    }
    private void Start()
    {
        SceneLoadSetting_DoorItself();
    }
    // Update is called once per frame
    void Update()//Ҳ�����ô������
    {
        
    }
    #region ��ʼ�����


    private void SceneLoadSetting_OpenerControllers()
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Opener_ButtonAwake();//��ȡ�浵�����¼���
                break;
            case DoorInteractType.opener_debris://��ȡ�浵
                Opener_DebrisAwake();
                break;
            case DoorInteractType.opener_key://��ȡ�浵����ȡ����
                Opener_KeyAwake();
                break;
        }
    }
    private void Opener_ButtonAwake()
    {
        //��ȡ�浵
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
            needToOpen = 0;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = true;
            }
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);
            foreach (OpenerController _opener in Openers)
            {
                //��ѯ���ԵĴ浵���,��û�б����£�����bool��ͬʱneedToOpen+1
                
            }
        }

    }
    private void Opener_DebrisAwake()
    {
        //��ȡ�浵
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = true;
            }
        }
        else
        {
            thisBoxCol.enabled = true;
            //��������
            needToOpen = Openers.Length;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = false;
            }
        }

    }
    private void Opener_KeyAwake()
    {
        //��ȡ�浵
        if (isOpened)
        {
            thisBoxCol.enabled = false;
        }
        else
        {
            thisBoxCol.enabled = true;
        }
        //��������

    }

    private void SceneLoadSetting_DoorItself()//�����Ƕ���ͬ��
    {
        if (isOpened)
        {
            thisAnim.SetBool(OPENEDSTR, true);

        }
        else
        {
            thisAnim.SetBool(CLOSEDSTR, true);

        }
    }


    #endregion



    #region �������
    public void ResetThisDoor()//���Ҫ��������,Ŀǰֻ��debris���������
    {

        switch (thisDoorType)
        {
            case DoorInteractType.opener_debris:
                Opener_DebtisReset();
                break;
        }
    }

    private void Opener_DebtisReset()
    {
        needToOpen = Openers.Length;
        foreach (OpenerController _opener in Openers)
        {
            _opener.Opener_DebrisReset();
        }
    }

    #endregion


    #region �ⲿ����


    public void OpenTheDoor()
    {
        needToOpen -= 1;
        if (needToOpen == 0)
        {
            isOpened = true;
            thisBoxCol.enabled = false;
            thisAnim.SetTrigger(OPENNINGSTR);
            //�浵
        }
    }



     public void CloseTheDoor()
    {
        isOpened = false;
        thisAnim.SetTrigger(CLOSEDSTR);
        thisBoxCol.enabled = false;
    }
    #endregion
}
