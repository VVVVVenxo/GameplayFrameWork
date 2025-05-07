using System;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private void Start()
    {
        Dead();
    }

    void Dead()
    {
        Debug.Log("怪物死亡");
        EventCenter.GetInstance().EventTrigger("MonsterDead");
    }
}
