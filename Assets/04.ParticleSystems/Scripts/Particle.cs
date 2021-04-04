using System;
using System.Collections;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [Range(1/60f, 2f)] [SerializeField] private float _timeStep = 1f;

    [SerializeField] private Vector3 _gravity = Vector3.zero;
    
    public Action OnCompleted { get; set; }
    
    private IEnumerator _playRoutine;
    
    public void Play(ParticleActivationInfo activationInfo)
    {
        StartPlayRoutine(activationInfo);
    }
    
    public void Reset()
    {
        
    }

    private void StartPlayRoutine(ParticleActivationInfo info)
    {
        StopPlayRoutine();

        _playRoutine = PlayProgress(info);

        StartCoroutine(_playRoutine);
    }

    private void StopPlayRoutine()
    {
        if (_playRoutine != null)
            StopCoroutine(_playRoutine);
    }

    private IEnumerator PlayProgress(ParticleActivationInfo activationInfo)
    {
        float timePassed = 0;

        Vector3 particlePosition = activationInfo.InitialPosition;
        Vector3 particleVelocity = activationInfo.InitialVelocity;
        
        transform.position = particlePosition;

        while (timePassed <= activationInfo.LifeTime)
        {
            particlePosition = GetParticleNextPosition(particlePosition, particleVelocity);
            particleVelocity = GetParticleNextVelocity(particleVelocity, _gravity);
            
            transform.position = particlePosition;

            timePassed += _timeStep;
            
            yield return new WaitForSeconds(_timeStep);
        }
        
        OnCompleted?.Invoke();
    }

    private Vector3 GetParticleNextPosition(Vector3 currentPosition, Vector3 currentVelocity)
    {
        return currentPosition + currentVelocity * _timeStep;
    }

    private Vector3 GetParticleNextVelocity(Vector3 currentVelocity, Vector3 acceleration)
    {
        return currentVelocity + acceleration * _timeStep;
    }
}
