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

    [Header("Fade Setting")]
    [SerializeField] float fadeInTime = 0.2f;
    [SerializeField] float fadeOutTime = 1.0f;


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

    void ShowGameOverPanel()
    {
        ShowGameOverPanelAsync().Forget();
    }

    async UniTask ShowGameOverPanelAsync()
    {
        if (HUDGroup != null) HUDGroup.alpha = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            await CanvasGroupFader.FadeAsync(gameOverPanel, 0.0f, 1.0f, fadeInTime);
        }
    }

    void ShowGameClearPanel()
    {
        ShowGameClearPanelAsync().Forget();
    }

    async UniTask ShowGameClearPanelAsync()
    {
        if (HUDGroup != null) HUDGroup.alpha = 0;

        if (clearTimeText != null && GameFlowManager.Instance != null)
        {
            clearTimeText.text = $"TIME:{GameFlowManager.Instance.CurrentTime:F2}";
        }

        if (gameClearPanel != null)
        {
            gameClearPanel.gameObject.SetActive(true);
            await CanvasGroupFader.FadeAsync(gameClearPanel, 0.0f, 1.0f, fadeInTime);
        }
    }

    // Inspectorのボタンから呼び出す
    public void OnRetryButtonClicked()
    {
        OnRetryButtonClickedAsync().Forget();
    }

    async UniTask OnRetryButtonClickedAsync()
    {
        await AllFadeOutAsync();
        GameFlowManager.Instance?.RetryScene();
    }

    public void OnNextButtonClicked()
    {
        OnNextButtonClickedAsycn().Forget();
    }

    async UniTask OnNextButtonClickedAsycn()
    {
        await AllFadeOutAsync();
        GameFlowManager.Instance?.LoadNextScene();
    }

    public void OnTitleButtonClicked()
    {
        OnTitleButtonClickedAsync().Forget();
    }

    async UniTask OnTitleButtonClickedAsync()
    {
        await AllFadeOutAsync();
        GameFlowManager.Instance?.BackTitleScene();
    }

    async UniTask AllFadeOutAsync()
    {
        await UniTask.WhenAll(
            CanvasGroupFader.FadeAsync(gameOverPanel, 1.0f, 0.0f, fadeOutTime),
            CanvasGroupFader.FadeAsync(gameClearPanel, 1.0f, 0.0f, fadeOutTime)
        );
    }
}
