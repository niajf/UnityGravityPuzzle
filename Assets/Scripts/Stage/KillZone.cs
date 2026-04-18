using UnityEngine;

/// <summary>
/// プレイヤーが死亡ゾーンに触れたらゲームオーバーを通知するスクリプト。
/// 落下穴や溶岩など即死エリアに配置する。
/// </summary>
public class KillZone : MonoBehaviour
{
    /// <summary>プレイヤーが触れたらゲームオーバー状態に移行する。</summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.GameOver();
        }
    }
}