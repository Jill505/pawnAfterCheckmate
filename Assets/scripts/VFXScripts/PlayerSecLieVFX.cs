using System;
using System.Collections;
using UnityEngine;

public class PlayerSecLieVFX : MonoBehaviour
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

    public float magicalNumber = 10f;
    
    public void SetTarget(GameObject obj)
    {
        Target = obj.transform;
    }

    private void Start()
    {
        Debug.Log("海馬成長痛");
        p = GetComponent<ParticleSystem>();
        SetTarget(FindFirstObjectByType<TSA_Player>().gameObject);

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
        particleMove();
        transform.position = Target.position;
        acceleration = Mathf.Lerp(acceleration, accelerationTargetStrength, 2 * Time.deltaTime);
    }

    public void particleMove()
    {
        if (p == null || Target == null) return;

        int count = p.particleCount;

        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        p.GetParticles(particles);

        for (int i = 0; i < p.particleCount; i++)
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
    public void KillAllParticles()
    {
        //StartCoroutine(KillAllParticlesCoroutine());
        //p = GetComponent<ParticleSystem>();
        //p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (gameObject != null)
        {
            Destroy(gameObject, 3f);
        }
    }
    /*
    public IEnumerator KillAllParticlesCoroutine()
    {
        p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        yield return null;

        int count = p.particleCount;

        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        p.GetParticles(particles);

        //實作粒子alpha漸變消失
        for (int i = 0; i < particles.Length; i++)
        {
        }
    }*/
    public IEnumerator KillAllParticlesCoroutine()
    {
        if (p == null) p = GetComponent<ParticleSystem>();
        if (p == null) yield break;

        // 1) 先停止產生新粒子
        var emission = p.emission;
        emission.enabled = false;

        p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        yield return null; // 等一幀讓 StopEmitting 生效

        // 2) 取得現有粒子
        int count = p.particleCount;
        if (count <= 0) yield break;

        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        count = p.GetParticles(particles);

        // 3) 記錄每顆粒子的起始 alpha（用 startColor）
        byte[] startA = new byte[count];
        for (int i = 0; i < count; i++)
            startA[i] = particles[i].startColor.a;

        // 4) 逐幀淡出
        float fadeDuration = 0.7f; // 可調
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);

            for (int i = 0; i < count; i++)
            {
                Color32 c = particles[i].startColor;   // startColor 是 Color32
                c.a = (byte)Mathf.RoundToInt(Mathf.Lerp(startA[i], 0f, k));
                particles[i].startColor = c;

                // 可選：淡出時順便減速
                // particles[i].velocity *= 0.9f;
            }

            p.SetParticles(particles, count);
            yield return null;
        }

        // 5) 保險：清掉殘留
        p.Clear(true);
    }



    /*
    public void KillAllParticles() //GPT實作
    {
        if (p == null) p = GetComponent<ParticleSystem>();
        if (p == null) return;

        // 1) 先禁止 ParticleSystem 生成新粒子（最重要）
        var emission = p.emission;
        emission.enabled = false;

        // 保險：停止「繼續發射」，但保留目前已存在的粒子
        p.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // 2) 取得目前現存粒子數，並以此為準配置所有陣列（不信任 particleKilled.Length）
        int count = p.particleCount;
        if (count <= 0) return;

        if (particles == null || particles.Length < count)
            particles = new ParticleSystem.Particle[count];

        p.GetParticles(particles);

        // 3) 確保狀態陣列至少有 count 長度
        if (particleKilled == null || particleKilled.Length < count)
        {
            var newKilled = new bool[count];
            if (particleKilled != null)
                Array.Copy(particleKilled, newKilled, particleKilled.Length);
            particleKilled = newKilled;
        }

        if (particleAlpha == null || particleAlpha.Length < count)
        {
            var newAlpha = new float[count];
            if (particleAlpha != null)
                Array.Copy(particleAlpha, newAlpha, particleAlpha.Length);

            // 新增的欄位給預設 alpha=1，避免被當成 0 直接透明
            for (int i = (particleAlpha?.Length ?? 0); i < count; i++)
                newAlpha[i] = 1f;

            particleAlpha = newAlpha;
        }

        // 4) 全部標記為要 kill（你現有的 RenderAlphaParticleCal() 會把 alpha lerp 到 0）
        for (int i = 0; i < count; i++)
        {
            particleKilled[i] = true;

            // 如果你希望 kill 時先把速度停掉（可選）
            // particles[i].velocity = Vector3.zero;

            // 若要「立刻消失」而不是淡出，就改用這行（擇一）
            // particles[i].remainingLifetime = 0f;
        }

        // 回寫（可有可無；如果你沒改 particles 內容可以不 SetParticles）
        p.SetParticles(particles, count);
    }*/
}
