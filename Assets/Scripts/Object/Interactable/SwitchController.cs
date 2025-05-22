using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveAndInteractableEnums;
using PlayerInterfaces;

public class SwitchController : MonoBehaviour,IInteract
{
    #region ���
    private Animator thisAnim;
    private CombineInteractableManager theCombineManager;
    #endregion



    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĿ������ͣ���thisSwitchType\n2���趨�ö����Ƿ��ʼ״̬is Triggered���Ƿ�����Ҫ����is Primary Switch")]
    [Space(3)]
    #region ����
    [Header("Swtich Setting")]
    public InteractableConfigSO interactableConfigSO;
    public switchTpye thisSwitchType;
    public bool isPrimarySwitch;//ͨ���������ж����ڳ�ʼ���Ķ���Χ�������ظ�����ͻ

    [Header("Switch Info")]
    public bool isTriggered;
    public bool canTriggered;

    [Header("Alternative Related")]//�漰����ֻ�п��ض�Ӧ������״̬�߼�
    [Header("3����Ӹ�λʱ�䣬��canTriggeredDuration\n4����Ӷ��񿪹�״̬��Ӧ����")]

    private float canTriggeredCounter;
    public PlatformController[] triggeredPlatforms;
    public PlatformController[] unTriggeredPlatforms;

    [Header("Autoresetable Related")]//�漰����ֻ��һ��״̬�߼��������߼��п��ܴ��ڶ���ɿؽ��
    [Header("3����Ӹÿ��صĶ�Ӧ����λ�ã���thisElevatorArrivalPoint\n4����ӵ��ݶ���")]
    public Transform thisElevatorArrivalPoint;
    public PlatformController theElevator;




    #endregion

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//��Ҫ����ͬ�����switch��״̬
        SceneLoadSetting_PlatformControllers();//����Switch���漰������г�ʼ��������Awake��Ҫ����PlaytformUnit����Start����Platform������Ҳ���г�ʼ�����������������Start֮ǰ

    }

    void Start()
    {
        SceneLoadSetting_SwitchItself();//����Switch���������Start���г�ʼ��

    }

    #region ��ʼ�����
    private void SceneLoadSetting_PlatformControllers()//pairҪ�Կ���״̬�������֣�����elevator���ÿ�����������ָø�ʽ��Ϊ��δ����չ�Ժ͸�ʽһ����
    {
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairAwake();//��ʹ�и�primaryҲֻ�����ڶ�ƽ̨�ĳ�ʼ�����������Ķ��񿪹ر���һ��Ҫ�������ó�ʼ���߸��ݴ浵
                break;

        }
    }

    private void Alternative_PairAwake()
    {
        if (isPrimarySwitch)
        {
            if (isTriggered)
            {
                foreach (PlatformController triggeredPlatform in triggeredPlatforms)
                {
                    triggeredPlatform.isHidden = false;
                }
                foreach (PlatformController triggeredPlatform in unTriggeredPlatforms)
                {
                    triggeredPlatform.isHidden = true;
                }
            }
            else
            {
                foreach (PlatformController triggeredPlatform in triggeredPlatforms)
                {
                    triggeredPlatform.isHidden = true;
                }
                foreach (PlatformController triggeredPlatform in unTriggeredPlatforms)
                {
                    triggeredPlatform.isHidden = false;
                }
            }
        }
    }

    private void SceneLoadSetting_SwitchItself()//���񿪹ر���һ��Ҫ�������ó�ʼ���߸��ݴ浵����������Ƕ���ͬ����=
    {
        //��Ҫ�ô浵��¼
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairStart();
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorStart();
                break;

        }
    }

    private void Alternative_PairStart()//
    {
        if (isTriggered)
        {
            thisAnim.SetBool(interactableConfigSO.Switch_TRIGGEREDSTR, true);
        }
        else
        {
            thisAnim.SetBool(interactableConfigSO.Switch_UNTRIGGEREDSTR, true);
        }
    }

    private void AutoResetable_ElevatorStart()
    {
        thisAnim.SetBool(interactableConfigSO.Switch_UNTRIGGEREDSTR, true);
    }

    #endregion

    #region Update���
    void Update()
    {
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairUpdate();//����֮��̶�ʱ�临λ
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorUpdate();//�������⵽���λ
                break;



        }
    }
    private void Alternative_PairUpdate()
    {
        if (!canTriggered)
        {
            if (canTriggeredCounter > 0)
            {
                canTriggeredCounter -= Time.deltaTime;
            }
            else
            {
                canTriggered = true;
            }
        }
        else
        {
            //Debug.Log("��������");
        }
    }

    private void AutoResetable_ElevatorUpdate()
    {
        if (isPrimarySwitch && theElevator.hasArrived)
        {
            theCombineManager.SwitchReset();
        }
    }

    #endregion

    #region �ӿ����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable==null)
            {
                //Debug.Log("���뽻������");
                other.GetComponent<NewPlayerController>().theInteractable = this;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable == this.GetComponent<IInteract>())
            {
                other.GetComponent<NewPlayerController>().theInteractable = null;
                //Debug.Log("�Ƴ���������");
            }

        }
    }
    #endregion

    #region С�������ⲿ����
    public void hadAutoReset()//����elevator���������ظ�λ
    {
        canTriggered = true;
        isTriggered = false;
        thisAnim.SetTrigger(interactableConfigSO.Switch_UNTRIGGERINGSTR);
    }
    public void Interact()//���ݲ�ͬ���ͽ��в�ͬ�Ľ�������
    {
        switch (thisSwitchType) 
        {
            case switchTpye.alternative_pair:
                Alternative_PairInteract();//���ж���ƽ̨��״̬ת��
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorInteract();//���е��ݵ���
                break;
        }
    }
    private void Alternative_PairInteract()
    {
        if (canTriggered)
        {
            if (isTriggered)
            {
                foreach (PlatformController _triggeredPlatform in triggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact1();
                }
                foreach (PlatformController _triggeredPlatform in unTriggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact2();
                }

            }
            else
            {
  
                foreach (PlatformController _triggeredPlatform in triggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact2();
                }
                foreach (PlatformController _triggeredPlatform in unTriggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact1();
                }

            }
            theCombineManager.SwitchsTrigger();
        }
    }

    private void AutoResetable_ElevatorInteract()
    {
        if (canTriggered)
        {
            if (!isTriggered)
            {
                //Debug.Log("���ݿ�����");
                theElevator.CallThisPlatform(thisElevatorArrivalPoint);
                theCombineManager.SwitchsTrigger();
            }
            else
            {
                //Debug.Log("��ʱ�޷��ٽ���");
            }
        }
    }


    public void JustTrigger()//����ĳ���غ�������صĵ�ͬ��
    {
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairJustTrigger();
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorJustTrigger();
                break;

        }


    }

    private void Alternative_PairJustTrigger()
    {
        if (isTriggered)
        {
            isTriggered = false;
            thisAnim.SetTrigger(interactableConfigSO.Switch_UNTRIGGERINGSTR);
        }
        else
        {
            isTriggered = true;
            thisAnim.SetTrigger(interactableConfigSO.Switch_TRIGGERINGSTR);
        }
        canTriggered = false;
        canTriggeredCounter = interactableConfigSO.canTriggeredDuration;
    }

    private void AutoResetable_ElevatorJustTrigger()
    {
        isTriggered = true;
        canTriggered = false;
        thisAnim.SetTrigger(interactableConfigSO.Switch_TRIGGERINGSTR);

    }

    #endregion

}
