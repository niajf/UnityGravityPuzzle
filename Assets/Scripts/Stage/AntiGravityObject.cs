using UnityEngine;

public class AntiGravityObject : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 targetUpVector = Vector3.down; // 重力の向き
    private bool isReverse = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GravityManager.Instance.OnGravityChanged += OnGravityChanged;
    }

    void FixedUpdate()
    {
        Vector3 dir = targetUpVector;
        if (isReverse) dir = -dir;

        rb.AddForce(dir * Physics.gravity.magnitude);
    }

    void OnGravityChanged(Vector3 newGravityDir)
    {
        targetUpVector = newGravityDir;
    }

    void OnDestroy()
    {
        if (GravityManager.Instance != null)
        {
            GravityManager.Instance.OnGravityChanged -= OnGravityChanged;
        }
    }

    public void normalGravity()
    {
        isReverse = false;
    }

    public void revesalGravity()
    {
        isReverse = true;
    }
}
