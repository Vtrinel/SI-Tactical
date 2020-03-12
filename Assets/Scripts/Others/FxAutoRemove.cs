using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FxManager;

public class FxAutoRemove : MonoBehaviour
{
    private float timeToDisable;
    [SerializeField] List<ParticleSystem> particles = default;
    


    private void Update()
    {
        List<ParticleSystem> particlesToRemove = new List<ParticleSystem>();

        foreach (ParticleSystem part in particles)
        {
            if (!part.isPlaying)
                particlesToRemove.Add(part);
        }

        foreach (ParticleSystem part in particlesToRemove)
            particles.Remove(part);
    }

    [ContextMenu("GetParticles")]
    public void GetParticles()
    {
        particles = new List<ParticleSystem>();
        ParticleSystem[] foundParticles = GetComponentsInChildren<ParticleSystem>();

        foreach(ParticleSystem part in foundParticles)
        {
            particles.Add(part);
        }
    }
}
