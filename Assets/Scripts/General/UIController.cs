using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //public static UIController instance;
    private ControllerManager thisCM;
    //private PlayerController thePlayer;
    private NewPlayerController thePlayer;
    private LevelController theLevel;
    private InputController theInput;
    private DialogeController theDC;//���ڴ����Ի�

    [SerializeField] private GameObject theMainmenuUI;
    [SerializeField] private GameObject theGamePlayLevelUI;
    [SerializeField] private GameObject thePauseUI;
    [SerializeField] private GameObject theDialogeUI;

    [Header("Transition")]
    public Image theBlackScreen;
    public float fadeInRatio;
    public float fadeOutRatio;
    public bool isFadeIn;
    public bool isFadeOut;


    [Header("MainMenu Info")]
    //public GameObject theStartButton;
    //public GameObject theContinueButton;
    //public GameObject theQuitButton;
    //public Transform theFirstStartPos;
    //public Transform theFirstQuitPos;



    [Header("Skill Info")]
    //public Image thePullInfo;
    //public Image thePullGrayInfo;
    //public Image theBoomInfo;
    //public Image theElectricInfo;
    //public Image thePlantInfo;
    //public Image theBabbleInfo;
    //public TextMeshProUGUI thePullText;
    //public TextMeshProUGUI theBoomText;
    //public TextMeshProUGUI theElectricText;
    //public TextMeshProUGUI thePlantText;
    //public TextMeshProUGUI theBabbleText;
    //private Vector4 uncooldownColor = new Vector4(1, 1, 1, 0);
    //private Vector4 cooldownColor = new Vector4(0, 0, 0, 1);
    //public Sprite NoAbilitySprite;
    //public List<Image> needCooldownSkills = new List<Image>();
    [Header("Dialogue")]
    public bool isDialogue;
    private void Awake()
    {
    }
    private void Start()
    {
        thisCM = ControllerManager.instance;
        theLevel = thisCM.theLevel;
        thePlayer = thisCM.thePlayer;
        theInput = thisCM.theInput;
        theDC = thisCM.theDC;
        UICanvasSelect();
        UIObjectsUpdate();
        FadeIn();
    }


    private void Update()
    {
        FadeInOrFadeOut();//Step1.���뵭��
        UIDisplayUpdate();//Step2.��������ȫ��UI�����Ϣ�������¼���Ѫ��������
        //Step3.����������ȫ��UI��Ϣ�����紥����ʾ
    }

    #region ��������
    private void FadeInOrFadeOut()
    {
        if (isFadeIn)
        {
            //Debug.Log("����");
            theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, Mathf.MoveTowards(theBlackScreen.color.a, 0f, fadeInRatio * Time.deltaTime));
            if (theBlackScreen.color.a == 0f)
            {
                isFadeIn = false;

            }

        }
        if (isFadeOut)
        {
            //Debug.Log("���� ");
            theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, Mathf.MoveTowards(theBlackScreen.color.a, 1f, fadeOutRatio * Time.deltaTime));
            if (theBlackScreen.color.a == 1f)
            {
                isFadeOut = false;

            }
        }
    }

    public void FadeIn()
    {
        isFadeIn = true;
        isFadeOut = false;
    }
    public void FadeOut()
    {

        isFadeIn = false;
        isFadeOut = true;
    }

    #endregion
    #region GUI����
    private void UICanvasSelect()//���ݵ�ǰ��������UI���ڻ���
    {
        //theBlackScreen.transform.gameObject.SetActive(true);
        if (theLevel.currentSceneName == "MainMenu")
        {
            theMainmenuUI.SetActive(true);
            theGamePlayLevelUI.SetActive(false);
            theBlackScreen.transform.SetParent(theMainmenuUI.transform, false);
        }
        else
        {
            //theMainmenuUI.SetActive(false);
            theGamePlayLevelUI.SetActive(true);
            theBlackScreen.transform.SetParent(theGamePlayLevelUI.transform, false);

        }
    }
    public void UIObjectsUpdate()//������Ϸ���ȵ���UI��ʾ����
    {
        if (theLevel.currentSceneName == "MainMenu")
        {
            if (!PlayerPrefs.HasKey("HasStartGame"))
            {
                //�Ѿ���ʼ����Ϸ
                //theStartButton.transform.position = theFirstStartPos.position;
                //theContinueButton.SetActive(false);
                //theStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
                //theQuitButton.transform.position = theFirstQuitPos.position;
            }
            else
            {
                //��û�п�ʼ����Ϸ
            }
        }
        else
        {
            //Gameplayʱ����¶���
        }
    }
    public void UIDisplayUpdate()//������Ϸ���ȸ���UI��ʾ����
    {

    }


    public void TurnOnDialogCanvas()
    {
        theDialogeUI.SetActive(true);
        isDialogue = true;
    }
    public void TurnOffDialogCanvas()
    {
        theDialogeUI.SetActive(false);
        isDialogue = false;
    }
    public void GamePlayPause()
    {
        thePauseUI.SetActive(true);
    }

    public void GamePlayResume()
    {
        thePauseUI.SetActive(false);
    }
    #endregion

    #region �¼�֡����
    public void theStartFunction()
    {
        //PlayerPrefs.SetInt("AnimatonTransition", 1);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("HasStartGame", 1);
        PlayerPrefs.SetInt("Level1" + "_unlocked", 1);
        //theLevel.GoToLevelSelected("LevelSelect");
        theLevel.GoToLevelSelected("AnimationTransition");

    }
    public void theContinueFunction()
    {
        theLevel.BackToLevelSelect();
    }
    public void theQuitFunction()
    {
        Application.Quit();
        Debug.Log("Quit This Game");
    }

    public void theResumeFunction()
    {

        GamePlayResume();
        theLevel.GamePlayResume();
        if (theLevel.currentSceneName != "LevelSelect")
        {
            theInput.GamePlayInput();
        }
        else
        {
            theInput.LevelSelectInput();
        }
    }

    public void theMainMenuFunction()
    {
        theLevel.BackToMainMenu();
    }

    #endregion
}
