using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SystemTriggers : MonoBehaviour
{
    public GameObject TargetParticleSystem;
    private List<ParticleSystem> _particles = new List<ParticleSystem>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnEffect();
        }
    }

    private void SpawnEffect()
    {
        var fx = Instantiate(TargetParticleSystem, transform.position, transform.rotation);
        _particles = fx.GetComponentsInChildren<ParticleSystem>().ToList();

        foreach (var particle in _particles)
        {
            particle.Play();
        }
        Destroy(fx, 2f);
    }
}
