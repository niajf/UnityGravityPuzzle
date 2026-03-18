using UnityEngine;

/// <summary>
/// タイトルシーンの背景を動的生成するスクリプト。
/// 重力パズルのイメージに合わせ、宇宙空間を漂う幾何学オブジェクトを描画する。
/// </summary>
public class TitleBackground : MonoBehaviour
{
    [Header("Floating Objects")]
    [SerializeField] int objectCount = 30;
    [SerializeField] float spawnRadius = 18f;
    [SerializeField] float minScale = 0.2f;
    [SerializeField] float maxScale = 1.2f;

    [Header("Animation")]
    [SerializeField] float driftSpeed = 0.4f;
    [SerializeField] float rotationSpeed = 20f;

    [Header("Color Theme")]
    [SerializeField] Color primaryColor = new Color(0.15f, 0.4f, 0.9f);   // 青
    [SerializeField] Color secondaryColor = new Color(0.4f, 0.15f, 0.85f);  // 紫
    [SerializeField] Color accentColor = new Color(0.6f, 0.75f, 1.0f);   // 淡い青白

    // 各オブジェクトの漂い設定を保持する内部クラス
    class FloatingObject
    {
        public Transform transform;
        public Vector3 driftAxis;
        public float driftFrequency;
        public float driftPhase;
        public Vector3 rotationAxis;
        public float rotSpeed;
        public Vector3 basePosition;
    }

    FloatingObject[] floatingObjects;

    void Start()
    {
        floatingObjects = new FloatingObject[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            floatingObjects[i] = CreateFloatingObject(i);
        }
    }

    void Update()
    {
        if (floatingObjects == null) return;

        float t = Time.time;
        foreach (var obj in floatingObjects)
        {
            if (obj == null || obj.transform == null) continue;

            // 緩やかな漂い（サイン波で往復）
            Vector3 drift = obj.driftAxis * (Mathf.Sin(t * obj.driftFrequency + obj.driftPhase) * driftSpeed);
            obj.transform.position = obj.basePosition + drift;

            // ゆっくりした自転
            obj.transform.Rotate(obj.rotationAxis, obj.rotSpeed * Time.deltaTime, Space.Self);
        }
    }

    FloatingObject CreateFloatingObject(int index)
    {
        // キューブと球をランダムに選択
        GameObject go;
        if (Random.value > 0.35f)
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        else
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // コライダーは不要なので削除
        Destroy(go.GetComponent<Collider>());

        go.name = $"BG_Object_{index}";
        go.transform.SetParent(transform);

        // ランダムな位置（カメラの前方に球状に配置）
        Vector3 pos = Random.insideUnitSphere * spawnRadius;
        pos.z = Mathf.Abs(pos.z) + 5f; // カメラより前方に配置
        go.transform.position = pos;

        // ランダムなスケール
        float s = Random.Range(minScale, maxScale);
        go.transform.localScale = Vector3.one * s;

        // ランダムな初期回転
        go.transform.rotation = Random.rotation;

        // マテリアルの設定（プリミティブのデフォルトマテリアルを複製して使用）
        Renderer rend = go.GetComponent<Renderer>();
        Material mat = new Material(rend.sharedMaterial);

        // 3色からランダムに選択
        Color[] palette = { primaryColor, secondaryColor, accentColor };
        Color chosen = palette[Random.Range(0, palette.Length)];
        mat.color = chosen;

        rend.material = mat;

        // FloatingObject のデータ設定
        var fo = new FloatingObject
        {
            transform = go.transform,
            basePosition = pos,
            driftAxis = Random.onUnitSphere,
            driftFrequency = Random.Range(0.3f, 0.8f),
            driftPhase = Random.Range(0f, Mathf.PI * 2f),
            rotationAxis = Random.onUnitSphere,
            rotSpeed = Random.Range(-rotationSpeed, rotationSpeed)
        };

        return fo;
    }
}
