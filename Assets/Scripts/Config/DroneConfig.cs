using UnityEngine;

[CreateAssetMenu(fileName = "DroneConfig", menuName = "GravityPuzzle/DroneConfig")]
public class DroneConfig : ScriptableObject
{
    [Header("AI Settings")]
    public float moveSpeed = 3.0f;
    public float rotationSpeed = 5.0f;

    [Header("Sensor Settings")]
    public float sightRadius = 10.0f;   // 追跡範囲
    public float fieldOfView = 60.0f;   // 視野

    [Header("Projectile Setting")]
    public float attackInterval = 2.0f;
    public float projectileSpeed = 10.0f;
}
