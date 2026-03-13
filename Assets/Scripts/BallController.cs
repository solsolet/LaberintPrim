using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float force = 10f;

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0, v);

        rb.AddForce(movement * force);
    }
}