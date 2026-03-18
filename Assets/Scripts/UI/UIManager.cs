using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI gravityDirectionText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;

    [Header("Clear Panel")]
    [SerializeField] private TextMeshProUGUI clearTimeText;

    [Header("Instruction Panel")]
    [SerializeField] private CanvasGroup instructionGroup;

    private void Start()
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

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.SetActive(false);
    }

    private void Update()
    {
        if (timerText != null && GameFlowManager.Instance != null)
        {
            timerText.text = $"TIME: {GameFlowManager.Instance.CurrentTime:F2}";
        }
    }

    private void OnDestroy()
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

    private void UpdateGravityText(Vector3 direction)
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

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (instructionGroup != null) instructionGroup.alpha = 0;
    }

    private void ShowGameClearPanel()
    {
        if (gameClearPanel != null) gameClearPanel.SetActive(true);
        if (instructionGroup != null) instructionGroup.alpha = 0;

        if (clearTimeText != null && GameFlowManager.Instance != null)
        {
            clearTimeText.text = $"TIME:{GameFlowManager.Instance.CurrentTime:F2}";
        }
    }

    // Inspectorのボタンから呼び出す
    public void OnRetryButtonClicked()
    {
        GameFlowManager.Instance?.RetryScene();
    }

    public void OnNextButtonClicked()
    {
        GameFlowManager.Instance?.LoadNextScene();
    }

    public void OnTitleButtonClicked()
    {
        GameFlowManager.Instance?.backTitleScene();
    }
}
