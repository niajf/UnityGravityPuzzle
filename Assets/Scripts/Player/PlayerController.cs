using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerConfig config;

    Rigidbody rb;
    Vector3 targetUpVector = Vector3.up;    // 現在の上方向のベクトル

    Vector2 moveInput;
    Vector2 lookInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 重力変更のイベントを購読
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;
    }

    void OnEnable()
    {
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnDisable()
    {
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputActions.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled -= ctx => lookInput = Vector2.zero;
    }


    void Update()
    {
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
            return;

        // カーソル移動の合わせてプレイヤーを回転
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

    // 重力変更の際、自分自身の上方向を変更
    void OnGravityChanged(Vector3 newGravityDir)
    {
        targetUpVector = -newGravityDir;
        rb.linearVelocity *= 0.5f;
    }

    // クォータニオンによる姿勢制御
    void HandleRotation()
    {
        // 重力方向へ滑らかに姿勢を合わせる
        Quaternion gravityAligned = Quaternion.Slerp(
            rb.rotation,
            Quaternion.FromToRotation(transform.up, targetUpVector) * rb.rotation,
            config.rotateSpeed * Time.fixedDeltaTime
        );
        rb.MoveRotation(gravityAligned);
    }

    // 移動制御
    void HandleMovement()
    {
        // 移動方向のベクトルを計算
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y) * config.moveSpeed;

        // 座標を書き換え
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }

    // 破壊時にイベントの購読を解除
    void OnDestroy()
    {
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= OnGravityChanged;
        }
    }
}
