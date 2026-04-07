using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    ProjectilePool pool;
    float timer;
    bool isReturned;

    public void Initialize(ProjectilePool pool)
    {
        this.pool = pool;
        timer = 0f;
        isReturned = false;
    }

    void Update()
    {
        if (pool == null) return;
        if (isReturned) return;

        // 一定時間経過したら、オブジェクトをプールへ返却
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            ReleaseObject(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤー意外との衝突ならば、オブジェクトを返却
        if (!other.CompareTag("Player"))
        {
            ReleaseObject(gameObject);
        }
    }

    void ReleaseObject(GameObject ob)
    {
        isReturned = true;
        timer = 0f;
        pool.ReleaseObject(ob);
    }
}