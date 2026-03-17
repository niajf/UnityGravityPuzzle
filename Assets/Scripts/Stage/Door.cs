using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip openDoor;
    [SerializeField] private AudioClip closeDoor;

    [Header("Property")]
    [SerializeField] Vector3 openOffset = new Vector3(0, 5, 0); // 開くときの移動量
    [SerializeField] float openSpeed = 2f; // 開閉の速度

    private AudioSource audioSource;

    Vector3 closedPosition; // 閉じたときの位置
    Vector3 targetPosition; // 開いたときの位置

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 初期位置を「閉まった状態」として記憶
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    void Update()
    {
        // 現在地から目標位置へで滑らかに移動させる
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
    }

    // 開くメソッド
    public void OpenDoor()
    {
        targetPosition = closedPosition + openOffset;
        audioSource.PlayOneShot(openDoor);
    }

    // 閉まるメソッド
    public void CloseDoor()
    {
        targetPosition = closedPosition;
        audioSource.PlayOneShot(closeDoor);
    }
}