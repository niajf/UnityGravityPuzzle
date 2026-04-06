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

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            isReturned = true;
            pool.ReleaseObject(gameObject);
            timer = 0f;
        }
    }
}