using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Offset")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);

    [Header("Smoothing & Look-Ahead")]
    [SerializeField] private float sharpness = 5f;        // Blend sharpness factor[cite: 13]
    [SerializeField] private float lookAheadTime = 0.35f;   // Look-ahead multiplier[cite: 13]

    private Rigidbody targetRb;

    private void Awake()
    {
        if (target == null)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>(); // Slide 42 lookup[cite: 13]
            if (player != null) target = player.transform;
        }

        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody>();
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate Look-Ahead position using player's horizontal velocity[cite: 13]
        Vector3 targetVelocity = (targetRb != null) ? targetRb.linearVelocity : Vector3.zero;
        targetVelocity.y = 0f; // Flatten vertical velocity for smooth tracking[cite: 13]

        Vector3 wantedPosition = target.position + offset + (targetVelocity * lookAheadTime);

        // Frame-rate independent exponential smoothing: blend = 1 - e^(-k * dt)[cite: 13]
        float blend = 1f - Mathf.Exp(-sharpness * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, wantedPosition, blend);

        // Point camera at player's base target position[cite: 13]
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}