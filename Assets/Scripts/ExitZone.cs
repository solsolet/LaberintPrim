using UnityEngine;
 
// Attach to the ExitPrefab.
// The exit's collider must be set to "Is Trigger" in the Inspector.
public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            GameManager.Instance.ReachExit();
        }
    }
}
