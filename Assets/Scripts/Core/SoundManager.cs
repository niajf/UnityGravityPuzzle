using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sound files")]
    [SerializeField] private AudioClip gameOver;
    [SerializeField] private AudioClip gameClear;

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
}
