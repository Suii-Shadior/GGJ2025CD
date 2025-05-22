using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLogic:MonoBehaviour
{
    public UIController theUI;
    //[Header("Gameplaying Related")]

    [Header("MainMenu Related")]
    public GameObject theStartButton;
    public GameObject theContinueButton;
    public GameObject theQuitButton;
    [Header("PauseMenu Related")]
    public GameObject theResumeButton;
    public GameObject theMainMenuButton;

    private void Awake()
    {
        theUI = GetComponent<UIController>();
    }

    #region ��������
    public void GamePlay_ButtonPause()
    {
        theUI.FuncCall_GamePlay_Pause();

    }

    public void GamePause_ButtonResume()
    {
        theUI.FuncCall_GamePause_Resume();

    }
    public void GameMenu_ButtonStartNewGame()
    {
        theUI.FuncCall_GameMenu_GameStart();

        //��Ϸ��������
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.SetInt("HasStartGame", 1);
        //�������еĴ浵����
    }

    public void GameMenu_ButtonContinue()
    {
        theUI.FuncCall_GameMenu_Continue();


    }
    public void GameMenu_ButtonQuit()
    {
        Application.Quit();
        Debug.Log("Quit This Game");
    }


    public void GamePause_ButtonBackToMenu()
    {
        theUI.FuncCall_GamePause_BackToMenu();

    }

    public void GameSave_ButtonReturnToMenu()
    {
        theUI.FuncCal_SaveData_Return();
    }

    public void GameSave_SaveData1NewGame()
    {
        theUI.FuncCall_SaveData_NewGame("SaveData1");
    }
    public void GameSave_SaveData2NewGame()
    {
        theUI.FuncCall_SaveData_NewGame("SaveData2");

    }
    public void GameSave_SaveData3NewGame()
    {
        theUI.FuncCall_SaveData_NewGame("SaveData3");
    }
    public void GameConfirm_Ensure()
    {
        theUI.FuncCall_Confirm_Ensure();

    }
    public void GameConfirm_Cancle()
    {
        theUI.FuncCall_Confirm_Cancle();
    }

    #endregion
}
