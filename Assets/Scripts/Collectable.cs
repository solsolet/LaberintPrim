using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Tooltip("Points awarded when collected")]
    public int points = 10;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;

        // !! Game logic FIRST — effect second.
        // If the effect throws on mobile the score still registers.
        GameManager.Instance.CollectItem(points);

        CollectEffect fx = GetComponent<CollectEffect>();
        if (fx != null) fx.Burst(transform.position);

        Destroy(gameObject);
    }
}