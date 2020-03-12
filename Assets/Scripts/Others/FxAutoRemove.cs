using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FxManager;

public class FxAutoRemove : MonoBehaviour
{
    List<ParticleSystem> particles = default;

    [SerializeField] int timeBeforeDelete = 5;

    public void Start()
    {
        particles = new List<ParticleSystem>();
        ParticleSystem[] foundParticles = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem part in foundParticles)
        {
            particles.Add(part);
        }
    }

    private void Update()
    {
        List<ParticleSystem> particlesToRemove = new List<ParticleSystem>();

        foreach (ParticleSystem part in particles)
        {
            if (!part.isPlaying)
            {
                particlesToRemove.Add(part);
            }
        }

        foreach (ParticleSystem part in particlesToRemove)
        {
            particles.Remove(part);
        }

        if (particles.Count == 0)
        {
            Destroy(gameObject);
        }

        StartCoroutine(DestroyInCase());
    }

    // Somes FX have problems to delete themself
    IEnumerator DestroyInCase()
    {
        yield return new WaitForSeconds(timeBeforeDelete);
        Destroy(gameObject);
    }
}
