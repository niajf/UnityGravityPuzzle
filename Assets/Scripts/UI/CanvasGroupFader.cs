using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// CanvasGroup のアルファ値を非同期でアニメーションするユーティリティクラス。
/// フェードイン・フェードアウト処理を UniTask で await できる静的メソッドとして提供する。
/// </summary>
public class CanvasGroupFader : MonoBehaviour
{
    /// <summary>
    /// CanvasGroup のアルファ値を from から to へ duration 秒かけて補間する。
    /// </summary>
    /// <param name="group">対象の CanvasGroup</param>
    /// <param name="from">開始アルファ値</param>
    /// <param name="to">終了アルファ値</param>
    /// <param name="duration">フェードにかける時間（秒）</param>
    /// <param name="cancellationToken">キャンセルトークン（シーン破棄時に渡す）</param>
    public static async UniTask FadeAsync(CanvasGroup group, float from, float to, float duration, CancellationToken cancellationToken = default)
    {
        float elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            await UniTask.Yield(cancellationToken);
            if (group == null) return; // await 後に破棄されていた場合は即終了
        }

        if (group != null)
            group.alpha = to;
    }
}
