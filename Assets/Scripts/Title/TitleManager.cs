using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class TitleManager : MonoBehaviour
{
    [SerializeField] CanvasGroup fader;    //フェードインを行う対象
    [SerializeField] int initialSceneIndex = 1; // 最初のスタートインデックス

    bool isTransitioning = false;

    async void Start()
    {
        if (fader != null)
            await CanvasGroupFader.FadeAsync(fader, 0.0f, 1.0f, 1.0f);
    }

    // スタートボタンが押された際の処理
    public void OnStartButton()
    {
        OnStartButtonAsync().Forget();
    }

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

    // ゲーム終了処理
    public void OnExitButton()
    {
        OnExitButtonAsync().Forget();
    }

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
