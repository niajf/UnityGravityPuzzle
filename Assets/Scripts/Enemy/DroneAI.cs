using UnityEngine;

public class DroneAI : MonoBehaviour
{
    // AIの状態を定義（ステートマシンの基礎）
    private enum State
    {
        Idle,
        Chase
    }

    [Header("AI Settings")]
    [SerializeField] private State currentState = State.Idle;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("Sensor Settings")]
    [SerializeField] private float sightRadius = 10.0f; // 視界の届く距離
    [SerializeField] private float fieldOfView = 60.0f; // 視野角（前方〇度）

    private Transform playerTransform;

    void Start()
    {
        // プレイヤーをTagで探し出してキャッシュする
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null || GameFlowManager.Instance.CurrentState != GameFlowManager.GameState.Playing) return;

        // ステートマシンによる行動の分岐
        switch (currentState)
        {
            case State.Idle:
                // プレイヤーを発見したらChase状態へ移行
                if (CanSeePlayer())
                {
                    currentState = State.Chase;
                }
                break;

            case State.Chase:
                ChasePlayer();
                // 見失ったらIdle状態へ戻る
                if (!CanSeePlayer())
                {
                    currentState = State.Idle;
                }
                break;
        }
    }

    // プレイヤーが視界内にいるか（数学的アプローチと物理判定の融合）
    private bool CanSeePlayer()
    {
        // 1. 距離の判定
        Vector3 dirToPlayer = playerTransform.position - transform.position;
        if (dirToPlayer.magnitude > sightRadius) return false;

        // 2. 視野角の判定
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleToPlayer > fieldOfView * 0.5f) return false; // 視野角の半分より外なら見えない

        // 3. 障害物の判定
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, sightRadius))
        {
            // 当たったものがPlayerタグを持っていれば「視認」成功
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    private void ChasePlayer()
    {
        // プレイヤーの方向ベクトル
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;

        // 滑らかにプレイヤーの方を向く（Quaternion.Slerpの活用）
        Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 前方へ移動する
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    // エディタ上で視界を可視化する（エンジニアとしてのツール作成能力アピール）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius); // 視界の距離

        // 視野角のラインを描画
        Vector3 rightView = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * transform.forward;
        Vector3 leftView = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightView * sightRadius);
        Gizmos.DrawRay(transform.position, leftView * sightRadius);
    }
}