using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 弾（Projectile）オブジェクトを Unity の ObjectPool で管理するスクリプト。
/// 生成・取得・返却を一元管理することで GC の発生を抑制する。
/// </summary>
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] GameObject prefab;          // プールするプレハブ
    [SerializeField] int defaultCapacity = 10;   // プールの初期確保数
    [SerializeField] int maxSize = 20;           // プールの上限数

    ObjectPool<GameObject> pool;

    void Awake()
    {
        // ObjectPool を初期化し、各コールバックで Active 状態を切り替える
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

    // プールに追加する弾オブジェクトを生成し、プールへの参照を渡す
    GameObject CreatePooledItem()
    {
        GameObject obj = Instantiate(prefab);
        obj.GetComponent<Projectile>().Initialize(this);
        return obj;
    }

    /// <summary>
    /// 指定の位置・回転でプールからオブジェクトを取得する。
    /// </summary>
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.GetComponent<Projectile>().Initialize(this);
        return obj;
    }

    /// <summary>
    /// 使い終わったオブジェクトをプールへ返却する。
    /// </summary>
    public void ReleaseObject(GameObject obj)
    {
        pool.Release(obj);
    }
}