using UnityEngine;
using TMPro; // TextMeshProを扱うために必須

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gravityText;
    [SerializeField] private TextMeshProUGUI timeText;

    // Game Over 表示用パネル（エディタでアサイン）
    [SerializeField] private GameObject gameOverPanel;

    private bool gameOverShown = false;

    void Start()
    {
        // 2日目に作った「重力が変わった時」のイベントを購読する
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += UpdateGravityUI;
            // 初期状態の表示を更新
            UpdateGravityUI(GravityManager.Instance.GravityDirection);
        }

        // ゲームフローのイベントを購読
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred += ShowGameOverUI;
        }

        // 初期状態では非表示
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // 重力が変わった瞬間【だけ】呼ばれる（イベント駆動）
    private void UpdateGravityUI(Vector3 newGravityDir)
    {
        // ベクトルの向きに応じて文字列を変更
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
        // タイムは毎フレーム変わるのでUpdateで監視・更新する
        if (GameFlowManager.Instance != null && GameFlowManager.Instance.CurrentState == GameFlowManager.GameState.Playing)
        {
            // "F2" は小数点以下2桁まで表示するフォーマット
            timeText.text = $"Time: {GameFlowManager.Instance.CurrentTime:F2}";
        }
    }

    // ゲームオーバー時に呼び出されるUI表示処理
    private void ShowGameOverUI()
    {
        if (gameOverShown) return;
        gameOverShown = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // 必要なら他のUIも非表示にする
        // timeText.gameObject.SetActive(false);
        Debug.Log("UIManager: GameOver画面を表示");
    }

    // リトライボタンから呼ばせる
    public void OnRetryButton()
    {
        // ボタンを押したら即座にシーンをリロード
        GameFlowManager.Instance.RetrySceneRoutine();
    }

    void OnDestroy()
    {
        // メモリリーク防止のため、破棄時にイベント購読を解除
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= UpdateGravityUI;
        }
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred -= ShowGameOverUI;
        }
    }
}