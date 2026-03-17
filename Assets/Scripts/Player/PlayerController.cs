using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotateSpeed = 5.0f;


    Rigidbody rb;
    Vector3 targetUpVector = Vector3.up;    // 現在の上方向のベクトル
    float inputH;   // 水平方向の移動量
    float inputV;   // 垂直方向の移動量

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 重力変更のイベントを購読
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;
    }

    void Update()
    {
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentState != GameFlowManager.GameState.Playing)
            return;

        // 水平、垂直方向の移動力を取得
        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");

        // カーソル移動の合わせてプレイヤーを回転
        float mouseX = Input.GetAxis("Mouse X");
        if (Mathf.Abs(mouseX) > 0.01f)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX, 0f));
        }
    }

    void FixedUpdate()
    {
        // ゲーム進行中のみ実行
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentState != GameFlowManager.GameState.Playing)
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
        Vector3 move = (transform.right * inputH + transform.forward * inputV) * moveSpeed;

        // 座標を直接書き換え
        transform.position = transform.position + move * Time.fixedDeltaTime;
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
