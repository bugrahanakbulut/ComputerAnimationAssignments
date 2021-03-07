using System;
using System.Collections.Generic;
using BLUE.PoolingSystem;
using UnityEngine;

public class MovementSimulator : MonoBehaviour
{
    private class TimeStampData
    {
        public Vector3 Position { get; }
        
        public Vector3 Velocity { get; }

        public Vector3 Acceleration { get; }
        
        public float Time { get; }

        public TimeStampData(Vector3 position, Vector3 velocity, Vector3 acceleration, float time)
        {
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Time = time;
        }
    }
    
    [SerializeField] private BallFactory _factory = null;

    [SerializeField] private InputUI _inputUI = null;
    
    [SerializeField] private Transform _targetPhysicTransform = null;
    
    [SerializeField] private Vector3 _initialPosition = Vector3.zero;
    
    [SerializeField] private Vector3 _initialVelocity = Vector3.zero;

    [SerializeField] private Vector3 _gravity = Physics.gravity;
    
    [SerializeField] private Vector3 _windVelocity = Vector3.zero;

    [SerializeField] private float _airResCoef = 0;

    [SerializeField] private float _mass = 1;
    
    [Range(1/60f, 2f)][SerializeField] private float _timeStep = 1f;

    [SerializeField] private int _showStep = 0;
    
    private List<TimeStampData> _timeStamps = new List<TimeStampData>();

    private Pool<BallActivationInfo, BallPoolObject, BallFactory> _pool;

    public void UpdateInitialPosition(Vector3 val)
    {
        _initialPosition = val;
        
        _targetPhysicTransform.position = _initialPosition;
        
        InitPositions();
    }
    
    public void UpdateInitialVelocity(Vector3 vectorParser)
    {
        _initialVelocity = vectorParser;
        
        InitPositions();
    }
    
    public void UpdateTimeStep(float value)
    {
        _timeStep = value;
        
        InitPositions();
    }
    
    public void DisplayStep(int step)
    {
        _showStep = step;
        
        _targetPhysicTransform.position = _timeStamps[_showStep].Position;
    }
    
    public void UpdateWindPosition(Vector3 val)
    {
        _windVelocity = val;
        
        InitPositions();
    }
    
    public void UpdateMass(float val)
    {
        _mass = val;
    }

    public void UpdateAirResFloatParser(float val)
    {
        _airResCoef = val;
    }

    private void Awake()
    {
        _pool = new Pool<BallActivationInfo, BallPoolObject, BallFactory>(_factory,1000);
        
        _targetPhysicTransform.position = _initialPosition;
        
        InitPositions();

        InitUI();
    }

    private void LateUpdate()
    {
        InitPositions();
    }

    private void InitPositions()
    {
        _timeStamps.Clear();

        Vector3 acceleration = _gravity + (_airResCoef / _mass) * (_windVelocity - _initialVelocity);
        
        _timeStamps.Add(new TimeStampData(_initialPosition, _initialVelocity, acceleration, 0));

        Vector3 position = _initialPosition;

        int step = 0;
        
        while (position.y > 0)
        {
            Vector3 newVelocity = _timeStamps[step].Velocity + _timeStamps[step].Acceleration * _timeStep;

            Vector3 newPosition = _timeStamps[step].Position + _timeStamps[step].Velocity * _timeStep;

            position = newPosition;
            
            step++;
                
            if (position.y > 0)
                _timeStamps.Add(new TimeStampData(newPosition, newVelocity, acceleration, step * _timeStep));
        }

        VisualizeMovement();

        DisplayStep(Mathf.Clamp(_showStep, 0, _timeStamps.Count - 1));

        InitUI();
    }

    private void VisualizeMovement()
    {
        _pool.DeactivateAll();
        
        foreach (TimeStampData data in _timeStamps)
        {
            _pool.ActivatePoolObject(new BallActivationInfo(data.Position));
        }
    }

    private void InitUI()
    {
        _inputUI.InitTimeStepSlider(_timeStep);
        
        _inputUI.InitStepSlider(_timeStamps.Count - 1, _showStep);
    }
}
