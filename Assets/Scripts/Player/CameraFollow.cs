using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;   // 追従する対象

    [Header("Property")]
    [SerializeField] float smoothSpeed = 5.0f;  // 追従の遅延速度
    [SerializeField] float sensitivity = 1.0f;  // 感度
    [SerializeField] float maxDistance = 5.0f;  // ターゲットとの最大距離
    [SerializeField] float minDistance = 0.5f;  // ターゲットとの最小距離
    [SerializeField] float collisionRadius = 0.2f;  //spherecastの球の半径
    [SerializeField] LayerMask obstacleMask;    // 衝突判定を行わないオブジェクト

    // カメラの位置と回転情報
    float currentDistance;  // ターゲットとの距離
    float rotationY = 0.0f; // ターゲットを中心とした回転角

    // ズーム処理用フラグ
    bool isZoom = false;    // ズーム処理判定フラグ
    bool isZoomPrev = false;    // ズーム解除判定用フラグ

    void Start()
    {
        currentDistance = maxDistance;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
            isZoom = !isZoom;
    }

    void LateUpdate()
    {

        // ズームを解除した場合、カメラを初期位置に戻す
        if (!isZoom && isZoomPrev)
        {
            transform.SetLocalPositionAndRotation(
                new Vector3(0f, 0f, -maxDistance),
                Quaternion.Euler(0f, 0f, 0f)
            );
        }

        isZoomPrev = isZoom;

        // ゲームオーバーやクリア時には操作を受け付けない
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentState != GameFlowManager.GameState.Playing)
            return;

        // 追尾するターゲットが存在しない場合は処理を行わない
        if (target == null) return;

        // プレイヤーとの方向を計算
        Vector3 desiredDirection = transform.position - target.position;
        desiredDirection.Normalize();

        float targetDistance = maxDistance;

        // プレイヤーとの間にオブジェクトが存在しないかを確認
        if (Physics.SphereCast(target.position, collisionRadius, desiredDirection, out RaycastHit hit, maxDistance, obstacleMask))
        {
            targetDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        // オブジェクトが存在する場合、カメラを寄せる
        if (targetDistance < currentDistance)
        {
            currentDistance = targetDistance;
        }

        // カメラを引くときは、滑らかに移動
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, smoothSpeed * Time.deltaTime);
        }

        if (isZoom)
            transform.position = target.position + transform.forward * (currentDistance - 0.5f);
        else
            transform.position = target.position + desiredDirection * (currentDistance - 0.5f);

        // マウスの移動量を取得
        rotationY = Input.GetAxis("Mouse Y") * sensitivity;

        // Y方向に一定量移動していれば縦回転
        if (Mathf.Abs(rotationY) > 0.01f)
        {
            // 回転軸はカメラ自身のX軸
            transform.RotateAround(target.transform.position, target.transform.right, -rotationY);
        }
    }
}