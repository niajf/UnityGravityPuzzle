using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

/// <summary>
/// ボタンを囲む矩形の枠線を表示するスクリプト。
/// 上下左右に細い Image を動的生成してボーダーを描画する。
/// </summary>
[RequireComponent(typeof(Image))]
public class ButtonOutlineEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Border Colors")]
    [SerializeField] Color normalColor = new Color(0f, 0.9f, 1f, 0.5f);  // 通常：薄いシアン
    [SerializeField] Color hoverColor = new Color(0f, 0.9f, 1f, 1f);    // ホバー：シアン
    [SerializeField] Color pressedColor = new Color(0f, 0.5f, 0.7f, 1f);  // 押下：暗いシアン

    [Header("Border Thickness")]
    [SerializeField] float normalThickness = 5f;
    [SerializeField] float hoverThickness = 8f;

    [Header("Text Colors")]
    [SerializeField] Color textNormalColor = new Color(0.82f, 0.95f, 1f);
    [SerializeField] Color textHoverColor = new Color(0f, 0.9f, 1f);

    [Header("Transition")]
    [SerializeField] float transitionDuration = 0.15f;

    // 上下左右の枠線 Image
    Image borderTop, borderBottom, borderLeft, borderRight;
    TextMeshProUGUI label;
    Coroutine coroutine;

    void Awake()
    {
        // ボタン本体の背景は透明にする
        GetComponent<Image>().color = Color.clear;

        // 枠の生成
        CreateBorders();

        label = GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.color = textNormalColor;
    }

    // ボタンの枠を生成するメソッド
    void CreateBorders()
    {
        borderTop = CreateEdge("Border_Top");
        borderBottom = CreateEdge("Border_Bottom");
        borderLeft = CreateEdge("Border_Left");
        borderRight = CreateEdge("Border_Right");

        ApplyBorderLayout(normalThickness);
        SetBorderColor(normalColor);
    }

    //　枠線1辺分の Image オブジェクトを生成する
    Image CreateEdge(string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        // クリック判定に干渉しないよう raycast を無効化
        var img = go.GetComponent<Image>();
        img.raycastTarget = false;
        img.color = normalColor;

        return img;
    }

    //ボタンの RectTransform に合わせて4辺を配置する
    void ApplyBorderLayout(float thickness)
    {
        SetEdgeRect(borderTop, new Vector2(0, 1), new Vector2(1, 1),
                    new Vector2(0, -thickness), new Vector2(0, 0));

        SetEdgeRect(borderBottom, new Vector2(0, 0), new Vector2(1, 0),
                    new Vector2(0, 0), new Vector2(0, thickness));

        SetEdgeRect(borderLeft, new Vector2(0, 0), new Vector2(0, 1),
                    new Vector2(0, 0), new Vector2(thickness, 0));

        SetEdgeRect(borderRight, new Vector2(1, 0), new Vector2(1, 1),
                    new Vector2(-thickness, 0), new Vector2(0, 0));
    }

    // 辺の配置
    void SetEdgeRect(Image img,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 offsetMax)
    {
        var rt = img.rectTransform;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    // 枠の色の設定
    void SetBorderColor(Color color)
    {
        borderTop.color = color;
        borderBottom.color = color;
        borderLeft.color = color;
        borderRight.color = color;
    }

    // ポインターイベント
    public void OnPointerEnter(PointerEventData e) =>
        Transition(hoverColor, hoverThickness, textHoverColor);

    public void OnPointerExit(PointerEventData e) =>
        Transition(normalColor, normalThickness, textNormalColor);

    public void OnPointerDown(PointerEventData e) =>
        Transition(pressedColor, normalThickness, textNormalColor);

    public void OnPointerUp(PointerEventData e)
    {
        bool over = RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(), e.position, e.pressEventCamera);
        Transition(
            over ? hoverColor : normalColor,
            over ? hoverThickness : normalThickness,
            over ? textHoverColor : textNormalColor);
    }

    // トランジション
    void Transition(Color col, float thickness, Color textCol)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(DoTransition(col, thickness, textCol));
    }

    IEnumerator DoTransition(Color targetCol, float targetThickness, Color targetText)
    {
        Color startCol = borderTop.color;
        float startThickness = borderTop.rectTransform.offsetMax.x != 0
                               ? Mathf.Abs(borderTop.rectTransform.offsetMax.x)
                               : Mathf.Abs(borderTop.rectTransform.offsetMin.y);
        Color startText = label != null ? label.color : Color.white;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);

            Color c = Color.Lerp(startCol, targetCol, t);
            float th = Mathf.Lerp(startThickness, targetThickness, t);

            SetBorderColor(c);
            ApplyBorderLayout(th);
            if (label != null) label.color = Color.Lerp(startText, targetText, t);

            yield return null;
        }

        SetBorderColor(targetCol);
        ApplyBorderLayout(targetThickness);
        if (label != null) label.color = targetText;
    }
}
