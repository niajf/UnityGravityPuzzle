using UnityEngine;

/// <summary>
/// プレイヤーの移動・カメラ設定を一元管理する ScriptableObject。
/// Inspector から値を調整することでコードを変更せずにパラメータチューニングできる。
/// </summary>
[CreateAssetMenu(fileName = "PlayerConfig", menuName = "GravityPuzzle/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Property")]
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 5.0f;

    [Header("Camera")]
    public float smoothSpeed = 5.0f;  // 追従の遅延速度
    public float sensitivity = 0.2f;  // 感度
    public float maxDistance = 5.0f;  // ターゲットとの最大距離
    public float minDistance = 0.5f;  // ターゲットとの最小距離
    public float collisionRadius = 0.2f;  //spherecastの球の半径
    public LayerMask obstacleMask;    // 衝突判定を行わないオブジェクト
}
