using UnityEngine;

/// <summary>
/// プレイヤーを追従するサードパーソンカメラ。
/// SphereCast で障害物を検出してカメラ距離を動的に調整し、
/// マウス Y 軸入力で垂直方向の軌道を操作する。
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] PlayerConfig config;

    public Transform target;        // 追従する対象（プレイヤー）
    float currentDistance;          // ターゲットとの現在距離

    bool isZoom = false;            // ズーム中フラグ
    bool isZoomPrev = false;        // 前フレームのズーム状態（解除検出用）

    Vector2 lookInput;

    // 購読解除のために参照を保持するデリゲート
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onToggleZoom;
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onLookPerformed;
    System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _onLookCanceled;

    void Awake()
    {
        currentDistance = config.maxDistance;

        // 再有効化時も正しく購読解除できるようデリゲートを事前に生成する
        _onToggleZoom    = _ => isZoom = !isZoom;
        _onLookPerformed = ctx => lookInput = ctx.ReadValue<Vector2>();
        _onLookCanceled  = _ => lookInput = Vector2.zero;
    }

    // 有効化時に入力イベントを購読する
    void OnEnable()
    {
        if (InputManager.Instance == null) return;
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.ToggleZoom.performed += _onToggleZoom;
        inputActions.Player.Look.performed       += _onLookPerformed;
        inputActions.Player.Look.canceled        += _onLookCanceled;
    }

    // 無効化時に入力イベントの購読を解除する
    void OnDisable()
    {
        if (InputManager.Instance == null) return;
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.ToggleZoom.performed -= _onToggleZoom;
        inputActions.Player.Look.performed       -= _onLookPerformed;
        inputActions.Player.Look.canceled        -= _onLookCanceled;
    }

    void LateUpdate()
    {
        // ズームを解除した場合、カメラをデフォルト位置に即時リセットする
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

        if (target == null) return;

        // カメラからターゲットへの方向ベクトルを計算
        Vector3 desiredDirection = transform.position - target.position;
        desiredDirection.Normalize();

        float targetDistance = config.maxDistance;

        // SphereCast で障害物を検出し、衝突距離に応じてカメラを寄せる
        if (Physics.SphereCast(target.position, config.collisionRadius, desiredDirection, out RaycastHit hit, config.maxDistance, config.obstacleMask))
        {
            targetDistance = Mathf.Clamp(hit.distance, config.minDistance, config.maxDistance);
        }

        if (targetDistance < currentDistance)
        {
            // 障害物に近づく際は即時追従してクリッピングを防ぐ
            currentDistance = targetDistance;
        }
        else
        {
            // 引く際は Lerp で滑らかに復帰する
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, config.smoothSpeed * Time.deltaTime);
        }

        if (isZoom)
            transform.position = target.position + transform.forward * (currentDistance - 0.5f);
        else
            transform.position = target.position + desiredDirection * (currentDistance - 0.5f);

        // マウス Y 軸入力でカメラを垂直方向に回転する（回転軸はプレイヤーのローカル X 軸）
        if (Mathf.Abs(lookInput.y) > 0.01f)
        {
            transform.RotateAround(target.transform.position, target.transform.right, -lookInput.y * config.sensitivity);
        }
    }
}
