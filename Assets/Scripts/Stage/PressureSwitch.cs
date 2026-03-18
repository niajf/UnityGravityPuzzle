using UnityEngine;
using UnityEngine.Events; // UnityEventを使うための準備

public class PressureSwitch : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] UnityEvent onActivate;   // 踏まれたときに発火するイベント
    [SerializeField] UnityEvent onDeactivate; // 離れたときに発火するイベント

    Collider onSwitch = null;

    // Is TriggerなColliderに他のRigidbodyが侵入したときに呼ばれる
    void OnTriggerEnter(Collider other)
    {
        // スイッチが誰にも踏まれていない場合のみ、スイッチを踏まれた処理を行う
        if ((other.attachedRigidbody != null) && (onSwitch == null))
        {
            // スイッチを踏んでいるオブジェクトを記憶する
            onSwitch = other;

            // 割り当てられたイベントを発火
            onActivate?.Invoke();

            // スイッチが沈み込むような視覚的フィードバック
            transform.position -= 0.1f * transform.up;
        }
    }

    // Colliderから出たときに呼ばれる
    void OnTriggerExit(Collider other)
    {
        // 記録していたオブジェクトがスイッチから離れた場合のみ、処理を実行する
        if (onSwitch == other)
        {
            onSwitch = null;
            onDeactivate?.Invoke();
            transform.position += 0.1f * transform.up;
        }
    }
}