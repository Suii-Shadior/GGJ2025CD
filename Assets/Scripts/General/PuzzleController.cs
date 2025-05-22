using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{

    private EventController theEC;

    [Header("Local Related")]

    private List<LocalPuzzleBase> activeLocalPuzzles = new();
    private List<GlobalPuzzleBase> activeGlobalPuzzles = new();
    [Header("Global Related")]
    private Dictionary<string, string[]> globalPuzaleRefer = new Dictionary<string, string[]>(); //����string���Ǵ浵�ڵ�Saveab������ʵȫ�����ⲻ���ظ�
    private Dictionary<string, int> globalPuzzleCheck = new Dictionary<string, int>();
    private Dictionary<string, string> globalFinishRefer = new Dictionary<string, string>();
    [Header("Save Related")]
    private const string PUZZLESTR = "Puzzle"; 

    private void Awake()
    {
        theEC = GetComponentInParent<ControllerManager>().theEvent;
        theEC.OnLocalPuzzleRegister += RegisterLocalPuzzle;
        theEC.OnLocalPuzzleUnregister += UnregisterLocalPuzzle;
        theEC.OnGlobalPuzzleRegister += RegisterGlobalPuzzle;

    }


    private void Start()
    {
        
        theEC.OnGlobalEvent += FinishGlobalPuzzle;

    }
    void Update()
    {
        foreach (LocalPuzzleBase activeLocakPuzzle in activeLocalPuzzles)// Step1.�����л״̬�µ�������и��¼��
        {
            activeLocakPuzzle.ThisPuzzleUpdate();


            //    if (activePuzzle.isLocalPuzzle)//Step2-1.�����Ƿ��Ǳ�����������ж�
            //    {
            //        activePuzzle.ThisPuzzleUpdate();
            //    }
            //    if (activePuzzle.isGlobalPuzzle)//Step2-2.�����Ƿ���ȫ����������ж�
            //    {
            //        foreach(string globalPuzzleString in activePuzzle.globalPuzzleStrings)//Step3.�������е�ȫ��������������ȫ���ж�
            //        {
            //            if (GlobalPuzzleCheck(globalPuzzleString))
            //            {
            //                FinishGlobalPuzzle(globalPuzzleString);
            //            }

            //        }
            //    }
        }
    }
    #region ע������
    public void RegisterLocalPuzzle(LocalPuzzleBase puzzle)
    {
        activeLocalPuzzles.Add(puzzle);
    }

    public void UnregisterLocalPuzzle(LocalPuzzleBase puzzle)
    {
        activeLocalPuzzles.Remove(puzzle);

    }
    public void RegisterGlobalPuzzle(GlobalPuzzleBase puzzle)
    {
        activeGlobalPuzzles.Add(puzzle);
    }
    public void UnregisterGlobalPuzzle(GlobalPuzzleBase puzzle)
    {
        activeGlobalPuzzles.Remove(puzzle);
    }
    #endregion
    private bool GlobalPuzzleCheck(string puzzleName)
    {
        bool globalPuzzleCompleted = true;
        foreach(string _referPuzzleName in globalPuzaleRefer[puzzleName])//Step1.�Ը�ȫ�����������漰�����ж�������ڻ����浵�����ж�
        {

            if (PlayerPrefs.GetInt(_referPuzzleName) != globalPuzzleCheck[_referPuzzleName])//Step2.���ö���Ļ����浵���ȫ�������ڸö���Ķ�Ӧ��Ҫ��ͬ���򷵻ط���������ֱ�������ͬ���򷵻���
            {
                globalPuzzleCompleted = false;
                return globalPuzzleCompleted;
            }
        }
        return globalPuzzleCompleted;
        //puzzle.EnableInteractions(false);
    }

    public void FinishGlobalPuzzle(string puzzleName)
    {
        //�޸Ļ����浵
    }

}
