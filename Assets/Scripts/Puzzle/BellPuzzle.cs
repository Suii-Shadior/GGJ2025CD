using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellPuzzle : LocalPuzzleBase
{
    /*
    1���漰�����У�1��bellzones�������ڽ��ܹ�����չʾ�ɻ������2��indications�����ڼ�¼�Ѿ���ȷ��ɵĴ�����3��counterboard�����ڼ�¼������Ϸ�Ѿ���ȥ��ʱ��
    2���������̣� 1�����ռ����counterboard��ʼ��ʱ����س����Źر�(�¼�)��������������ʱ��ɻ�������ɺ󣬳����Ŵ�(�¼�)��
                  2����bellzones�����һ������Ϊ���ֻ�����󣬲�������bellzones���ţ���ҿ���ͨ���Ӿ�ȷ������Ӧ�����ĸ�����
                  3��������ڿɻ���ʱ���ڻ������������bellzones�Źرա�����һ������ȷ��������ȷ��������һ��ͬʱindicationsͬ������δ������ȷ����򳬹�����ʱ�䡣�򲻸ı��κ��������ݣ�
                  4�������ؿ��ŵ���ʱ��������ʱ������counterboard��ʱ�ڣ���ص�2����
                  5��counterboard��ʱ�ڣ�����ȷ�������ﵽĿ�꣬��������ɡ�����ʱ��������������á�
     */
    [Header("��������������������ʹ��˵����������������������\n1����������BellZone��bellZones\n2����������Indication��theIndications\n3�����ù��������¼������¿���ʱ��")]
    [Header("Setting")]
    public BellZone[] bellZones;//
    public SpriteRenderer[] theIndications;
    public float attackableDuration;
    public float reopenDuration;
    [Header("Info")]
    public bool bellWinsClosing;
    public float attackableCounter;
    public float reopenCounter;
    [Header("Indications Related")]
    public Sprite rightAttackIndication;
    //public Sprite wrongIndication;
    public Sprite unattackedIndication;
    [Header("Condition Related")]
    public int totalTurn;
    public int needToComplete;



    protected override void Awake()
    {
        base.Awake();//������
        theEC.OnLocalEvent += OnLocalEventer;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        theEC.LoaclPuzzleRegisterPublish(this);
        SceneLoad_Itself();
    }
    protected override void Start()
    {
        base.Start();

    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }




    #region ���󷽷����
    public override void SceneLoad_Itself()
    {
        if (!isSolved)
        {

        }
        else
        {

        }
    }

    public override void SceneLoad_Relative()
    {
        if (!isSolved)
        {
            foreach (BellZone bellzone in bellZones)//Step2.���������漰��������֪��������
            {
                bellzone.SetPuzzle(this);
            }
            needToComplete = totalTurn;
            foreach (SpriteRenderer indication in theIndications)
            {
                indication.sprite = unattackedIndication;
            }
        }
        else
        {

        }
    }

    public override void SceneLoad_Related()
    {
        throw new System.NotImplementedException();
    }


    public override void ThisPuzzleUpdate()
    {
        if (!isSolved)
        {
            if (isActive)
            {
                if (bellWinsClosing)
                {
                    if (reopenCounter > 0)
                    {
                        reopenCounter -= Time.deltaTime;
                    }
                    else
                    {
                        OpendWindows();
                    }
                }
                else
                {
                    if(attackableCounter > 0)
                    {
                        attackableCounter -= Time.deltaTime;
                    }
                    else
                    {
                        CloseWindows();
                    }
                }
                if (CheckComplete())
                {
                    FinishPuzzle();
                }
            }
        }
    }
    public override bool CheckComplete()
    {
        if (needToComplete == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void FinishPuzzle()
    {
        isSolved = true;
        theEC.LocalEventPublish(localPublisherChannel_FinishPuzzle);
        UnregisterLocalPuzzle(GetComponent<LocalPuzzleBase>());

    }



    public override void ActivatePuzzle()
    {
        isActive = true;

        OpendWindows();
    }
    public override void UnactivatePuzzle()
    {
        isActive = false;
        CloseWindows();

    }

    #endregion


    #region С�������ⲿ����
    public void RightAttack()
    {
        //������ȷ��Ч
        CloseWindows();
        theIndications[totalTurn-needToComplete].sprite = rightAttackIndication;
        needToComplete--;

    }
    public void WrongAttack()
    {
        //���Ŵ�����Ч
        CloseWindows();

    }

    public void CloseWindows()
    {
        bellWinsClosing = true;
        foreach (BellZone bellzone in bellZones)
        {
            bellzone.CloseWindow();
        }
        if (isActive)
        {
            reopenCounter = reopenDuration;
        }
    }

    public void OpendWindows()
    {
        bellWinsClosing = false;
        int isNeedToAttack = (int)Random.Range(0, 5);
        for(int i =0;i<=bellZones.Length;i++)
        {
            if (i == isNeedToAttack)
            {
                bellZones[i].isNeedToAttack = true;
            }
            else
            {
                bellZones[i].isNeedToAttack = false;

            }
            bellZones[i].OpenWindow();
        }
        if (isActive)
        {
            attackableCounter = attackableDuration;
        }
    }

    public override void OnLocalEventer(string _localEventerChannel)
    {
        if (canBeActivated)
        {
            if(_localEventerChannel == localSubscriberChannel_ActivatePuzzle)
            {
                if (!isActive)
                {
                    ActivatePuzzle();
                }
            }
            else if(_localEventerChannel == localSubscriberChannel_UnactivatePuzzle)
            {
                if (isActive)
                {
                    UnactivatePuzzle();
                }
            }
        }

    }
    #endregion
}
