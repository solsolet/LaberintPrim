using UnityEngine;

// Attach to each collectable prefab.
// Set 'points' in the Inspector:
//   CollectableSmall  -> 10
//   CollectableMedium -> 25
//   CollectableLarge  -> 50
public class Collectable : MonoBehaviour
{
    [Tooltip("Points awarded when the ball touches this")]
    public int points = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            GameManager.Instance.CollectItem(points);
            Destroy(gameObject);   // Remove from scene
        }
    }
}