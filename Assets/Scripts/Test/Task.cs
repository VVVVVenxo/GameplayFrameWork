using UnityEngine;

public class Task : MonoBehaviour
{
    private void Start()
    {
        EventCenter.GetInstance().AddEventListener("MonsterDead", MonsterDeadDo);
    }

    private void MonsterDeadDo()
    {
        Debug.Log("任务记录完成");
    }
}
