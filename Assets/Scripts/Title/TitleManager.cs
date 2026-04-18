using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// タイトルシーンのボタン操作とシーン遷移を管理するスクリプト。
/// 起動時のフェードイン、スタート・終了ボタンの非同期遷移処理を担う。
/// </summary>
public class TitleManager : MonoBehaviour
{
    [SerializeField] CanvasGroup fader;         // フェードイン・アウトを行う対象
    [SerializeField] int initialSceneIndex = 1; // 最初のゲームシーンのビルドインデックス

    bool isTransitioning = false; // 二重遷移を防ぐフラグ

    // シーン開始時にフェードインを再生する
    async void Start()
    {
        if (fader != null)
            await CanvasGroupFader.FadeAsync(fader, 0.0f, 1.0f, 1.0f, destroyCancellationToken);
    }

    /// <summary>Inspector のスタートボタンから呼び出す。</summary>
    public void OnStartButton()
    {
        OnStartButtonAsync().Forget();
    }

    // フェードアウト後にゲームシーンへ遷移する
    async UniTask OnStartButtonAsync()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (fader != null)
        {
            await CanvasGroupFader.FadeAsync(fader, 1.0f, 0.0f, 1.0f, destroyCancellationToken);
            await SceneManager.LoadSceneAsync(initialSceneIndex);
        }
    }

    /// <summary>Inspector の終了ボタンから呼び出す。</summary>
    public void OnExitButton()
    {
        OnExitButtonAsync().Forget();
    }

    // フェードアウト後にアプリを終了する（エディタ時は再生停止）
    async UniTask OnExitButtonAsync()
    {
        await CanvasGroupFader.FadeAsync(fader, 1.0f, 0.0f, 1.0f, destroyCancellationToken);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
