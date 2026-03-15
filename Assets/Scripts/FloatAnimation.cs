using UnityEngine;

// Makes the object float up and down and slowly rotate — no Rigidbody needed.
public class FloatAnimation : MonoBehaviour
{
    [Tooltip("How high it bobs up and down (in units)")]
    public float amplitude = 0.15f;

    [Tooltip("How fast it bobs")]
    public float frequency = 1.5f;

    [Tooltip("Degrees per second rotation around Y axis")]
    public float rotationSpeed = 90f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        // Offset the phase based on position so nearby collectables
        // don't all move in perfect sync — looks more natural
        float phaseOffset = (transform.position.x + transform.position.z) * 0.5f;
        startPosition.y += Mathf.Sin(phaseOffset) * amplitude;
    }

    void Update()
    {
        // Bob up and down using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotate around Y axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }
}