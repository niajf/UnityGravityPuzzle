using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int defaultCapacity = 10;
    [SerializeField] int maxSize = 20;

    ObjectPool<GameObject> pool;

    void Awake()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: CreatePooledItem,
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    GameObject CreatePooledItem()
    {
        GameObject obj = Instantiate(prefab);
        obj.GetComponent<Projectile>().Initialize(this);
        return obj;
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.GetComponent<Projectile>().Initialize(this);
        return obj;
    }

    public void ReleaseObject(GameObject obj)
    {
        pool.Release(obj);
    }
}