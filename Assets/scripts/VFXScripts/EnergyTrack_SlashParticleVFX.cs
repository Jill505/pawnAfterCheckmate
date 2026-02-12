using UnityEngine;
using System;

public class EnergyTrack_SlashParticleVFX : MonoBehaviour
{
    public ParticleSystem p;
    public Transform Target;

    public float acceleration = 10f;   // 吸引力強度
    public float accelerationTargetStrength;
    public float maxSpeed = 20f;        // 速度上限
    public float killDistance = 0.5f;

    private ParticleSystem.Particle[] particles;
    bool[] particleKilled;
    float[] particleAlpha;

    Transform thisTransform;

    public float magicalNumber = 10f;

    private void Start()
    {
        p = GetComponent<ParticleSystem>();
        Target = GameObject.Find("ParticleTrackTarget").GetComponent<Transform>();

        acceleration = 10f;
        particleAlpha = new float[p.particleCount];
        particleKilled = new bool[p.particleCount];

        for (int i = 0; i < particleAlpha.Length; i++)
        {
            particleAlpha[i] = 1f;
            particleKilled[i] = false;
        }
    }

    void Update()
    {

        //particleMove();
        //whatEverLah();
        //copyVar();
        particleMove();
        acceleration = Mathf.Lerp(acceleration, accelerationTargetStrength, 2 *Time.deltaTime);
    }
    public void whatEverLah()
    {
        particles = new ParticleSystem.Particle[p.particleCount];

        p.GetParticles(particles);

        for (int i = 0; i < particles.GetUpperBound(0); i++)
        {
            particles[i].velocity = (particles[i].velocity) / 2 * Time.deltaTime; 
        }

        p.SetParticles(particles, particles.Length);
    }
    public void copyVar()
    {
        particles = new ParticleSystem.Particle[p.particleCount];

        p.GetParticles(particles);

        for (int i = 0; i < particles.GetUpperBound(0); i++)
        {

            float ForceToAdd = (particles[i].startLifetime - particles[i].remainingLifetime) * (magicalNumber * Vector3.Distance(Target.position, particles[i].position));

            //Debug.DrawRay (particles [i].position, (Target.position - particles [i].position).normalized * (ForceToAdd/10));

            particles[i].velocity = (Target.position - particles[i].position).normalized * ForceToAdd;
            ParticleSystem.Particle tP = new ParticleSystem.Particle();

            particles [i].position = Vector3.Lerp (particles [i].position, Target.position, Time.deltaTime / 2.0f);

        }

        p.SetParticles(particles, particles.Length);
    }

    public void particleMove()
    {
        if (p == null || Target == null) return;

        int count = p.particleCount;

        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        p.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector2 toTarget = Target.position - particles[i].position;
            float distance = toTarget.magnitude;

            // 方向
            Vector2 dir = toTarget.normalized;

            particles[i].velocity += (Vector3)dir * acceleration * Time.deltaTime;

            if (particles[i].velocity.magnitude > maxSpeed)
            {
                particles[i].velocity =
                    particles[i].velocity.normalized * maxSpeed;
            }

            if (distance < killDistance)
            {
                //particles[i].remainingLifetime = 0f;
                //particleKilled[i] = true;
            }
        }

        RenderAlphaParticleCal();
        
        for (int i = 0; i < particleKilled.Length; i++)
        {
            particles[i].color = new Color(particles[i].color.r, particles[i].color.g, particles[i].color.b, particleAlpha[i]);
        }

        p.SetParticles(particles, count);
    }

    public void RenderAlphaParticleCal()
    {
        for (int i = 0; i < particleKilled.Length; i++)
        {
            if (particleKilled[i])
            {
                particleAlpha[i] = Mathf.Lerp(particleAlpha[i], 0f, 2f * Time.deltaTime);
            }
        }
    }
}
