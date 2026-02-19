using UnityEngine;
using UnityEngine.Events; // UnityEventを使うための準備

public class PressureSwitch : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onActivate;   // 踏まれたときに発火するイベント
    public UnityEvent onDeactivate; // 離れたときに発火するイベント

    // Is TriggerなColliderに他のRigidbodyが侵入したときに呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        // 今回は「Rigidbodyを持っているもの（プレイヤーかMovableBox）」なら反応するようにする
        if (other.attachedRigidbody != null)
        {
            Debug.Log($"{other.name} がスイッチを踏みました！");
            onActivate?.Invoke(); // イベントを実行！

            // スイッチが沈み込むような視覚的フィードバックを入れるとベター
            transform.localScale = new Vector3(1.5f, 0.05f, 1.5f);
        }
    }

    // Colliderから出たときに呼ばれる
    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            Debug.Log($"{other.name} がスイッチから降りました。");
            onDeactivate?.Invoke();

            transform.localScale = new Vector3(1.5f, 0.2f, 1.5f);
        }
    }
}