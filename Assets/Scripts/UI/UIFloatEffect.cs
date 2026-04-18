using UnityEngine;

/// <summary>
/// UI 要素をサイン波で上下に浮かせるエフェクト。
/// タイトル画面のロゴなど、注目を引きたい要素に付与する。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIFloatEffect : MonoBehaviour
{
    [SerializeField] float amplitude = 10f; // 上下の振れ幅（ピクセル）
    [SerializeField] float speed = 1.2f;    // 浮遊速度（周波数）

    RectTransform rectTransform;
    Vector2 basePosition; // 開始時のアンカー位置（浮遊のゼロ点）

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // 初期位置を基準点として記憶する
        basePosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // サイン波で Y 座標をオフセットして浮遊を表現する
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        rectTransform.anchoredPosition = basePosition + new Vector2(0f, offset);
    }
}
