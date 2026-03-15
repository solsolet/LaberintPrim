using UnityEngine;

public class HoleTrap : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        // !! Game logic FIRST — effect second.
        GameManager.Instance.FallInHole();

        HoleEffect fx = GetComponent<HoleEffect>();
        if (fx != null) fx.Burst(other.transform.position);
    }
}