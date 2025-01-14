using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRelated : MonoBehaviour//���Ӧ��ҲҪ���ɽӿ�
{
    public int currentHP;
    public int HPMax;
    //[SerializeField] private int currentHP;


    private void Awake()
    {
        currentHP = HPMax;
    }

    public void FullHP()
    {
        currentHP = HPMax;
    }
    public void RecoverHP(int _recoveralue)
    {
        currentHP = (currentHP + _recoveralue > HPMax) ? HPMax : currentHP + _recoveralue;
    }


}
