using BLUE.PoolingSystem;
using UnityEngine;

public class ParticleActivationInfo : PoolObjectActivationInfo
{
    public Vector3 InitialPosition { get; }
    
    public Vector3 InitialVelocity { get; }
    
    public float LifeTime { get; }

    public ParticleActivationInfo(Vector3 initialPosition, Vector3 initialVelocity, float lifeTime)
    {
        InitialPosition = initialPosition;
        InitialVelocity = initialVelocity;
        LifeTime = lifeTime;
    }
}

public class Particle_PoolObject : PoolGameObject<ParticleActivationInfo>
{
    [SerializeField] private Particle _particle = null;
    
    protected override void ActivateCustomActions(ParticleActivationInfo activationInfo)
    {
        _particle.gameObject.SetActive(true);

        _particle.Play(activationInfo);

        _particle.OnCompleted += OnParticleCompleted;
    }

    protected override void DeactivationCustomActions()
    {
        _particle.gameObject.SetActive(false);
        
        _particle.Reset();
    }
    
    private void OnParticleCompleted()
    {
        DeactivatePoolObject();
    }
}
