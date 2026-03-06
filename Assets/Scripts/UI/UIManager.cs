using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gravityText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroupFader gameOverFader;

    [Header("Game Clear Panel")]
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private CanvasGroupFader gameClearFader;

    private bool gameOverShown = false;
    private bool gameClearShown = false;

    void Start()
    {
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += UpdateGravityUI;
            UpdateGravityUI(GravityManager.Instance.GravityDirection);
        }

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred += ShowGameOverUI;
            GameFlowManager.Instance.OnGameClearOccurred += ShowGameClearUI;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);
    }

    private void UpdateGravityUI(Vector3 newGravityDir)
    {
        string dirName = "Unknown";
        if (newGravityDir == Vector3.down) dirName = "Down";
        else if (newGravityDir == Vector3.up) dirName = "Up";
        else if (newGravityDir == Vector3.left) dirName = "Left";
        else if (newGravityDir == Vector3.right) dirName = "Right";
        else if (newGravityDir == Vector3.forward) dirName = "Forward";
        else if (newGravityDir == Vector3.back) dirName = "Back";

        gravityText.text = $"Gravity: {dirName}";
    }

    void Update()
    {
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentState == GameFlowManager.GameState.Playing)
        {
            timeText.text = $"Time: {GameFlowManager.Instance.CurrentTime:F2}";
        }
    }

    private void ShowGameOverUI()
    {
        if (gameOverShown) return;
        gameOverShown = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverFader != null)
            gameOverFader.FadeIn();

        Debug.Log("UIManager: GameOver画面を表示");
    }

    private void ShowGameClearUI()
    {
        if (gameClearShown) return;
        gameClearShown = true;

        if (gameClearPanel != null)
            gameClearPanel.SetActive(true);

        if (gameClearFader != null)
            gameClearFader.FadeIn();

        Debug.Log("UIManager: GameClear画面を表示");
    }

    public void OnRetryButton()
    {
        GameFlowManager.Instance.RetrySceneRoutine();
    }

    public void OnExitButton()
    {
        GameFlowManager.Instance.backTitleScene();
    }

    void OnDestroy()
    {
        if (GravityManager.Instance != null)
            GravityManager.Instance.OnGravityChanged -= UpdateGravityUI;

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred -= ShowGameOverUI;
            GameFlowManager.Instance.OnGameClearOccurred -= ShowGameClearUI;
        }
    }
}
