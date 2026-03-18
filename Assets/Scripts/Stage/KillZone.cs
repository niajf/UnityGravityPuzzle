using UnityEngine;

public class KillZone : MonoBehaviour
{
    // プレイヤーが触れたら状態をゲームオーバーに変更するメソッド
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.GameOver();
        }
    }
}