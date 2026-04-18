using UnityEngine;

/// <summary>
/// プレイヤーがステージ範囲外に出たらゲームオーバーを通知するスクリプト。
/// ステージ全体を囲む不可視トリガーに配置する。
/// </summary>
public class OutStage : MonoBehaviour
{
    /// <summary>プレイヤーがトリガー範囲から出たらゲームオーバー状態に移行する。</summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.GameOver();
        }
    }
}
