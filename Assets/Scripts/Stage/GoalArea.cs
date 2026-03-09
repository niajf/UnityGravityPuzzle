using UnityEngine;

public class GoalArea : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        // 侵入したのが「Player」タグを持つオブジェクトなら
        if (other.gameObject.CompareTag("Player"))
        {
            GameFlowManager.Instance.StageClear();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.StageClear();
        }
    }
}