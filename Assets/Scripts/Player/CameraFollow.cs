using NUnit.Framework;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 5.0f; // 追従の遅延速度
    public float sensitivity = 1.0f;

    public LayerMask obstacleMask;
    public float maxDistance = 5.0f;
    public float minDistance = 0.5f;
    public float collisionRadius = 0.2f;

    private float currentDistance;
    private float rotationY = 0.0f;

    private bool isZoom = false;
    private bool isZoomPrev = false;

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

        if (!isZoom && isZoomPrev)
        {
            // transform.position = target.position - target.transform.forward * currentDistance;
            // transform.rotation = target.transform.rotation;

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
            // transform.RotateAround(target.transform.position, transform.right, -rotationY);
            transform.RotateAround(target.transform.position, target.transform.right, -rotationY);
        }
    }
}