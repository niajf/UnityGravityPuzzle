using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5.0f; // 追従の遅延速度
    public float sensitivity = 10.0f;
    public Vector3 offset = new Vector3(0, 0, -5); // プレイヤー背後からのオフセット
    private Vector3 upVector = Vector3.up;

    private float rotationY = 0.0f;

    void Start()
    {
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;
    }

    void OnGravityChanged(Vector3 newGravityDir)
    {
        upVector = -newGravityDir;
    }


    void Update() // 物理演算に合わせてカメラも動かす
    {
        if (target == null) return;

        // 1. 位置の計算
        // ターゲットの「上」方向を基準に、オフセットをローカル座標として適用する
        // TransformPointは「ローカル座標」を「ワールド座標」に変換する関数
        // Vector3 desiredPosition = target.TransformPoint(offset);
        // Vector3 desiredPosition = target.position + offset;
        transform.position = target.TransformPoint(offset);

        // Lerp（線形補間）で滑らかに移動
        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        // transform.position = smoothedPosition;

        // マウスの移動量を取得
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;

        // 2. 回転の計算
        // カメラの上方向を、ターゲット（プレイヤー）の上方向に合わせる
        // LookAtの第2引数で「上方向」を指定できる
        Vector3 lookTarget = target.position;

        // // 滑らかに回転させるために、LookRotationとSlerpを使う
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position, upVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);

        // Y方向に一定量移動していれば縦回転
        if (Mathf.Abs(rotationY) > 0.01f)
        {
            // 回転軸はカメラ自身のX軸
            transform.RotateAround(target.transform.position, transform.right, -rotationY);
        }
    }
}