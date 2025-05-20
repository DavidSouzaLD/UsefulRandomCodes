using UnityEngine;

public class Lean : MonoBehaviour
{
    [Header("Lean Settings")]
    [SerializedField, Tooltip("Target lean angle in degrees (positive = right, negative = left)")]
    private float targetAngle = 0f;

    [SerializedField, Tooltip("How strong the PID response is (proportional)")]
    private float kp = 5f;

    [SerializedField, Tooltip("How much to accumulate error (integral)")]
    private float ki = 0f;

    [SerializedField, Tooltip("How much to resist change (derivative)")]
    private float kd = 1f;

    [SerializedField, Tooltip("Max degrees per second to lean")]
    private float maxLeanSpeed = 90f;

    private float integral;
    private float lastError;

    public float CurrentLeanAngle => Vector3.SignedAngle(Vector3.up, transform.up, transform.forward);
    private Rigidbody Rigidbody { get; set; };

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Step 1: Get current lean angle
        var currentAngle = CurrentLeanAngle;

        // Step 2: Calculate PID output
        var error = targetAngle - currentAngle;
        integral += error * Time.fixedDeltaTime;
        var derivative = (error - lastError) / Time.fixedDeltaTime;
        lastError = error;
        var pidOutput = kp * error + ki * integral + kd * derivative;

        // Step 3: Clamp angular change to avoid overshoot
        var clampedAngle = Mathf.Clamp(pidOutput * Time.fixedDeltaTime, -maxLeanSpeed * Time.fixedDeltaTime, maxLeanSpeed * Time.fixedDeltaTime);

        // Step 4: Apply rotation around local forward axis (roll)
        var deltaRotation = Quaternion.AngleAxis(clampedAngle, transform.forward);
        var targetRotation = Rigidbody.rotation * deltaRotation;
        Rigidbody.MoveRotation(targetRotation);
    }
}
