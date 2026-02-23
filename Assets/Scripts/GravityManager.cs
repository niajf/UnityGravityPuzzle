using UnityEngine;
using UnityEngine.InputSystem;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance { get; private set; }

    public Vector3 GravityDirection { get; private set; } = Vector3.down;

    [SerializeField] private float gravityStrength = 9.81f;

    public Transform target;


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
        if (Keyboard.current == null || target == null) return;

        // 重力の向きを左に変更 
        if (Input.GetMouseButtonDown(0)) ChangeGravity(-target.transform.right);

        // 重力の向きを右に変更
        if (Input.GetMouseButtonDown(1)) ChangeGravity(target.transform.right);
    }

    public void ChangeGravity(Vector3 newDirection)
    {
        float maxDot = 0.0f;
        Vector3 dir = Vector3.up;
        var vectors = new Vector3[4] { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
        for (int i = 0; i < 4; i++)
        {
            float dot = Vector3.Dot(vectors[i], newDirection);
            if (maxDot < dot)
            {
                maxDot = dot;
                dir = vectors[i];
            }
        }
        GravityDirection = dir;
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
