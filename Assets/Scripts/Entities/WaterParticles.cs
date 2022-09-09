using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WaterParticles : MonoBehaviour
{
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        particles.Stop();
    }

    public void PlayWaterParticles(float duration)
    {
        // Need to stop and clear before setting duration of system
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Get main module to access the duration (can't change it directly)
        ParticleSystem.MainModule pMain = particles.main;

        // Change duration to match
        pMain.duration = duration;

        // Play for given duration
        particles.Play();

        // Destroy particles after duration ended
        Invoke(nameof(DestroyParticles), duration);
    }

    private void DestroyParticles()
    {
        Destroy(gameObject);
    }

}
