using UnityEngine;

// Smoothly follows the ball from above, zoomed in closer than the full-maze view.
// This makes the game harder (you can't see the whole maze) and more exciting.
// Assign the ball via FindBallByTag() automatically, or drag it in the Inspector.
public class CameraFollow : MonoBehaviour
{
    [Tooltip("How high above the ball the camera hovers")]
    public float height = 8f;

    [Tooltip("How smoothly the camera moves — lower = snappier, higher = floatier")]
    [Range(1f, 20f)]
    public float smoothSpeed = 6f;

    [Tooltip("Tag used to find the ball at runtime (must match BallPrefab tag)")]
    public string ballTag = "Ball";

    private Transform target;

    void Update()
    {
        // Find the ball if we don't have a reference yet
        // (the ball is spawned at runtime, so we can't assign it in the Inspector)
        if (target == null)
        {
            GameObject ball = GameObject.FindWithTag(ballTag);
            if (ball != null)
                target = ball.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Target position: directly above the ball
        Vector3 desired = new Vector3(target.position.x, target.position.y + height, target.position.z);

        // Smoothly interpolate toward it
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // Always look straight down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}