using UnityEngine;
using UnityEngine.InputSystem;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance { get; private set; }

    public Vector3 GravityDirection { get; private set; } = Vector3.down;
    [SerializeField] private float gravityStrength = 9.81f;

    public event System.Action<Vector3> OnGravityChanged;

    private void Awake()
    {
        // 既に存在している場合は自分を削除
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else Instance = this;

        ApplyGravity(GravityDirection);

        Debug.Log("GravityManager initialized");
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.iKey.wasPressedThisFrame) ChangeGravity(Vector3.forward);
        if (Keyboard.current.kKey.wasPressedThisFrame) ChangeGravity(Vector3.back);
        if (Keyboard.current.jKey.wasPressedThisFrame) ChangeGravity(Vector3.left);
        if (Keyboard.current.lKey.wasPressedThisFrame) ChangeGravity(Vector3.right);
        if (Keyboard.current.uKey.wasPressedThisFrame) ChangeGravity(Vector3.up);
        if (Keyboard.current.oKey.wasPressedThisFrame) ChangeGravity(Vector3.down);
    }

    public void ChangeGravity(Vector3 newDirection)
    {
        GravityDirection = newDirection.normalized;
        ApplyGravity(GravityDirection);

        OnGravityChanged?.Invoke(GravityDirection);

        Debug.Log($"Gravity changed to: {GravityDirection}");
    }

    public Vector3 getGravityDirection()
    {
        return GravityDirection;
    }

    private void ApplyGravity(Vector3 direction)
    {
        Physics.gravity = direction * gravityStrength;
    }
}
