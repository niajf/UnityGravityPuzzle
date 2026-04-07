using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの移動・回転を制御する。
/// GravityManager の OnGravityChanged イベントを購読し、
/// 重力変更に応じて Quaternion.Slerp で姿勢を滑らかに補正する。
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerConfig config;

    Rigidbody rb;
    Vector3 targetUpVector = Vector3.up;    // 重力逆方向の目標上ベクトル（姿勢補正の目標値）

    Vector2 moveInput;
    Vector2 lookInput;

    // 購読解除のために参照を保持するデリゲート
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onMovePerformed;
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onMoveCanceled;
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onLookPerformed;
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onLookCanceled;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 重力変更イベントを購読する
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;

        // 再有効化時も正しく購読解除できるようデリゲートを事前に生成する
        _onMovePerformed = ctx => moveInput = ctx.ReadValue<Vector2>();
        _onMoveCanceled  = _ => moveInput = Vector2.zero;
        _onLookPerformed = ctx => lookInput = ctx.ReadValue<Vector2>();
        _onLookCanceled  = _ => lookInput = Vector2.zero;
    }

    // 有効化時に入力イベントを購読する
    void OnEnable()
    {
        if (InputManager.Instance == null) return;
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.Move.performed += _onMovePerformed;
        inputActions.Player.Move.canceled  += _onMoveCanceled;
        inputActions.Player.Look.performed += _onLookPerformed;
        inputActions.Player.Look.canceled  += _onLookCanceled;
    }

    // 無効化時に入力イベントの購読を解除する
    void OnDisable()
    {
        if (InputManager.Instance == null) return;
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.Move.performed -= _onMovePerformed;
        inputActions.Player.Move.canceled  -= _onMoveCanceled;
        inputActions.Player.Look.performed -= _onLookPerformed;
        inputActions.Player.Look.canceled  -= _onLookCanceled;
    }

    void Update()
    {
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
            return;

        // マウス X 軸入力でプレイヤーをローカル Y 軸回りに回転する
        if (Mathf.Abs(lookInput.x) > 0.01f)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, lookInput.x * config.sensitivity, 0f));
        }
    }

    void FixedUpdate()
    {
        // ゲーム進行中のみ実行
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
            return;

        HandleRotation();
        HandleMovement();
    }

    // 重力変更時に目標上ベクトルを更新し、速度を減衰させる
    void OnGravityChanged(Vector3 newGravityDir)
    {
        targetUpVector = -newGravityDir;
        rb.linearVelocity *= 0.5f;  // 重力反転による跳ね返りを抑制
    }

    // Quaternion.Slerp でプレイヤーの上方向を重力逆方向へ滑らかに補正する
    void HandleRotation()
    {
        Quaternion gravityAligned = Quaternion.Slerp(
            rb.rotation,
            Quaternion.FromToRotation(transform.up, targetUpVector) * rb.rotation,
            config.rotateSpeed * Time.fixedDeltaTime
        );
        rb.MoveRotation(gravityAligned);
    }

    // 移動入力をワールド空間ベクトルに変換して位置を更新する
    void HandleMovement()
    {
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * config.moveSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }

    // シーン破棄時にイベントとデリゲートの購読を解除する
    void OnDestroy()
    {
        if (GravityManager.Instance != null)
            GravityManager.Instance.OnGravityChanged -= OnGravityChanged;
    }
}
