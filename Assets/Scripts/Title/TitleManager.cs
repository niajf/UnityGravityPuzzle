using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [SerializeField] CanvasGroupFader fader;    //フェードインを行う対象
    [SerializeField] int initialSceneIndex = 1; // 最初のスタートインデックス

    bool isTransitioning = false;

    void Start()
    {
        if (fader != null)
            fader.FadeIn();
    }

    // スタートボタンが押された際の処理
    public void OnStartButton()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(LoadGameScene());
    }

    // 最初のステージを読み込むコルーチン
    IEnumerator LoadGameScene()
    {
        if (fader != null)
            yield return fader.FadeOutRoutine(0.5f);

        SceneManager.LoadScene(initialSceneIndex);
    }

    // ゲーム終了処理
    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
