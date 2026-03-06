using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private CanvasGroupFader fader;

    private bool isTransitioning = false;

    void Start()
    {
        if (fader != null)
            fader.FadeIn();
    }

    public void OnStartButton()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        if (fader != null)
            yield return fader.FadeOutRoutine(0.5f);

        SceneManager.LoadScene(1);
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
