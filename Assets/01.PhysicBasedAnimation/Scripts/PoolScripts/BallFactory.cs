using System;
using BLUE.PoolingSystem;
using UnityEngine;

public class BallFactory : MonoBehaviour, 
    IFactory<BallActivationInfo, BallPoolObject>
{
    [SerializeField] private BallPoolObject _referenceObject = null;

    [SerializeField] private Transform _poolHolder = null;
    
    public Action OnCreatedPoolObject { get; set; }
    
    public BallPoolObject Create()
    {
        OnCreatedPoolObject?.Invoke();

        return Instantiate(_referenceObject, _poolHolder).GetComponent<BallPoolObject>();
    }
}