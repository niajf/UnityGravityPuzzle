using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] PlayerConfig config;

    public Transform target;   // 追従する対象

    // カメラの位置と回転情報
    float currentDistance;  // ターゲットとの距離

    // ズーム処理用フラグ
    bool isZoom = false;    // ズーム処理判定フラグ
    bool isZoomPrev = false;    // ズーム解除判定用フラグ

    // InputSystem
    PlayerInputActions inputActions;
    Vector2 lookInput;

    void Awake()
    {
        currentDistance = config.maxDistance;
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.ToggleZoom.performed += _ => isZoom = !isZoom;
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void LateUpdate()
    {

        // ズームを解除した場合、カメラを初期位置に戻す
        if (!isZoom && isZoomPrev)
        {
            transform.SetLocalPositionAndRotation(
                new Vector3(0f, 0f, -config.maxDistance),
                Quaternion.Euler(0f, 0f, 0f)
            );
        }

        isZoomPrev = isZoom;

        // ゲームオーバーやクリア時には操作を受け付けない
        if (GameFlowManager.Instance != null && !GameFlowManager.Instance.IsPlaying)
            return;

        // 追尾するターゲットが存在しない場合は処理を行わない
        if (target == null) return;

        // プレイヤーとの方向を計算
        Vector3 desiredDirection = transform.position - target.position;
        desiredDirection.Normalize();

        float targetDistance = config.maxDistance;

        // プレイヤーとの間にオブジェクトが存在しないかを確認
        if (Physics.SphereCast(target.position, config.collisionRadius, desiredDirection, out RaycastHit hit, config.maxDistance, config.obstacleMask))
        {
            targetDistance = Mathf.Clamp(hit.distance, config.minDistance, config.maxDistance);
        }

        // オブジェクトが存在する場合、カメラを寄せる
        if (targetDistance < currentDistance)
        {
            currentDistance = targetDistance;
        }

        // カメラを引くときは、滑らかに移動
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, config.smoothSpeed * Time.deltaTime);
        }

        if (isZoom)
            transform.position = target.position + transform.forward * (currentDistance - 0.5f);
        else
            transform.position = target.position + desiredDirection * (currentDistance - 0.5f);

        // マウスの移動量を取得
        if (Mathf.Abs(lookInput.y) > 0.01f)
        {

            // 回転軸はカメラ自身のX軸
            transform.RotateAround(target.transform.position, target.transform.right, -lookInput.y * config.sensitivity);
        }
    }
}