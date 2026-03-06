using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIFloatEffect : MonoBehaviour
{
    [SerializeField] private float amplitude = 10f;
    [SerializeField] private float speed = 1.2f;

    private RectTransform rectTransform;
    private Vector2 basePosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        basePosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        rectTransform.anchoredPosition = basePosition + new Vector2(0f, offset);
    }
}
