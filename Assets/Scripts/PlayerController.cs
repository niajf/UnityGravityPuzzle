using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 5.0f; // 姿勢制御の速度

    private Rigidbody rb;
    private Vector3 targetUpVector = Vector3.up; // 目標とする「上」方向

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // GravityManagerのイベントを購読する
        // 重力が変わったら OnGravityChanged が呼ばれる
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;
    }

    // イベントハンドラ
    private void OnGravityChanged(Vector3 newGravityDir)
    {
        // 重力の反対方向が、プレイヤーにとっての「上」
        targetUpVector = -newGravityDir;

        // 物理挙動のリセット（空中で回転すると変な慣性が残るため少し減衰させる）
        rb.linearVelocity *= 0.5f; // Unity 6ではvelocityではなくlinearVelocity推奨
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    // クォータニオンによる姿勢制御（ここが技術的アピールポイント）
    void HandleRotation()
    {
        // 現在の「上」方向から、目標の「上」方向への回転差分を計算
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, targetUpVector) * transform.rotation;

        // Slerp（球体線形補間）を使って滑らかに回転させる
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
    }

    void HandleMovement()
    {
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");

        // Vector3 movement = new Vector3(moveH, 0.0f, moveV);
        // rb.AddForce(movement * moveSpeed);

        // // プレイヤーの「右」と「前」方向に力を加える
        Vector3 force = (transform.right * moveH + transform.forward * moveV) * moveSpeed;
        rb.AddForce(force);
    }

    // オブジェクトが破棄されるときにイベント購読を解除（メモリリーク防止）
    void OnDestroy()
    {
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= OnGravityChanged;
        }
    }
}