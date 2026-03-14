using UnityEngine;

// On PC: WASD / Arrow keys
// On mobile: gyroscope (tilt the device)
// Both can be active at the same time — useful for testing in the editor
public class BallController : MonoBehaviour
{
    [Tooltip("How strong the force applied to the ball is")]
    public float force = 15f;

    [Tooltip("Limits the ball's top speed so it doesn't fly off")]
    public float maxSpeed = 12f;

    [Tooltip("How sensitive the gyroscope tilt is")]
    public float gyroSensitivity = 2.5f;

    private Rigidbody rb;
    private bool gyroAvailable = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Enable gyroscope if the device has one
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            gyroAvailable = true;
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;
 
        // --- Keyboard input (PC + editor testing) ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        movement += new Vector3(h, 0f, v);

        // --- Gyroscope input (mobile) ---
        if (gyroAvailable)
        {
            // Input.gyro.gravity gives a Vector3 representing how gravity
            // pulls relative to the screen. We map it to X/Z movement
            Vector3 tilt = Input.gyro.gravity;
            float gx =  tilt.x * gyroSensitivity;   // tilt left/right
            float gz = tilt.y * gyroSensitivity;
 
            movement += new Vector3(gx, 0f, gz);
        }
 
        // Apply force only if there's input
        if (movement.sqrMagnitude > 0.01f)
            rb.AddForce(movement.normalized * force);
 
        // Clamp horizontal speed
        Vector3 flat = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flat.x, rb.linearVelocity.y, flat.z);
        }
    }
}