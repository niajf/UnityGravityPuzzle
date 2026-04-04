using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] CanvasGroup HUDGroup;
    [SerializeField] TextMeshProUGUI gravityDirectionText;
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Panels")]
    [SerializeField] CanvasGroup gameOverPanel;
    [SerializeField] CanvasGroup gameClearPanel;

    [Header("Clear Panel")]
    [SerializeField] TextMeshProUGUI clearTimeText;

    void Start()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred += ShowGameOverPanel;
            GameFlowManager.Instance.OnGameClearOccurred += ShowGameClearPanel;
        }

        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += UpdateGravityText;
            UpdateGravityText(GravityManager.Instance.GravityDirection);
        }

        if (gameOverPanel != null) gameOverPanel.gameObject.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (timerText != null && GameFlowManager.Instance != null)
        {
            timerText.text = $"TIME: {GameFlowManager.Instance.CurrentTime:F2}";
        }
    }

    void OnDestroy()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred -= ShowGameOverPanel;
            GameFlowManager.Instance.OnGameClearOccurred -= ShowGameClearPanel;
        }

        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= UpdateGravityText;
        }
    }

    void UpdateGravityText(Vector3 direction)
    {
        if (gravityDirectionText == null) return;

        string label = direction switch
        {
            var d when d == Vector3.down => "DOWN",
            var d when d == Vector3.up => "UP",
            var d when d == Vector3.left => "LEFT",
            var d when d == Vector3.right => "RIGHT",
            var d when d == Vector3.forward => "FORWARD",
            var d when d == Vector3.back => "BACK",
            _ => direction.ToString()
        };

        gravityDirectionText.text = $"GRAVITY: {label}";
    }

    async void ShowGameOverPanel()
    {
        if (HUDGroup != null) HUDGroup.alpha = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            await CanvasGroupFader.FadeAsync(gameOverPanel, 1.0f, 0.0f, 1.0f);
        }
    }

    async void ShowGameClearPanel()
    {
        if (HUDGroup != null) HUDGroup.alpha = 0;

        if (clearTimeText != null && GameFlowManager.Instance != null)
        {
            clearTimeText.text = $"TIME:{GameFlowManager.Instance.CurrentTime:F2}";
        }

        if (gameClearPanel != null)
        {
            gameClearPanel.gameObject.SetActive(true);
            await CanvasGroupFader.FadeAsync(gameClearPanel, 0.0f, 1.0f, 1.0f);
        }
    }

    // Inspectorのボタンから呼び出す
    public async void OnRetryButtonClicked()
    {
        await AllFadeOutAsync();
        GameFlowManager.Instance?.RetryScene();
    }

    public async void OnNextButtonClicked()
    {
        await AllFadeOutAsync();
        GameFlowManager.Instance?.LoadNextScene();
    }

    public async void OnTitleButtonClicked()
    {
        await AllFadeOutAsync();
        GameFlowManager.Instance?.BackTitleScene();
    }

    async UniTask AllFadeOutAsync()
    {
        await CanvasGroupFader.FadeAsync(gameOverPanel, 1.0f, 0.0f, 1.0f);
        await CanvasGroupFader.FadeAsync(gameClearPanel, 1.0f, 0.0f, 1.0f);
    }
}
