using UnityEngine;
using UnityEngine.SceneManagement; // シーン遷移に必須
using System.Collections;          // コルーチン(IEnumerator)に必須

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }
    public float CurrentTime { get; private set; } = 0.0f;

    // 外部リスナー向けにゲームオーバーを通知するイベント
    public event System.Action OnGameOverOccurred;

    // ゲームの状態を列挙型で管理（ステートマシンの基礎）
    public enum GameState
    {
        Playing,
        Cleared,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;

    private void Awake()
    {
        // 既に存在している場合は自分を削除
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else Instance = this;

        Debug.Log("GameFlowManager initialized.");
    }

    void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentTime += Time.deltaTime;
        }
    }
    // クリア処理
    public void StageClear()
    {
        // 既にクリア済み、またはゲームオーバーなら何もしない（多重判定防止）
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Cleared;
        Debug.Log("Stage Clear!! 次のステージへ遷移します...");

        // クリアタイムをログに出力
        Debug.Log($"Stage Clear!! タイム: {CurrentTime:F2}秒");

        // コルーチンを開始して、遅延処理を行う
        StartCoroutine(LoadNextSceneRoutine());
    }

    // ゲームオーバー（落下死など）処理
    public void GameOver()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.GameOver;
        Debug.Log("Game Over... リトライします。");

        // 購読者に通知（UIなど）
        OnGameOverOccurred?.Invoke();
    }

    // 次のシーンを読み込むコルーチン
    public IEnumerator LoadNextSceneRoutine()
    {
        // 2秒間待機する（この間にクリア演出やSEを鳴らす）
        yield return new WaitForSeconds(2.0f);

        // 現在のシーンのインデックス番号を取得し、+1する
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // 次のシーンが登録されているかチェック
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("全ステージクリア！タイトルに戻るなどの処理をここに書く");
            // SceneManager.LoadScene(0); // 仮に0番(タイトル)に戻す場合
        }
    }

    // 現在のシーンを読み込み直す（リトライ）コルーチン
    public void RetrySceneRoutine()
    {
        // 現在のシーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}