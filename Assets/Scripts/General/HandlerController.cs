using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using InteractiveAndInteractableEnums;

public class HandlerController : MonoBehaviour,IInteract
{

    #region ����
    [Header("Hnadler Setting")]
    private handlerType thisJamdleType;
    public bool isMirrorInput;
    [Header("Hnader Info")]
    public NewPlayerController thePlayer;
    public Vector2 HandlerInputVec;
    [Header("MoveablePlatform Related")]
    public PlatformController[] thePlatforms;
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        switch (thisJamdleType)
        {
            case handlerType.moveableplatform_handler:
                MoveablePlatform_HandlerStart();
                break;
        }
    }

    // Update is called once per frame


    private void MoveablePlatform_HandlerStart()
    {

    }











    #region �ӿ����
    public void Interact()
    {
        thePlayer.theHandle = this;
        thePlayer.ChangeToHandleState();
    }

    #endregion


    #region С�������ⲿ����

    public void HandlerUpdate()
    {
        switch (thisJamdleType)
        {
            case handlerType.moveableplatform_handler:
                MoveablePlatform_HnadlerUpdate();
                break;
        }
    }
    private void MoveablePlatform_HnadlerUpdate()
    {
        //HandlerInputVec = new Vector2(thePlayer.horizontalInputVec, thePlayer.verticalInputVec);
        foreach(PlatformController _platform in thePlatforms)
        {
            if (!isMirrorInput)
            {
                _platform.handlerInput = thePlayer.horizontalInputVec;

            }
            else
            {
                _platform.handlerInput = -thePlayer.horizontalInputVec;

            }
        }
    }
    public void ClearInput()//Player�˳�Handle״̬ʱ
    {
        switch (thisJamdleType)
        {
            case handlerType.moveableplatform_handler:
                MoveablePlatform_HnadlerExit();
                break;
        }
    }
    private void MoveablePlatform_HnadlerExit()
    {
        foreach (PlatformController _platform in thePlatforms)
        {
            _platform.handlerInput = 0;
        }
    }
    #endregion

    #region �ӿ����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable == null)
            {
                //Debug.Log("���뽻������");
                other.GetComponent<NewPlayerController>().theInteractable = this;
                thePlayer = other.GetComponent<NewPlayerController>();
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
                thePlayer = null;
                //Debug.Log("�Ƴ���������");
            }

        }
    }
    #endregion
}
