using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0, 5, 0); // 開くときの移動量（上に5移動）
    public float openSpeed = 2f; // 開閉のスムーズさ

    private Vector3 closedPosition;
    private Vector3 targetPosition;

    void Start()
    {
        // 初期位置を「閉まった状態」として記憶
        closedPosition = transform.position;
        targetPosition = closedPosition;
    }

    void Update()
    {
        // 現在地から目標位置へ、Lerp（線形補間）で滑らかに移動させる
        // C++の競技プログラミングなどでは即値で動かしがちですが、
        // ゲームではTime.deltaTimeを使ったフレーム非依存の補間が重要です。
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);
    }

    // 外部から呼ばれる「開く」メソッド
    public void OpenDoor()
    {
        targetPosition = closedPosition + openOffset;
    }

    // 外部から呼ばれる「閉まる」メソッド
    public void CloseDoor()
    {
        targetPosition = closedPosition;
    }
}