using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

/// <summary>
/// ゲーム中の HUD・ゲームオーバー／クリアパネルを管理する UI コントローラー。
/// GameFlowManager・GravityManager のイベントを購読し、状態に応じて UI を切り替える。
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] CanvasGroup HUDGroup;
    [SerializeField] TextMeshProUGUI gravityDirectionText;
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Panels")]
    [SerializeField] CanvasGroup gameOverPanel;
    [SerializeField] CanvasGroup gameClearPanel;
    [SerializeField] CanvasGroup missionPanel;
    [SerializeField] CanvasGroup pausePanel;

    [Header("Clear Text")]
    [SerializeField] TextMeshProUGUI clearTimeText;

    [Header("Fade Setting")]
    [SerializeField] float fadeInTime = 0.2f;
    [SerializeField] float fadeOutTime = 1.0f;


    void Start()
    {
        // ゲーム状態変化イベントを購読する
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred += ShowGameOverPanel;
            GameFlowManager.Instance.OnGameClearOccurred += ShowgameClearGroup;
            GameFlowManager.Instance.OnPauseOccurred += ControlePauseGroup;
        }

        // 重力変化イベントを購読し、初期表示を更新する
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += UpdateGravityText;
            UpdateGravityText(GravityManager.Instance.GravityDirection);
        }

        // 各パネルを非表示にして開始する
        if (gameOverPanel != null) gameOverPanel.gameObject.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.gameObject.SetActive(false);
        if (pausePanel != null) pausePanel.gameObject.SetActive(false);
        if (missionPanel != null) ShowMissionGroup().Forget();
    }

    void Update()
    {
        // 毎フレームタイマー表示を更新する
        if (timerText != null && GameFlowManager.Instance != null)
        {
            timerText.text = $"TIME: {GameFlowManager.Instance.CurrentTime:F2}";
        }
    }

    // 破棄時に購読しているイベントをすべて解除する
    void OnDestroy()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred -= ShowGameOverPanel;
            GameFlowManager.Instance.OnGameClearOccurred -= ShowgameClearGroup;
            GameFlowManager.Instance.OnPauseOccurred -= ControlePauseGroup;
        }

        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= UpdateGravityText;
        }
    }

    // 重力方向ベクトルを文字列ラベルに変換して HUD に反映する
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

    // UniTask の Forget を使い fire-and-forget で非同期処理を呼び出す
    void ShowGameOverPanel()
    {
        ShowGameOverPanelAsync().Forget();
    }

    // HUD を非表示にしてゲームオーバーパネルをフェードインする
    async UniTask ShowGameOverPanelAsync()
    {
        if (HUDGroup != null) HUDGroup.alpha = 0;

        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            await CanvasGroupFader.FadeAsync(gameOverPanel, 0.0f, 1.0f, fadeInTime, destroyCancellationToken);
        }
    }

    // UniTask の Forget を使い fire-and-forget で非同期処理を呼び出す
    void ShowgameClearGroup()
    {
        ShowgameClearGroupAsync().Forget();
    }

    // HUD を非表示にしてクリアタイムを表示後、クリアパネルをフェードインする
    async UniTask ShowgameClearGroupAsync()
    {
        if (HUDGroup != null) HUDGroup.alpha = 0;

        if (clearTimeText != null && GameFlowManager.Instance != null)
        {
            clearTimeText.text = $"TIME:{GameFlowManager.Instance.CurrentTime:F2}";
        }

        if (gameClearPanel != null)
        {
            gameClearPanel.gameObject.SetActive(true);
            await CanvasGroupFader.FadeAsync(gameClearPanel, 0.0f, 1.0f, fadeInTime, destroyCancellationToken);
        }
    }

    // UniTask の Forget を使い fire-and-forget で非同期処理を呼び出す
    void ControlePauseGroup()
    {
        if (GameFlowManager.Instance.IsPause)
            ShowPauseGroupAsync().Forget();
        else
            HidePauseGroupAsync().Forget();
    }

    // HUD を非表示にして、ポーズパネルをフェードインする
    async UniTask ShowPauseGroupAsync()
    {
        if (HUDGroup != null) HUDGroup.alpha = 0;

        if (pausePanel != null)
        {
            pausePanel.gameObject.SetActive(true);
            await CanvasGroupFader.FadeAsync(pausePanel, 0.0f, 1.0f, 0.2f, destroyCancellationToken);
        }
    }

    // HUD を非表示にしてク、ポーズパネルをフェードアウトする
    async UniTask HidePauseGroupAsync()
    {

        if (pausePanel != null)
        {
            await CanvasGroupFader.FadeAsync(pausePanel, 1.0f, 0.0f, 0.2f, destroyCancellationToken);
            pausePanel.gameObject.SetActive(false);
        }

        if (HUDGroup != null) HUDGroup.alpha = 1f;
    }

    // Mission UIの表示・非表示を行う
    async UniTask ShowMissionGroup()
    {
        // ゲーム開始時は表示しない
        missionPanel.gameObject.SetActive(false);

        // 指定時間後に表示
        await UniTask.Delay(500);
        missionPanel.gameObject.SetActive(true);
        await CanvasGroupFader.FadeAsync(missionPanel, 0.0f, 1.0f, 0.1f, destroyCancellationToken);

        // 指定時間後に非表示
        await UniTask.Delay(2000);
        await CanvasGroupFader.FadeAsync(missionPanel, 1.0f, 0.0f, 1.0f, destroyCancellationToken);
        missionPanel.gameObject.SetActive(false);
    }

    /// <summary>Inspector のリトライボタンから呼び出す。</summary>
    public void OnRetryButtonClicked()
    {
        OnRetryButtonClickedAsync().Forget();
    }

    async UniTask OnRetryButtonClickedAsync()
    {
        await AllFadeOutAsync();
        if (GameFlowManager.Instance != null)
            await GameFlowManager.Instance.RetryScene();
    }

    /// <summary>Inspector の次ステージボタンから呼び出す。</summary>
    public void OnNextButtonClicked()
    {
        OnNextButtonClickedAsycn().Forget();
    }

    async UniTask OnNextButtonClickedAsycn()
    {
        await AllFadeOutAsync();
        if (GameFlowManager.Instance != null)
            await GameFlowManager.Instance.LoadNextScene();
    }

    /// <summary>Inspector のタイトルボタンから呼び出す。</summary>
    public void OnTitleButtonClicked()
    {
        OnTitleButtonClickedAsync().Forget();
    }

    async UniTask OnTitleButtonClickedAsync()
    {
        await AllFadeOutAsync();
        if (GameFlowManager.Instance != null)
            await GameFlowManager.Instance.BackTitleScene();
    }

    // 全パネルを同時にフェードアウトしてシーン遷移に備える
    async UniTask AllFadeOutAsync()
    {
        await UniTask.WhenAll(
            CanvasGroupFader.FadeAsync(gameOverPanel, 1.0f, 0.0f, fadeOutTime, destroyCancellationToken),
            CanvasGroupFader.FadeAsync(gameClearPanel, 1.0f, 0.0f, fadeOutTime, destroyCancellationToken),
            CanvasGroupFader.FadeAsync(pausePanel, 1.0f, 0.0f, fadeOutTime, destroyCancellationToken)
        );
    }
}
