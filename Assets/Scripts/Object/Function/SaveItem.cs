using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInterfaces;

public class SaveItem : MonoBehaviour,IInteract
{
    private DataController theData;
    private LevelController theLevel;
    private NewPlayerController thePlayer;
    public Transform thisLoadingPos;


    private void Start()
    {
        theData = ControllerManager.instance.theData;
        theLevel = ControllerManager.instance.theLevel;
        thePlayer = ControllerManager.instance.thePlayer;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable == null)
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
    public void Interact()
    {
        Debug.Log("��ʼ����");
        thePlayer.InteractRelated_SaveItem(thisLoadingPos.position);
        theData.SaveByJson();
        theData.SetSaveDataRelationsByPlayerPrefs();
    }
}
