using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CombineInteractableManager : MonoBehaviour
{
    [HideInInspector] public enum CombineInteractableType { switchplatform_pair }

    [Header("��������ʹ��˵����������\n1�������ű����صĶ�����Ϊ���е��漰����ĸ�����\n2��ѡ�񱾽ű����õ���ϻ������ͣ���thisCombineTpye\n3����������������������Ӧ�Ķ���")]
    public CombineInteractableType thisCombineTpye;


    [Header("Siwtchable Related")]
    public SwitchController[] switchs;






    private void Awake()
    {
        
    }


    private void Start()
    {
        
    }


    private void Update()
    {
        
    }


    #region �����ⲿ����

    public void SwitchsTrigger()
    {
        foreach (SwitchController _switch in switchs)
        {
            _switch.JustTrigger();
        }
    }

    #endregion
}
