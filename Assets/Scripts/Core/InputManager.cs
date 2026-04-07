using UnityEngine;

/// <summary>
/// 入力アクションを一元管理するシングルトン。
/// PlayerInputActions のライフサイクル（生成・有効化・破棄）を担い、
/// 他スクリプトは Instance.Actions 経由でアクションを取得する。
/// </summary>
[DefaultExecutionOrder(-6)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerInputActions Actions { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // アクションマップを生成し、Player マップを有効化する
        Actions = new PlayerInputActions();
        Actions.Player.Enable();
    }

    // シーン破棄時にアクションマップを無効化・解放する
    void OnDestroy()
    {
        Actions.Player.Disable();
        Actions.Dispose();
    }
}
