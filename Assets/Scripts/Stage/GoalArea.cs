using UnityEngine;

public class GoalArea : MonoBehaviour
{
    // プレイヤーが触れたら状態をゲームクリアに変更するメソッド
    void OnTriggerEnter(Collider other)
    {
        // 侵入したのが「Player」タグを持つオブジェクトなら
        if (other.CompareTag("Player"))
        {
            GameFlowManager.Instance.StageClear();
        }
    }
}