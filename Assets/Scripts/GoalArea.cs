using UnityEngine;

public class GoalArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 侵入したのが「Player」タグを持つオブジェクトなら
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter Goal.");
            // ゲーム進行管理クラスにクリアを通知する
            GameFlowManager.Instance.StageClear();
        }
    }
}