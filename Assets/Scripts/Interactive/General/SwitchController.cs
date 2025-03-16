using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour,IInteract
{
    #region ���������
    [HideInInspector]public enum SwitchTpye { swichabelPlatform_pair}
    private Animator thisAnim;
    private CombineInteractableManager theCombineManager;
    #endregion
    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĿ������ͣ���thisSwitchType\n2�������������������ӿ���״̬�¶�Ӧ�Ķ���\n3���趨�ö����Ƿ��ʼ״̬is Triggered���Ƿ�����Ҫ����is Primary Switch")]
    [Space(3)]

    #region ����
    [Header("Swtich Setting")]
    public SwitchTpye thisSwitchType;
    public bool isPrimarySwitch;//ͨ���������ж����ڳ�ʼ���Ķ���Χ�������ظ�����ͻ
    public float canInteractedDuration;
    [Header("Switch Info")]
    public bool isTriggered;
    public bool canInteracted;
    private float canInteractedCounter;
    [Header("Combine Related")]
    public PlatformController[] triggeredPlatforms;
    public PlatformController[] unTriggeredPlatforms;
    [Header("Animator Related")]
    private const string TRIGGEREDSTR = "isTriggered";
    private const string UNTRIGGEREDSTR = "isUntriggered";
    private const string TRIGGERINGSTR = "isTriggering";
    private const string UNTRIGGERINGSTR = "isUntriggering";

    #endregion

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//��ȡ������Ҫ��ʵ�ֶ����֮���ͬ��
        SceneLoadSetting_PlatformControllers();//����Switch���漰������г�ʼ��������Awake��Ҫ����PlaytformUnit����Start����Platform������Ҳ���г�ʼ�����������������Start֮ǰ

    }

    void Start()
    {
        SceneLoadSetting_SwitchItself();//����Switch���������Start���г�ʼ��
    }

    void Update()//���ݿ������ͽ��в�ͬ��Update
    {
        switch (thisSwitchType)
        {
            case SwitchTpye.swichabelPlatform_pair:
                SwitchablePlatform_PairUpdate();
                break;

        }
    }
    private void FixedUpdate()
    {
        
    }

    #region ��ʼ�����
    private void SceneLoadSetting_SwitchItself()//���ݿ������ͺ�������ݶԱ������ݽ��г�ʼ��
    {
        //��Ҫ�ô浵��¼
        if (isTriggered)
        {
            thisAnim.SetBool(TRIGGEREDSTR, true);
            //thisAnim.SetBool(UNTRIGGEREDSTR, false);
        }
        else
        {
            //thisAnim.SetBool(TRIGGEREDSTR, false);
            thisAnim.SetBool(UNTRIGGEREDSTR, true);
        }
    }

    private void SceneLoadSetting_PlatformControllers()//���ݿ������ͺ�������ݶ����е��漰������г�ʼ��
    {
        switch (thisSwitchType)
        {
            case SwitchTpye.swichabelPlatform_pair:
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
                            triggeredPlatform.isHidden =true;
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
                break;

        }
    }
    #endregion

    #region Update���
    private void SwitchablePlatform_PairUpdate()
    {
        if (!canInteracted)
        {
            if (canInteractedCounter>0)
            {
                canInteractedCounter -= Time.deltaTime;
            }
            else
            {
                canInteracted = true;
            }

        }
        else
        {

            //Debug.Log("��������");
        }
    }
    #endregion

    #region Interact�ӿ����
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable==null&& other.GetComponent<NewPlayerController>())
            {
                //Debug.Log("���뽻������");
                other.GetComponent<NewPlayerController>().theInteractable = this;
                //��ʾ����ָʾ
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>().theInteractable == this.GetComponent<IInteract>())
        {
            other.GetComponent<NewPlayerController>().theInteractable = null;
            //Debug.Log("�Ƴ���������");
        }
    }
    public void Interact()//���ݲ�ͬ���ͽ��в�ͬ�Ľ�������
    {
        switch (thisSwitchType) 
        {
            case SwitchTpye.swichabelPlatform_pair:
                //Debug.Log("����1");
                SwitchablePlatform_PairInteract();
                break;
        }
    }
    private void SwitchablePlatform_PairInteract()
    {
        if (canInteracted)
        {
            if (isTriggered)
            {
                foreach (PlatformController _triggeredPlatform in triggeredPlatforms)
                {
                    _triggeredPlatform.HideThisPlatform();
                }
                foreach (PlatformController _triggeredPlatform in unTriggeredPlatforms)
                {
                    _triggeredPlatform.ReappearThisPlatform();
                }

            }
            else
            {
  
                foreach (PlatformController triggeredPlatform in triggeredPlatforms)
                {
                    triggeredPlatform.ReappearThisPlatform();
                }
                foreach (PlatformController triggeredPlatform in unTriggeredPlatforms)
                {
                    triggeredPlatform.HideThisPlatform();
                }

            }
            theCombineManager.SwitchsTrigger();
        }
    }
















    #endregion

    #region �����ⲿ����
    public void JustTrigger()
    {
        if (isTriggered)
        {
            isTriggered = false;
            thisAnim.SetTrigger(UNTRIGGERINGSTR);
        }
        else
        {
            isTriggered = true;
            thisAnim.SetTrigger(TRIGGERINGSTR);
        }
        canInteracted = false;
        canInteractedCounter = canInteractedDuration;
    }
    #endregion

}
