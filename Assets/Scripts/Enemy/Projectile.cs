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
        // 一定時間経過したら、オブジェクトをプールへ返却
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            ReturedPool(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤー以外との衝突ならば、オブジェクトを返却
        if (!other.CompareTag("AI") && !other.CompareTag("Stage"))
        {
            ReturedPool(gameObject);
        }
    }

    void ReturedPool(GameObject ob)
    {
        if (isReturned) return;  // 二重返却を防止
        isReturned = true;

        if (pool != null)
            pool.ReleaseObject(ob);
        else
            ob.SetActive(false);
    }
}