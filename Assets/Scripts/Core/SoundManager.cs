using UnityEngine;

/// <summary>
/// ゲームイベントに応じた効果音の再生を管理するスクリプト。
/// GameFlowManager・GravityManager のイベントを購読し、対応する AudioClip を再生する。
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Sound files")]
    [SerializeField] AudioClip gameOver;
    [SerializeField] AudioClip gameClear;
    [SerializeField] AudioClip gravityChange;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // ゲームの状態が変化した際のイベントを購読
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred += PlayGameOver;
            GameFlowManager.Instance.OnGameClearOccurred += PlayGameClear;
        }

        // 重力が変化した際のイベントを購読
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += PlayGravityChange;
        }
    }

    // ゲームオーバーサウンドの再生
    public void PlayGameOver()
    {
        if (gameOver == null || audioSource == null) return;
        audioSource.PlayOneShot(gameOver);
    }

    // ゲームクリアサウンドの再生
    public void PlayGameClear()
    {
        if (gameClear == null || audioSource == null) return;
        audioSource.PlayOneShot(gameClear);
    }

    // 重力変更サウンドの再生
    public void PlayGravityChange(Vector3 _)
    {
        if (gravityChange == null || audioSource == null) return;
        audioSource.PlayOneShot(gravityChange);
    }

    // 自身が破壊される際にイベントの購読を解除
    void OnDestroy()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred -= PlayGameOver;
            GameFlowManager.Instance.OnGameClearOccurred -= PlayGameClear;
        }

        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= PlayGravityChange;
        }
    }
}
