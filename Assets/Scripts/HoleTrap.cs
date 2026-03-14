using UnityEngine;

// Attach to the HolePrefab.
// The hole's collider must be set to "Is Trigger" in the Inspector.
public class HoleTrap : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            GameManager.Instance.FallInHole();
        }
    }
}