using UnityEngine;
using UnityEngine.SceneManagement;  // シーン遷移に必須
using Cysharp.Threading.Tasks;      // 非同期処理用
using System.Threading;
using System.Threading.Tasks;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }    // GameFlowManagerのシングルトン
    public float CurrentTime { get; private set; } = 0.0f;  // タイマー

    // 外部リスナー向けにゲームオーバーを通知するイベント
    public event System.Action OnGameOverOccurred;
    public event System.Action OnGameClearOccurred;

    [SerializeField] private int titleSceneIndex = 0;   // タイトルシーンのインデックス

    // ゲームの状態を列挙型で管理
    public enum GameState
    {
        Playing,
        Cleared,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;    // 現在の状態を管理する変数

    // ゲームが進行中かを確認するヘルパープロパティ
    public bool IsPlaying => CurrentState == GameState.Playing;

    CancellationTokenSource cts;

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

        cts = new CancellationTokenSource();
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

    // 次のシーンを読むこむ
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

    void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}