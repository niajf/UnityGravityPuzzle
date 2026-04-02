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
            createFunc: () => Instantiate(prefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    public void Release(GameObject obj)
    {
        pool.Release(obj);
    }
}