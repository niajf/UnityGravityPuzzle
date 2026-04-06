using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }
    public PlayerInputActions Actions { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        Actions = new PlayerInputActions();
        Actions.Player.Enable();
    }

    void OnDestroy()
    {
        Actions.Player.Disable();
        Actions.Dispose();
    }
}
