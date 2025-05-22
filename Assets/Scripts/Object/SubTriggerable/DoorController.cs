using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubInteractiveEnum;
using System;
using StructForSaveData;
using static UnityEngine.EventSystems.EventTrigger;


public class DoorController : MonoBehaviour, ISave<DoorSaveData>
{
    #region ���������
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    private EventController theEC;

    public DoorSaveData thisDoorSaveData;
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
    public string localSubscriberChannel;

    [Header("Animator Related")]
    private const string OPENEDSTR = "isOpened";
    private const string CLOSEDSTR = "isClosed";
    private const string OPENNINGSTR = "isOpenning";
    private const string CLOSINGSTR = "isClosing";

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        theEC = ControllerManager.instance.theEvent;


        SceneLoadSetting_Itself();//��ȡ������ߴ浵����ȡ��ӦSaveData���ݲ��Ա�����г�ʼ��
        SceneLoadSetting_Relative();//����SaveData�����ݶԱ����漰�������ǰ����ɴ洢���ݵĶ�����г�ʼ��


    }
    private void Start()
    {
        SceneLoadSetting_Related();//���ݱ�����Ƶ����������ɴ洢���ݵĶ�����г�ʼ��
    }
    // Update is called once per frame
    #region ��ʼ�����


    private void SceneLoadSetting_Itself()
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Collector_Button_Itself();//��ȡ�浵�����¼���
                break;
            case DoorInteractType.opener_debris://��ȡ�浵
                Collector_Debris_Itself();
                break;
            case DoorInteractType.opener_key://��ȡ�浵����ȡ����
                Collector_Key_Itself();
                break;
            case DoorInteractType.eventer_local:
                Eventer_Local_Itself();
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_Itself();
                break;

        }
    }


    #region ��ʼ�����
    #region Collector_Button
    private void Collector_Button_Itself()
    {

        RegisterSaveable(GetComponent<ISaveable>());
        isOpened = thisDoorSaveData.isOpened;
        needToOpen = thisDoorSaveData.needToOpen;
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);

        }

    }
    private void Collector_Button_Relative()
    {


    }
    private void Collector_Button_Related()
    {
        if (isOpened)
        {
            //Debug.Log("");
        }
        else
        {

        }
    }


    #endregion


    #region Collector_Debris
    private void Collector_Debris_Itself()
    {
        //��ȡ�浵
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = true;
            }
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);
            //��������
            needToOpen = Openers.Length;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = false;
            }
        }

    }

    private void Collector_Debris_Relative()
    {
        //LoadThisISave();

    }
    private void Opener_Debris_Related()
    {

    }
    #endregion


    #region Collector_Key
    private void Collector_Key_Itself()
    {

        RegisterSaveable(GetComponent<ISaveable>());
        isOpened = thisDoorSaveData.isOpened;
        needToOpen = thisDoorSaveData.needToOpen;
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);

        }

    }

    private void Collector_Key_Relative()
    {
       // LoadThisISave();

    }
    private void Collector_Key_Related()
    {
        if (isOpened)
        {
            //Debug.Log("");
        }
        else
        {

        }
    }
    #endregion

    #region Eventer_Local
    private void Eventer_Local_Itself()//���������趨�Ƿ���Ҫ�¼����ţ�Ĭ���ڱ����¼�����
    {
        RegisterSaveable(GetComponent<ISaveable>());
        isOpened = thisDoorSaveData.isOpened;
        needToOpen = thisDoorSaveData.needToOpen;
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);

        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);
        }
    }
    private void Eventer_Local_Relative()
    {

        if (!isOpened)
        {
            theEC.OnLocalEvent += OnLoaclEventer;

        }
    }
    private void Eventer_Local_Related()
    {

    }
    #endregion
    #region Eventer_Global
    private void Eventer_Global_Itself()//Ĭ�Ͻ����ݻ����浵���ţ�
    {
        //��ȡ�浵
        thisBoxCol.enabled = true;
        thisAnim.SetBool(CLOSEDSTR, true);
    }
    private void Eventer_Global_Relative()
    {
        theEC.SaveableRegisterPublish(this.GetComponent<ISaveable>());
    }
    private void Eventer_Global_SLRelatedLater()
    {

    }

    #endregion
    #endregion
    private void SceneLoadSetting_Relative()
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Collector_Button_Relative();//������
                break;
            case DoorInteractType.opener_debris://��ȡ�浵
                Collector_Debris_Relative();
                break;
            case DoorInteractType.opener_key://��ȡ�浵����ȡ����
                Collector_Key_Relative();
                break;
            case DoorInteractType.eventer_local:
                Eventer_Local_Relative();//��ȡ�浵
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_Relative();//��ȡ�浵
                break;

        }
    }






     private void SceneLoadSetting_Related()
     {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Collector_Button_Related();
                break;
            case DoorInteractType.opener_debris:
                Collector_Debris_Relative();
                break;
            case DoorInteractType.opener_key:
                Collector_Key_Related();
                break;
            case DoorInteractType.eventer_local:
                Eventer_Local_Related();
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_SLRelatedLater();
                break;
        }
    }



    #endregion

    void Update()//Ҳ�����ô������
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                break;
            case DoorInteractType.opener_debris://��ȡ�浵
                break;
            case DoorInteractType.opener_key://��ȡ�浵����ȡ����
                break;
            case DoorInteractType.eventer_local:
                Eventer_LocalUpdate();
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_Itself();
                break;

        }
    }


    private void Eventer_LocalUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            theEC.LocalEventPublish(localSubscriberChannel);
        }
    }
    private void Eventer_GlobalUpdate() 
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            theEC.LocalEventPublish(localSubscriberChannel);
        }
    }

    private void OnDisable()
    {
        SaveDataSync();
        theEC.SaveableUnregisterPublish(GetComponent<ISaveable>());
    }








    #region �������
    public void ResetThisDoor()//���Ҫ��������,Ŀǰֻ��debris���������
    {

        switch (thisDoorType)
        {
            case DoorInteractType.opener_debris:
                Opener_DebtisReset();
                break;
            case DoorInteractType.eventer_local:
                Eventer_LocalReset();
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
    private void Eventer_LocalReset()
    {
        thisBoxCol.enabled = false;
        thisAnim.SetBool(OPENEDSTR, true);
    }



    #endregion


    #region С�������ⲿ����

    private void OnLoaclEventer(string _localEventerChannel)
    {
        if (!isOpened)
        {
            if(_localEventerChannel == localSubscriberChannel)
            {
                Debug.Log("�����¼��ŵ���");
                isOpened = true;
                thisBoxCol.enabled = false;
                thisAnim.SetTrigger(OPENNINGSTR);
                //theEC.OnLocalEvent -= OnLoaclEventer;
            }
            else
            {
                Debug.Log("���Ǳ����¼���");

            }
        }
        else
        {

        }

    }


    public void OpenTheDoor()
    {

        if (needToOpen > 0)
        {
            needToOpen--;
            if (needToOpen == 0)
            {
            isOpened = true;
            thisBoxCol.enabled = false;
            thisAnim.SetTrigger(OPENNINGSTR);
            }
        }

    }



     public void CloseTheDoor()
    {
        isOpened = false;
        thisAnim.SetTrigger(CLOSEDSTR);
        thisBoxCol.enabled = false;
    }






    #region ISave�ӿ�ʵ��




    public string GetISaveID()
    {
        return thisDoorSaveData.SaveableID;
    }
    public void RegisterSaveable(ISaveable _saveable)
    {

        theEC.SaveableRegisterPublish(_saveable);

    }

    public void UnregisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableUnregisterPublish(_saveable);
    }

    public void LoadSpecificData(SaveData _passingData)
    {
       thisDoorSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
        //Debug.Log(thisDoorSaveData.isOpened);

    }

    public DoorSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        DoorSaveData thedata = JsonUtility.FromJson<DoorSaveData>(_passedData.value);
        //Debug.Log(thedata.isOpened);
        return thedata;
    }


    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        {
            key = thisDoorSaveData.SaveableID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(DoorSaveData)),
            value = JsonUtility.ToJson(thisDoorSaveData)
        };
        return _savedata;
    }

    public void SaveDataSync()
    {
        thisDoorSaveData.isOpened = isOpened;
        thisDoorSaveData.needToOpen = needToOpen;
    }




    #endregion
    #endregion
}
