using UnityEngine;

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
            GameFlowManager.Instance.OnGameOverOccurred += playGameOver;
            GameFlowManager.Instance.OnGameClearOccurred += playGameClear;
        }

        // 重力が変化した際のイベントを購読
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += playGravityChange;
        }
    }

    // ゲームオーバーサウンドの再生
    public void playGameOver()
    {
        if (gameOver == null || audioSource == null) return;
        audioSource.PlayOneShot(gameOver);
    }

    // ゲームクリアサウンドの再生
    public void playGameClear()
    {
        if (gameClear == null || audioSource == null) return;
        audioSource.PlayOneShot(gameClear);
    }

    // 重力変更サウンドの再生
    public void playGravityChange(Vector3 _)
    {
        if (gravityChange == null || audioSource == null) return;
        audioSource.PlayOneShot(gravityChange);
    }

    // 自身が破壊される際にイベントの購読を解除
    void OnDestroy()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred -= playGameOver;
            GameFlowManager.Instance.OnGameClearOccurred -= playGameClear;
        }

        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= playGravityChange;
        }
    }
}
