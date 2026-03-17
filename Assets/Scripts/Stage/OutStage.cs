using UnityEngine;

public class OutStage : MonoBehaviour
{
    // プレイヤーが離れたら状態をゲームオーバーに変更するメソッド
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.GameOver();
        }
    }
}
