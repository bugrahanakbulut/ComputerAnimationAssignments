using BLUE.PoolingSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleSys : MonoBehaviour
{
    [SerializeField] private int _particleCount = 100;

    [SerializeField] private float _particleSpeed = 5f;
    
    [SerializeField] private float _discRadius = 1.5f;

    [SerializeField] private float _particleLifeTime = 1f;
    
    [SerializeField] private ParticleFactory _particleFactory = new ParticleFactory();

    [SerializeField] private float _offset = 0.5f;

    [SerializeField] private Vector3 _particleAngularOffset = Vector3.zero;
    
    private Pool<
        ParticleActivationInfo, 
        Particle_PoolObject, 
        ParticleFactory> _particlePool;
    
    private void Awake()
    {
        _particlePool = new Pool<ParticleActivationInfo, Particle_PoolObject, ParticleFactory>(_particleFactory, 0);
        
        FireParticles();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            FireParticles();
    }

    private void FireParticles()
    {
        Vector3 particleSpeed = (transform.up + _particleAngularOffset).normalized * _particleSpeed;
        
        for (int i = 0; i < _particleCount; i++)
        {
            _particlePool.ActivatePoolObject(
                new ParticleActivationInfo(
                    GenerateRandomPoint(),
                    particleSpeed,
                    _particleLifeTime));
        }
    }

    private Vector3 GenerateRandomPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * _discRadius;

        Vector3 particlePosition = transform.position + transform.right * randomPoint.x + transform.forward * randomPoint.y;

        particlePosition += Random.Range(0f, _offset) * transform.up;
        
        return particlePosition;
    }
}
