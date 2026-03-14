using UnityEngine;

// Attach this to the Ball prefab.
// WASD / Arrow keys move the ball via physics forces.
public class BallController : MonoBehaviour
{
    [Tooltip("How strong the force applied to the ball is")]
    public float force = 15f;

    [Tooltip("Limits the ball's top speed so it doesn't fly off")]
    public float maxSpeed = 12f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float v = Input.GetAxis("Vertical");   // W/S or Up/Down

        Vector3 movement = new Vector3(h, 0f, v);
        rb.AddForce(movement * force);

        // Clamp horizontal speed to avoid the ball going too fast
        Vector3 flat = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flat.magnitude > maxSpeed)
        {
            flat = flat.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(flat.x, rb.linearVelocity.y, flat.z);
        }
    }
}