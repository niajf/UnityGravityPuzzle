using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

/// <summary>
/// ゲームの進行状態（プレイ中・クリア・ゲームオーバー）とシーン遷移を一元管理するシングルトン。
/// 状態変化は OnGameOverOccurred / OnGameClearOccurred イベントで購読者へ通知する。
/// </summary>
public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }    // GameFlowManagerのシングルトン
    public float CurrentTime { get; private set; } = 0.0f;  // タイマー

    // 外部リスナー向けにゲームオーバーを通知するイベント
    public event System.Action OnGameOverOccurred;
    public event System.Action OnGameClearOccurred;
    public event System.Action OnPauseOccurred;

    [SerializeField] private int titleSceneIndex = 0;   // タイトルシーンのインデックス

    // ゲームの状態を列挙型で管理
    public enum GameState
    {
        Playing,
        Cleared,
        GameOver,
        Pause
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;    // 現在の状態を管理する変数

    // ゲームが進行中かを確認するヘルパープロパティ
    public bool IsPlaying => CurrentState == GameState.Playing;

    // ポーズ中かを確認するヘルパープロパティ
    public bool IsPause => CurrentState == GameState.Pause;

    void Awake()
    {
        // フレームレートを固定
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        // 既に存在している場合は自分を削除
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else Instance = this;
    }

    void OnEnable()
    {
        if (InputManager.Instance == null) return;
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.Pose.performed += Pose;
    }

    // 無効化時に入力イベントの購読を解除する
    void OnDisable()
    {
        if (InputManager.Instance == null) return;
        var inputActions = InputManager.Instance.Actions;
        inputActions.Player.Pose.performed -= Pose;
    }

    void Update()
    {
        // ゲーム進行中ならば、タイマーの時間を増やす
        if (Instance.IsPlaying)
        {
            CurrentTime += Time.deltaTime;
        }
    }
    // クリア処理
    public void StageClear()
    {
        // 既にクリア済み、またはゲームオーバーなら何もしない（多重判定防止）
        if (!Instance.IsPlaying) return;

        CurrentState = GameState.Cleared;

        // 購読者に通知（UIなど）
        OnGameClearOccurred?.Invoke();
    }

    // ゲームオーバー（落下死など）処理
    public void GameOver()
    {
        if (!Instance.IsPlaying) return;

        CurrentState = GameState.GameOver;

        OnGameOverOccurred?.Invoke();
    }

    // ポーズ処理
    void Pose(InputAction.CallbackContext _)
    {
        if (CurrentState != GameState.Pause)
            CurrentState = GameState.Pause;
        else
            CurrentState = GameState.Playing;

        OnPauseOccurred?.Invoke();
    }

    // 次のシーンを読み込む（最後のシーンならタイトルへ戻る）
    public async UniTask LoadNextScene()
    {
        // シーン読み込み（非同期）
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        nextSceneIndex = nextSceneIndex < SceneManager.sceneCountInBuildSettings
            ? nextSceneIndex
            : titleSceneIndex;

        await SceneManager.LoadSceneAsync(nextSceneIndex).WithCancellation(destroyCancellationToken);
    }

    // 現在のシーンを読み込み直す（リトライ）
    public async UniTask RetryScene()
    {
        // 現在のシーンを再読み込み
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex).WithCancellation(destroyCancellationToken);
    }

    // タイトルに戻る
    public async UniTask BackTitleScene()
    {
        await SceneManager.LoadSceneAsync(titleSceneIndex).WithCancellation(destroyCancellationToken);
    }
}