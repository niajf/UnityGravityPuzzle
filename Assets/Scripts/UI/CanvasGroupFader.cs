using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    [SerializeField] float fadeInDuration = 1.0f;

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        canvasGroup.alpha = 0f;
        StartCoroutine(DoFade(0f, 1f, fadeInDuration));
    }

    public IEnumerator FadeOutRoutine(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator DoFade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
