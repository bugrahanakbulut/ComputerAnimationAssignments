using System;
using BLUE.PoolingSystem;
using UnityEngine;

[Serializable]
public class ParticleFactory : IFactory<ParticleActivationInfo, Particle_PoolObject>
{

    [SerializeField] private Particle_PoolObject _reference = null;

    [SerializeField] private Transform _poolCarrier = null;
    
    public Action OnCreatedPoolObject { get; set; }
    
    public Particle_PoolObject Create()
    {
        GameObject instantiated = GameObject.Instantiate(_reference.gameObject);

        instantiated.transform.parent = _poolCarrier;

        return instantiated.GetComponent<Particle_PoolObject>();
    }
}
