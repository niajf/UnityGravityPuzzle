using UnityEngine;

public class GoalArea : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        // 侵入したのが「Player」タグを持つオブジェクトなら
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enter Goal.");
            // ゲーム進行管理クラスにクリアを通知する
            GameFlowManager.Instance.StageClear();
        }
    }
}