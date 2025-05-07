using UnityEngine;

public class Other : MonoBehaviour
{
    private void Start()
    {
        EventCenter.GetInstance().AddEventListener("MonsterDead", MonsterDeadDo);
    }

    private void MonsterDeadDo()
    {
        Debug.Log("其他系统处理");
    }
}
