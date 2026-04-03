using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    ProjectilePool pool;
    float timer;

    public void Initialize(ProjectilePool pool)
    {
        this.pool = pool;
        timer = 0f;
    }

    void Update()
    {
        if (pool == null)
        {
            Debug.LogError($"[Projectile] pool is null on '{gameObject.name}'. Initialize() was not called.", gameObject);
            enabled = false;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            pool.ReleaseObject(gameObject);
            timer = 0f;
        }
    }
}