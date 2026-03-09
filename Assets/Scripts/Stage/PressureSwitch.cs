using UnityEngine;
using UnityEngine.Events; // UnityEventを使うための準備

public class PressureSwitch : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private UnityEvent onActivate;   // 踏まれたときに発火するイベント
    [SerializeField] private UnityEvent onDeactivate; // 離れたときに発火するイベント

    private Collider onSwitch = null;

    // Is TriggerなColliderに他のRigidbodyが侵入したときに呼ばれる
    private void OnTriggerEnter(Collider other)
    {
        // 今回は「Rigidbodyを持っているもの（プレイヤーかMovableBox）」なら反応するようにする
        if ((other.attachedRigidbody != null) && (onSwitch == null))
        {
            onSwitch = other;
            onActivate?.Invoke();

            // スイッチが沈み込むような視覚的フィードバック
            transform.position -= 0.1f * transform.up;
        }
    }

    // Colliderから出たときに呼ばれる
    private void OnTriggerExit(Collider other)
    {
        if (onSwitch == other)
        {
            onSwitch = null;
            onDeactivate?.Invoke();

            transform.position += 0.1f * transform.up;
        }
    }
}