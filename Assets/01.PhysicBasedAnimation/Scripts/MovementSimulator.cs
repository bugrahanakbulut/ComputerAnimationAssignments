using System.Collections;
using System.Collections.Generic;
using BLUE.PoolingSystem;
using UnityEngine;

public class MovementData
{
    public Vector3 Position { get; }
        
    public Vector3 Velocity { get; }

    public Vector3 Acceleration { get; }
    
    public MovementData(Vector3 position, Vector3 velocity, Vector3 acceleration)
    {
        Position = position;
        Velocity = velocity;
        Acceleration = acceleration;
    }
}

public class MovementSimulator : MonoBehaviour
{
    [SerializeField] private bool _isLocked = false;
    
    [SerializeField] private BallFactory _factory = null;

    [SerializeField] private CollisionSimulator _collisionSimulator = null;
    
    [SerializeField] private Transform _targetPhysicTransform = null;
    
    [SerializeField] private Vector3 _initialPosition = Vector3.zero;
    public Vector3 InitialPosition => _initialPosition;

    [SerializeField] private Vector3 _initialVelocity = Vector3.zero;
    public Vector3 InitialVelocity => _initialVelocity;

    [SerializeField] private Vector3 _gravity = Physics.gravity;
    
    [SerializeField] private Vector3 _windVelocity = Vector3.zero;

    [SerializeField] private float _airResCoef = 0;

    [SerializeField] private float _mass = 1;

    [Range(1/60f, 2f)][SerializeField] private float _timeStep = 1f;

    [SerializeField] private int _showStep = 0;

    [SerializeField] private int _consideredCollisions = 1;
    
    private List<MovementData> _movementDatum = new List<MovementData>();
    public List<MovementData> MovementDatum => _movementDatum;

    private Pool<BallActivationInfo, BallPoolObject, BallFactory> _pool;

    private List<CollisionData> _collisionDatum = new List<CollisionData>();

    private IEnumerator _movementDisplayRoutine;
    
    private const int _MAX_STEP = 500;

    public void StartMovementVisualizeRoutine()
    {
        StopMovementVisualizeRoutine();

        _movementDisplayRoutine = MovementVisualizeRoutine();

        StartCoroutine(_movementDisplayRoutine);
    }
    
    private void Awake()
    {
        _pool = new Pool<BallActivationInfo, BallPoolObject, BallFactory>(_factory,1000);
        
        _targetPhysicTransform.position = _initialPosition;
        
        InitPositions();
    }

    private void LateUpdate()
    {
        if (!_isLocked)
            InitPositions();
    }

    private void InitPositions()
    {
        _movementDatum.Clear();
        
        _collisionDatum.Clear();
        
        _movementDatum.AddRange(CalculateMovement(_initialPosition, _initialVelocity));

        int index = 0;
        
        for (int i = 0; i < _consideredCollisions; i++)
        {
            CollisionData collisionData = _collisionSimulator.DetectCollision(_movementDatum.GetRange(index, _movementDatum.Count - index));
            
            if (collisionData != null)
            {
                _movementDatum.RemoveAt(_movementDatum.Count - 1);
                
                index = _movementDatum.Count;
                
                _movementDatum.AddRange(
                    CalculateMovement(
                        collisionData.CollisionPoint, 
                        collisionData.ReflectionVelocity));
            }
            else
                break;
        }
        
        _movementDatum.RemoveAt(_movementDatum.Count - 1);
        
        DisplayStep(Mathf.Clamp(_showStep, 0, _movementDatum.Count - 1));
        
        VisualizeMovement();
    }

    private List<MovementData> CalculateMovement(Vector3 initialPosition, Vector3 initialVelocity)
    {
        List<MovementData> calculatedDatum = new List<MovementData>();
        
        Vector3 acceleration = _gravity + (_airResCoef / _mass) * (_windVelocity - initialVelocity);
        
        calculatedDatum.Add(new MovementData(initialPosition, initialVelocity, acceleration));
        
        while (calculatedDatum.Count - 1 < _MAX_STEP)
        {
            int step = calculatedDatum.Count - 1;
            
            Vector3 newVelocity = calculatedDatum[step].Velocity + calculatedDatum[step].Acceleration * _timeStep;

            Vector3 newPosition = calculatedDatum[step].Position + calculatedDatum[step].Velocity * _timeStep;
            
            MovementData calculatedData = new MovementData(newPosition, newVelocity, acceleration);

            CollisionData data = _collisionSimulator.DetectCollision(calculatedDatum[step], calculatedData);

            calculatedDatum.Add(calculatedData);
            
            if (data != null && calculatedDatum.Count > 2)
            {
                return calculatedDatum;
            }
        }

        return calculatedDatum;
    }
    
    private void DisplayStep(int step)
    {
        _showStep = step;
        
        _targetPhysicTransform.position = _movementDatum[_showStep].Position;
    }

    private void VisualizeMovement()
    {
        _pool.DeactivateAll();
        
        foreach (MovementData data in _movementDatum)
        {
            _pool.ActivatePoolObject(new BallActivationInfo(data.Position));
        }
    }
    
    private void StopMovementVisualizeRoutine()
    {
        if (_movementDisplayRoutine != null)
            StopCoroutine(_movementDisplayRoutine);
    }

    private IEnumerator MovementVisualizeRoutine()
    {
        _isLocked = true;
        
        for (int i = 0; i < _movementDatum.Count; i++)
        {
            DisplayStep(i);
            
            yield return new WaitForSeconds(_timeStep);
        }
        
        _isLocked = false;
    }
}
