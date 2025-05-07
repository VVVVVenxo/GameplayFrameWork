using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        EventCenter.GetInstance().AddEventListener("MonsterDead", MonsterDeadDo);
    }

    private void MonsterDeadDo()
    {
        Debug.Log("玩家获得奖励");
    }
}