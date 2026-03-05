using UnityEngine;
using UnityEngine.Events; // UnityEventを使うための準備

public class PressureSwitch : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onActivate;   // 踏まれたときに発火するイベント
    public UnityEvent onDeactivate; // 離れたときに発火するイベント

    private Collider onSwitch = null;

    // Is TriggerなColliderに他のRigidbodyが侵入したときに呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        // 今回は「Rigidbodyを持っているもの（プレイヤーかMovableBox）」なら反応するようにする
        if ((other.attachedRigidbody != null) && (onSwitch == null))
        {
            onSwitch = other;
            Debug.Log($"{other.name} がスイッチを踏みました！");
            onActivate?.Invoke(); // イベントを実行！

            // スイッチが沈み込むような視覚的フィードバック
            transform.position -= new Vector3(0.0f, 0.1f, 0.0f);
        }
    }

    // Colliderから出たときに呼ばれる
    private void OnTriggerExit(Collider other)
    {
        if (onSwitch == other)
        {
            onSwitch = null;
            Debug.Log($"{other.name} がスイッチから降りました。");
            onDeactivate?.Invoke();

            transform.position += new Vector3(0.0f, 0.1f, 0.0f);
        }
    }
}