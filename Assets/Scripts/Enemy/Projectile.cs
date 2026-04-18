using UnityEngine;

/// <summary>
/// ドローンが発射する弾を制御するスクリプト。
/// 一定時間経過または衝突によって ProjectilePool へ返却される。
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] float lifetime = 3f; // プールへ返却されるまでの生存時間（秒）

    ProjectilePool pool;
    float timer;
    bool isReturned; // 二重返却を防止するフラグ

    /// <summary>プールから取得した際に呼ばれる初期化処理。</summary>
    public void Initialize(ProjectilePool pool)
    {
        this.pool = pool;
        timer = 0f;
        isReturned = false;
    }

    void Update()
    {
        // 一定時間経過したら、オブジェクトをプールへ返却
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            ReturnToPool(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤー以外との衝突ならば、オブジェクトを返却
        if (!other.CompareTag("AI") && !other.CompareTag("Stage"))
        {
            ReturnToPool(gameObject);
        }
    }

    void ReturnToPool(GameObject ob)
    {
        if (isReturned) return;  // 二重返却を防止
        isReturned = true;

        if (pool != null)
            pool.ReleaseObject(ob);
        else
            ob.SetActive(false);
    }
}