using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class InputController : MonoBehaviour
{
    private Input1 theInput;
    private ControllerManager thisCM;//����Controller�ļ��й������ ��Ϸ�ڸ��������ͨ����Ψһʵ����ø���controller
    //private PlayerController thePlayer;
    private NewPlayerController thePlayer;
    private LevelController theLevel;//����ؿ���
    private UIController theUI;
    private DialogeController theDC;

    public int horizontalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().x != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().x > 0) ? 1 : -1) : 0;
    public int verticalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().y != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0) ? 1 : -1) : 0;

    public bool jumpWasPressed;//=>theInput.Gameplay.Jump.WasPressedThisFrame();
    public bool jumpIsHeld;//=>theInput.Gameplay.Jump.IsPressed();
    public bool jumpWasReleased;//=>theInput.Gameplay.Jump.WasReleasedThisFrame();






    private void Awake()
    {
        thisCM = GetComponentInParent<ControllerManager>();
        theInput = new Input1();
        thePlayer = thisCM.thePlayer;//Tip;���ݳ�����ͬ���ܲ��ܷ�����
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
        if (theLevel.currentSceneName != "MainMenu")
        {
            
            GamePlayInput();//Gameplay����
            //���¾���ע������
            theInput.Gameplay.Jump.started += ctx =>
            {
                if (thePlayer.canAct)
                {
                    if(thePlayer.canJump)
                    {
                        //Debug.Log("��ͨ��");
                        thePlayer.ChangeToJumpState();
                        return;
                    }else if (thePlayer.canWallJump)
                    {
                        //Debug.Logs("��ǽ����;
                    }
                    else thePlayer.JumpBufferCheck();

                }
            };

            theInput.Gameplay.Jump.canceled += ctx =>
            {
                //���ֹͣ��Ծ

                if (thePlayer.CurrentState() == thePlayer.jumpState)
                {
                    //if (thePlayer.isPastApexThreshold)
                    //{
                    //    ת��Apex״̬
                    //}
                    //else
                    //{
                    //    ת��Fall״̬
                    //}
                    //Debug.Log("��������");
                    thePlayer.HalfYVelocity();
                }
                
            };


            theInput.Gameplay.Exchange.started += ctx =>
            {
                //��ҳ��
            };
            theInput.Gameplay.Attack.started += ctx =>
            {
                if (thePlayer.canAct&&thePlayer.canAttack)
                {
                    //Debug.Log("����");
                    if(theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0)
                    {
                        thePlayer.attackCounter = 4;
                    }
                    else if (!thePlayer.thisPR.IsOnFloored()&&theInput.Gameplay.Movement.ReadValue<Vector2>().y < 0)
                    {
                        thePlayer.attackCounter = 3;
                    }
                    else
                    {
                        if (thePlayer.continueAttackCounter > 0&&thePlayer.attackCounter==1)
                        {
                            thePlayer.attackCounter = 2;
                        }
                        else
                        {
                            thePlayer.attackCounter = 1;
                        }
                    }
                    thePlayer.thisAC.SetAttackCounter();
                    thePlayer.ChangeToAttackState();

                }
            };
            theInput.Gameplay.Interact.started += ctx =>
            {
                if (thePlayer.CurrentState() == thePlayer.handleState)
                {
                    thePlayer.StateOver();//��ȷ��������û�з���
                }
                else if (thePlayer.theInteractable != null)
                {
                    thePlayer.theInteractable.Interact();
                }
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
        else
        {
            //���˵���Input
        }
    }


    private void Update()
    {
        
        //if (!thePlayer.releaseDuringRising && theInput.Gameplay.Jump.IsPressed())
        //{
        //    thePlayer.holdingCounter += Time.deltaTime;
        //    if (thePlayer.holdingCounter > thePlayer.apexThresholdLength)
        //    thePlayer.isPastApexThreshold = true;

        //}

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
        return theInput.Gameplay.Attack.ReadValue<float>() > .5f;
    }
    public bool WhetherXPressing()
    {
        return theInput.Gameplay.Exchange.ReadValue<float>() > .5f;
    }
    public bool WhetherCPressing()
    {
        return theInput.Gameplay.Jump.ReadValue<float>() > .5f;
    }
    public bool WhetherSPressing()
    {
        return theInput.Gameplay.Interact.ReadValue<float>() > .5f;
    }

}
