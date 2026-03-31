using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotateSpeed = 5.0f;

    Rigidbody rb;
    Vector3 targetUpVector = Vector3.up;    // 現在の上方向のベクトル

    // InputSystem
    InputAction moveAction;
    InputAction lookAction;

    // 移動量、回転量
    Vector2 moveVector;
    Vector2 lookVector;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 重力変更のイベントを購読
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;

        //  Moveのリファレンスを探す
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
            return;

        // プレイヤーの移動量を取得
        moveVector = moveAction.ReadValue<Vector2>();

        // プレイヤーの回転量を取得
        lookVector = lookAction.ReadValue<Vector2>();

        // カーソル移動の合わせてプレイヤーを回転
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, lookVector.x, 0f));
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
            rotateSpeed * Time.fixedDeltaTime
        );
        rb.MoveRotation(gravityAligned);
    }

    // 移動制御
    void HandleMovement()
    {
        // 移動方向のベクトルを計算
        Vector3 move = (transform.right * moveVector.x + transform.forward * moveVector.y) * moveSpeed;

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
