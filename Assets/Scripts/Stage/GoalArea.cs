using UnityEngine;

/// <summary>
/// プレイヤーがゴールエリアに触れたらステージクリアを通知するスクリプト。
/// </summary>
public class GoalArea : MonoBehaviour
{
    /// <summary>プレイヤーが触れたらステージクリア状態に移行する。</summary>
    void OnTriggerEnter(Collider other)
    {
        // 侵入したのが「Player」タグを持つオブジェクトなら
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.StageClear();
        }
    }
}