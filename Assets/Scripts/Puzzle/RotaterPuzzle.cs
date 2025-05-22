using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaterPuzzle : LocalPuzzleBase
{

    /*
        1���漰�����У�1��rotaters�������ڽ��ܹ�����չʾ�ɲ�������
        2���������̣�1��ʹ��leveler������Ӧ��rotater��ת����ͬleveler��Ӧ��ͬrotater��
                     2����rotaterͬʱ�����Ӧ�Ƕ�Ҫ��ʱ��������ɡ�
     */
    [Header("��������������������ʹ��˵����������������������\n1����������Rotaer��rotaters\n2���������ж�Ӧ��RightAngles��theCorrespondAngles")]
    [Header("Setting")]
    public PlatformController[] rotaters;
    [Header("Condition Related")]
    public float[] theCorrespondAngles;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        SceneLoad_Itself();
        SceneLoad_Relative();
    }
    protected override void Start()
    {
        base.Start();
        SceneLoad_Related();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #region ���󷽷����
    public override void SceneLoad_Itself()
    {

    }
    public override void SceneLoad_Relative()
    {

        if (!isSolved)
        {
            foreach (PlatformController rotater in rotaters)
            {
                rotater.ResetThisPlatform();
            }
        }
        else
        {
            for (int i = 0; i < rotaters.Length; i++)
            {
                Debug.Log("ת��");
                rotaters[i].transform.RotateAround(rotaters[i].theRotatePovit_ElevatorPoint.position, Vector3.forward, theCorrespondAngles[i]);
                rotaters[i].nowAngle=theCorrespondAngles[i];
                rotaters[i].isHidden = true;
            }
        }
    }
    public override void SceneLoad_Related()
    {

    }

    public override void ThisPuzzleUpdate()
    {
        if (!isSolved)
        {
            if (CheckComplete())
            {
                FinishPuzzle();
            }

        }
    }

    public override bool CheckComplete()
    {

            bool locallPuzzleCompleted = true;
            for(int i = 0; i< rotaters.Length; i++)
            {
                if (rotaters[i].nowAngle != theCorrespondAngles[i])
                {
                    locallPuzzleCompleted = false;
                }
            }
            return locallPuzzleCompleted;
    }

    public override void FinishPuzzle()
    {
        isSolved = true;
        theEC.LocalEventPublish(localPublisherChannel_FinishPuzzle);
        foreach(PlatformController _rotater in rotaters)
        {
            _rotater.currentPlatform.Interact1();
        }

        //UnregisterLocalPuzzle(GetComponent<LocalPuzzleBase>());
    }


    public override void ActivatePuzzle()
    {
        //Debug.Log("���ض�����");
    }

    public override void UnactivatePuzzle()
    {
        //Debug.Log("���ض�����");
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
