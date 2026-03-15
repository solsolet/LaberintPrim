using UnityEngine;

// Attach to HolePrefab.
// Assign 'particleMaterial' in the Inspector — same material as CollectEffect.
public class HoleEffect : MonoBehaviour
{
    [Tooltip("How many particles in the burst")]
    public int particleCount = 25;

    [Tooltip("Drag a URP Particles/Unlit material here from your project")]
    public Material particleMaterial;

    private static readonly Color HoleRed = new Color(0.9f, 0.1f, 0.1f);

    public void Burst(Vector3 position)
    {
        try
        {
            GameObject fx = new GameObject("HoleFX");
            fx.transform.position = position;

            ParticleSystem ps = fx.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.startLifetime   = 0.8f;
            main.startSpeed      = new ParticleSystem.MinMaxCurve(1.5f, 4f);
            main.startSize       = new ParticleSystem.MinMaxCurve(0.1f, 0.22f);
            main.startColor      = new ParticleSystem.MinMaxGradient(HoleRed);
            main.gravityModifier = 0.8f;
            main.loop            = false;
            main.playOnAwake     = false;

            var shape = ps.shape;
            shape.enabled   = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius    = 0.3f;

            if (particleMaterial != null)
            {
                var rend = fx.GetComponent<ParticleSystemRenderer>();
                rend.material = particleMaterial;
            }

            ps.Play();
            ps.Emit(particleCount);
            Destroy(fx, main.startLifetime.constantMax + 0.2f);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("HoleEffect.Burst failed: " + e.Message);
        }
    }
}