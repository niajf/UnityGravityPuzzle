using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;

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
    public async void OnStartButton()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (fader != null)
        {
            await CanvasGroupFader.FadeAsync(fader, 1.0f, 0.0f, 1.0f);
            await SceneManager.LoadSceneAsync(initialSceneIndex);
        }
    }

    // ゲーム終了処理
    public async void OnExitButton()
    {
        await CanvasGroupFader.FadeAsync(fader, 1.0f, 0.0f, 1.0f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
