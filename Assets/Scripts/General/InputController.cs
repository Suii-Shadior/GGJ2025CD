using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class InputController : MonoBehaviour
{
    private Input1 theInput;
    private ControllerManager thisCM;//����Controller�ļ��й������� ��Ϸ�ڸ��������ͨ����Ψһʵ����ø���controller
    private PlayerController thePlayer;//Gameplayʱ��ҽ�ɫ
    //private LSPlayerController theLSPlayer;//LevelSelectʱ��ҽ�ɫ
    private LevelController theLevel;//�����ؿ���
    private UIController theUI;
    private DialogeController theDC;

    public int horizontalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().x != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().x > 0) ? 1 : -1) : 0;
    public int verticalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().y != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0) ? ((thePlayer.canWallClimbForward) ? 1 : 0) : -1) : 0;



    private void Awake()
    {
        thisCM = GetComponentInParent<ControllerManager>();
        theInput = new Input1();
        theLevel = thisCM.theLevel;
        theUI = thisCM.theUI;
        theDC = thisCM.theDC;
    }
    private void OnEnable()
    {
        theInput.Enable();
    }
    private void OnDisable()
    {
        theInput.Disable();
    }
    void Start()
    {
        if (theLevel.currentSceneName != "LevelSelect")
        {
            thePlayer = thisCM.thePlayer;//LevelSelect����û��������������������Awake����GamePlayͬ��
            GamePlayInput();//�����л�ActionMap
            //���¾���ע������
            theInput.Gameplay.Jump.started += ctx =>
            {
                if (thePlayer.stateMachine.currentState == thePlayer.holdState || thePlayer.stateMachine.currentState == thePlayer.wallFallState || thePlayer.stateMachine.currentState == thePlayer.wallClimbState && (thePlayer.canAct && thePlayer.canWallJump))
                {
                    thePlayer.stateMachine.ChangeState(thePlayer.walljumpState);
                    return;
                }
                else
                {
                    if (thePlayer.canAct && thePlayer.canJump)
                    {
                        if (thePlayer.stateMachine.currentState != thePlayer.dashState && thePlayer.stateMachine.currentState != thePlayer.holdState && thePlayer.stateMachine.currentState != thePlayer.wallFallState && thePlayer.stateMachine.currentState != thePlayer.wallClimbState)
                        {
                            thePlayer.stateMachine.ChangeState(thePlayer.jumpState);
                            return;
                        }
                        else thePlayer.JumpBufferCheck();
                        //else Debug.Log("��׼�壡");
                    }

                }
            };
            theInput.Gameplay.Jump.canceled += ctx =>
            {
                //���ֹͣ��Ծ
            };
            theInput.Gameplay.Dash.started += ctx =>
            {
                //��ҳ��
            };
            theInput.Gameplay.Grab.started += ctx =>
            {
                //���ץסǽ��
            };
            theInput.Gameplay.Pause.started += ctx =>
            {
                if (theLevel.currentSceneName != "MainMenu")
                {
                    if (!theLevel.isLevelLoading)
                    {
                        if (theLevel.isPausing)
                        {
                            //theUI.GamePlayResume();
                            theLevel.GamePlayResume();
                            GamePlayInput();
                        }
                        else
                        {
                            //theUI.GamePlayPause();
                            theLevel.GamePlayPause();
                            UIInput();
                        }
                    }
                }
            };
        }
    }
    private void Update()
    {
        if (theLevel.currentSceneName != "LevelSelect" && thePlayer != null)
        {

}
        else
        {
            //theLSInputVec2 = theInput.LevelSelect.Move.ReadValue<Vector2>();
        }
    }


    public void GamePlayInput()
    {

        theInput.Disable();
        theInput.Gameplay.Enable();
    }
    public void LevelSelectInput()
    {
        theInput.Disable();
        //theInput.LevelSelect.Enable();
    }
    public void UIInput()//UI�õ�
    {
        theInput.Disable();
        theInput.UI.Enable();
    }
    public void DialoguingInput()
    {
        if (theDC.isPrinting)
        {
            theDC.QuickPrint();
        }
        else
        {
            theDC.NextSentence();
        }
    }


    public bool WhetherZPressing()
    {
        return theInput.Gameplay.Grab.ReadValue<float>() > .5f;
    }
}