using UnityEngine;

// Attach to each collectable prefab.
// Assign 'particleMaterial' in the Inspector — drag any URP-compatible
// particle material from your project. If left empty, particles still
// appear (white/default color) so gameplay is never blocked.
public class CollectEffect : MonoBehaviour
{
    [Tooltip("Color of the burst — set per prefab")]
    public Color particleColor = Color.yellow;

    [Tooltip("How many particles in the burst")]
    public int particleCount = 18;

    [Tooltip("Drag a URP Particles/Unlit material here from your project")]
    public Material particleMaterial;

    public void Burst(Vector3 position)
    {
        try
        {
            GameObject fx = new GameObject("CollectFX");
            fx.transform.position = position;

            ParticleSystem ps = fx.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.startLifetime   = 0.6f;
            main.startSpeed      = new ParticleSystem.MinMaxCurve(2f, 5f);
            main.startSize       = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
            main.startColor      = new ParticleSystem.MinMaxGradient(particleColor);
            main.gravityModifier = 0.4f;
            main.loop            = false;
            main.playOnAwake     = false;

            var shape = ps.shape;
            shape.enabled   = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius    = 0.2f;

            // Apply the material assigned in the Inspector (no Shader.Find)
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
            // Never block gameplay because of a visual effect
            Debug.LogWarning("CollectEffect.Burst failed: " + e.Message);
        }
    }
}