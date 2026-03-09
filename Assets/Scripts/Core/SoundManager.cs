using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sound files")]
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip gameClear;
    [SerializeField] private AudioClip gravityChange;

    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.OnGameOverOccurred += playGameOver;
            GameFlowManager.Instance.OnGameClearOccurred += playGameClear;
        }

        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged += playGravityChange;
        }
    }

    public void playGameOver()
    {
        if (gameOver == null || audioSource == null) return;
        audioSource.PlayOneShot(gameOver);
    }

    public void playGameClear()
    {
        if (gameClear == null || audioSource == null) return;
        audioSource.PlayOneShot(gameClear);
    }

    public void playGravityChange(Vector3 _)
    {
        if (gravityChange == null || audioSource == null) return;
        audioSource.PlayOneShot(gravityChange);
    }

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
