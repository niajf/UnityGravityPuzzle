using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CanvasGroupFader : MonoBehaviour
{
    public static async UniTask FadeAsync(CanvasGroup group, float from, float to, float duration, CancellationToken cancellationToken = default)
    {
        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            await UniTask.Yield(cancellationToken);
        }

        group.alpha = to;
    }
}
