using UnityEngine;

/// <summary>
/// 重力方向を一元管理するシングルトン。
/// Physics.gravity を直接操作し、変更を OnGravityChanged イベントで購読者へ通知する。
/// </summary>
[DefaultExecutionOrder(-5)]
public class GravityManager : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;  // 重力変更の起点となるターゲット（プレイヤー）

    [Header("Property")]
    [SerializeField] float gravityStrength = 9.81f;

    public static GravityManager Instance { get; private set; }
    public Vector3 GravityDirection { get; private set; } = Vector3.down;   // 現在の重力方向（軸揃えされた単位ベクトル）
    public event System.Action<Vector3> OnGravityChanged;                   // 重力変更時に新方向を引数として発火するイベント

    // 購読解除のために参照を保持するデリゲート
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onLeftGravity;
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onRightGravity;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else Instance = this;

        ApplyGravity(GravityDirection);
    }

    /// <summary>
    /// 全オブジェクトの Awake 完了後に入力イベントを購読する。
    /// OnEnable ではなく Start を使うことで InputManager.Instance の初期化順を保証する。
    /// </summary>
    void Start()
    {
        _onLeftGravity = _ => ChangeGravity(-target.transform.right);
        _onRightGravity = _ => ChangeGravity(target.transform.right);

        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.LeftGravity.performed += _onLeftGravity;
        inputActions.Player.RightGravity.performed += _onRightGravity;
    }

    // シーン破棄時に入力イベントの購読を解除する
    void OnDestroy()
    {
        if (InputManager.Instance == null) return;
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.LeftGravity.performed -= _onLeftGravity;
        inputActions.Player.RightGravity.performed -= _onRightGravity;
    }

    /// <summary>
    /// 指定方向に最も近い軸揃えワールドベクトルへ重力を変更し、購読者へ通知する。
    /// </summary>
    public void ChangeGravity(Vector3 newDirection)
    {
        if (target == null) return;

        // ゲーム進行中のみ受け付ける
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
            return;

        // newDirection と内積が最大となる軸揃えベクトルを選択する
        // （斜め入力をいずれかの軸にスナップするため）
        float maxDot = 0.0f;
        Vector3 bestDirection = Vector3.up;
        var vectors = new Vector3[] { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
        for (int i = 0; i < vectors.Length; i++)
        {
            float dot = Vector3.Dot(vectors[i], newDirection);
            if (maxDot < dot)
            {
                maxDot = dot;
                bestDirection = vectors[i];
            }
        }

        // 重力を変更し、イベントの購読者に通知
        GravityDirection = bestDirection;
        ApplyGravity(GravityDirection);
        OnGravityChanged?.Invoke(GravityDirection);
    }

    // Physics.gravity に重力強度を乗じて適用する
    void ApplyGravity(Vector3 direction)
    {
        Physics.gravity = direction * gravityStrength;
    }
}
