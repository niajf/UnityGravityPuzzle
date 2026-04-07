using UnityEngine;

public class DroneAI : MonoBehaviour
{
    // AIの状態を定義（ステートマシンの基礎）
    private enum State
    {
        Idle,
        Chase
    }

    [SerializeField] DroneConfig config;

    [Header("Attack Settings")]
    [SerializeField] ProjectilePool projectilePool;

    Transform playerTransform;
    State currentState = State.Idle;
    float attackTimer;

    void Start()
    {
        // プレイヤーをTagで探し出してキャッシュする
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // タイマーの初期化
        attackTimer = 0f;
    }

    void Update()
    {
        if (playerTransform == null || !GameFlowManager.Instance.IsPlaying) return;

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

    // プレイヤーが視界内にいるか
    bool CanSeePlayer()
    {
        // 1. 距離の判定
        Vector3 dirToPlayer = playerTransform.position - transform.position;
        if (dirToPlayer.magnitude > config.sightRadius) return false;

        // 2. 視野角の判定
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
        if (angleToPlayer > config.fieldOfView * 0.5f) return false; // 視野角の半分より外なら見えない

        // 3. 障害物の判定
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, config.sightRadius))
        {
            // 当たったものがPlayerタグを持っていれば「視認」成功
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void ChasePlayer()
    {
        // プレイヤーの方向ベクトル
        Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;

        // 滑らかにプレイヤーの方を向く（Quaternion.Slerpの活用）
        Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, config.rotationSpeed * Time.deltaTime);

        // 前方へ移動する
        transform.position += transform.forward * config.moveSpeed * Time.deltaTime;

        // 弾の発射判定
        attackTimer += Time.deltaTime;
        if (attackTimer >= config.attackInterval)
        {
            attackTimer = 0f;
            FireProjectile();
        }
    }

    // エディタ上で視界を可視化する
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, config.sightRadius); // 視界の距離

        // 視野角のラインを描画
        Vector3 rightView = Quaternion.Euler(0, config.fieldOfView * 0.5f, 0) * transform.forward;
        Vector3 leftView = Quaternion.Euler(0, -config.fieldOfView * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightView * config.sightRadius);
        Gizmos.DrawRay(transform.position, leftView * config.sightRadius);
    }

    // 弾を発射するメソッド
    void FireProjectile()
    {
        Vector3 desiredDirection = playerTransform.position - transform.position;
        GameObject proj = projectilePool.GetObject(transform.position, Quaternion.LookRotation(desiredDirection, Vector3.right));
        proj.GetComponent<Rigidbody>().linearVelocity = transform.forward * config.projectileSpeed;
    }
}