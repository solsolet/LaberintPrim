using System.Collections;
using UnityEngine;

// On level load: zooms out to show the full maze, then smoothly zooms back in
// to follow the ball. During play it tracks the ball from a close height.
public class CameraFollow : MonoBehaviour
{
    [Header("Follow settings")]
    [Tooltip("Height above the ball during normal play")]
    public float playHeight = 8f;

    [Tooltip("How smoothly the camera follows — lower = snappier")]
    [Range(1f, 20f)]
    public float smoothSpeed = 6f;

    [Header("Level-start zoom transition")]
    [Tooltip("Height for the full-maze overview at level start")]
    public float overviewHeight = 40f;

    [Tooltip("How long the overview is shown before zooming in (seconds)")]
    public float overviewDuration = 1.2f;

    [Tooltip("How long the zoom-in transition takes (seconds)")]
    public float zoomInDuration = 1.0f;

    [Header("Ball detection")]
    public string ballTag = "Ball";

    // -------------------------------------------------------
    private Transform target;
    private bool      transitioning = false;
    private float     currentHeight;

    void Awake()
    {
        currentHeight = overviewHeight;
    }

    void Update()
    {
        // Find the ball at runtime (it is spawned dynamically)
        if (target == null)
        {
            GameObject ball = GameObject.FindWithTag(ballTag);
            if (ball != null)
                target = ball.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null || transitioning) return;

        Vector3 desired = new Vector3(target.position.x,
                                      target.position.y + currentHeight,
                                      target.position.z);

        transform.position = Vector3.Lerp(transform.position, desired,
                                          smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    // Call this from MazeRenderer after building each level
    // It positions the camera at the maze centre for the overview,
    // then smoothly zooms down to follow the ball.
    public void PlayLevelTransition(Vector3 mazeCenter)
    {
        StopAllCoroutines();
        StartCoroutine(LevelTransition(mazeCenter));
    }

    IEnumerator LevelTransition(Vector3 center)
    {
        transitioning = true;
        currentHeight = overviewHeight;

        // Snap to overview position immediately
        transform.position = new Vector3(center.x, center.y + overviewHeight, center.z);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Hold the overview so the player can study the maze
        yield return new WaitForSeconds(overviewDuration);

        // Smoothly zoom in to play height
        float elapsed  = 0f;
        float startH   = overviewHeight;
        Vector3 startPos = transform.position;

        while (elapsed < zoomInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / zoomInDuration);
            currentHeight = Mathf.Lerp(startH, playHeight, t);

            // While zooming, keep centred on maze (target may not be moving yet)
            if (target != null)
            {
                Vector3 desired = new Vector3(target.position.x,
                                              target.position.y + currentHeight,
                                              target.position.z);
                transform.position = desired;
            }

            yield return null;
        }

        currentHeight = playHeight;
        transitioning = false;
    }
}