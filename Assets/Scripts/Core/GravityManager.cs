using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GravityManager : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;  // 重力変更の際に起点となるターゲット

    [Header("Property")]
    [SerializeField] float gravityStrength = 9.81f;

    // InputSystem
    InputAction leftGravity;
    InputAction rightGravity;

    public static GravityManager Instance { get; private set; } // 重力管理のシングルトン
    public Vector3 GravityDirection { get; private set; } = Vector3.down;   // 現在の重力ベクトル
    public event System.Action<Vector3> OnGravityChanged;   // 重力変更を通知するイベント

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else Instance = this;

        ApplyGravity(GravityDirection);

        leftGravity = InputSystem.actions.FindAction("LeftGravity");
        rightGravity = InputSystem.actions.FindAction("RightGravity");
    }

    private void Update()
    {
        // 起点となるターゲットが存在しない場合
        if (target == null)
            return;

        // 現在の状態がゲーム進行中でない場合
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
            return;

        // 左クリックが押された場合、左側面に向かって重力を変更
        if (leftGravity.WasPressedThisFrame()) ChangeGravity(-target.transform.right);

        // 右クリックが押された場合、右側面に向かって重力を変更
        if (rightGravity.WasPressedThisFrame()) ChangeGravity(target.transform.right);
    }

    // 重力を変更するメソッド
    public void ChangeGravity(Vector3 newDirection)
    {
        // ターゲットの右or左のローカルベクトルと内積が最大となるワールドベクトルを求める
        float maxDot = 0.0f;
        Vector3 GravityDirection = Vector3.up;
        var vectors = new Vector3[] { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        for (int i = 0; i < vectors.Length; i++)
        {
            float dot = Vector3.Dot(vectors[i], newDirection);
            if (maxDot < dot)
            {
                maxDot = dot;
                GravityDirection = vectors[i];
            }
        }

        // 重力を変更し、イベントの購読者に通知
        ApplyGravity(GravityDirection);
        OnGravityChanged?.Invoke(GravityDirection);
    }

    // 引数の重力を適応するメソッド
    private void ApplyGravity(Vector3 direction)
    {
        Physics.gravity = direction * gravityStrength;
    }
}
