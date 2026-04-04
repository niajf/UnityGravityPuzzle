using UnityEngine;
using Cysharp.Threading.Tasks;

public class CanvasGroupFader : MonoBehaviour
{
    public static async UniTask FadeAsync(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            await UniTask.Yield();
        }

        group.alpha = to;
    }
}
