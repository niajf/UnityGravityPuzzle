using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 5.0f;

    private Rigidbody rb;
    private Vector3 targetUpVector = Vector3.up;
    private float inputH;
    private float inputV;
    private float inputMouseX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;
    }

    private void OnGravityChanged(Vector3 newGravityDir)
    {
        targetUpVector = -newGravityDir;
        rb.linearVelocity *= 0.5f;
    }

    void Update()
    {
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentState != GameFlowManager.GameState.Playing)
            return;

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        inputMouseX = Input.GetAxis("Mouse X");
    }

    void FixedUpdate()
    {
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentState != GameFlowManager.GameState.Playing)
            return;

        HandleRotation();
        HandleMovement();
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

        // マウスX入力でプレイヤーのローカルY軸を中心に水平回転
        if (Mathf.Abs(inputMouseX) > 0.01f)
        {
            gravityAligned = gravityAligned * Quaternion.Euler(0f, inputMouseX, 0f);
        }

        rb.MoveRotation(gravityAligned);
    }

    void HandleMovement()
    {
        Vector3 move = (transform.right * inputH + transform.forward * inputV) * moveSpeed;
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }

    void OnDestroy()
    {
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= OnGravityChanged;
        }
    }
}
