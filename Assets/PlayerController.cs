using UnityEngine;
using System.Collections;

// MonoBehaviourはUnityの基底クラス。これを継承しないとGameObjectにアタッチできません。
public class PlayerController : MonoBehaviour
{
    // [SerializeField]をつけると、private変数でもUnityのInspectorから数値を調整できます。
    // コンパイル不要で調整できるので、開発効率が劇的に上がります。
    [SerializeField] private float moveSpeed = 5.0f;

    private Rigidbody rb;

    // Start() : C++のコンストラクタのようなもの。ゲーム開始時(または生成時)に1回呼ばれます。
    void Start()
    {
        // 自分のGameObjectについているRigidbodyコンポーネントを取得してキャッシュする
        // (毎回GetComponentすると重いため)
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate() : 物理演算の計算周期に合わせて呼ばれるループ。
    // 通常のUpdate()はフレームレート依存なので、物理挙動にはFixedUpdateを使います。
    void FixedUpdate()
    {
        // 入力の取得 (-1.0 ~ 1.0 の値を返す)
        // 矢印キーやWASDに対応しています。
        float moveH = Input.GetAxis("Horizontal"); // A/D または ←/→
        float moveV = Input.GetAxis("Vertical");   // W/S または ↑/↓

        // 力のベクトルを作成 (X, Y, Z)
        Vector3 movement = new Vector3(moveH, 0.0f, moveV);

        // Rigidbodyに力を加える
        // ForceMode.Force: 継続的な力を加える（車やロケットなど）
        // ForceMode.VelocityChange: 瞬時に速度を変える（キビキビ動くアクション向け）
        // 今回は物理パズルらしく慣性を残したいのでAddForceを使います。
        rb.AddForce(movement * moveSpeed);
    }
}