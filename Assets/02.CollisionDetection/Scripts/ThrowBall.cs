using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    [SerializeField] private MovementSimulator _movementSimulator = null;
    
    [SerializeField] private Vector3 _offset = Vector3.zero;

    [SerializeField] private Rigidbody _rigidbody = null;
    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Throw();
        }
    }

    private void Throw()
    {
        _rigidbody.position = _movementSimulator.InitialPosition + _offset;

        _rigidbody.velocity = _movementSimulator.InitialVelocity;

        _rigidbody.isKinematic = false;

        _rigidbody.useGravity = true;
        
        _movementSimulator.StartMovementVisualizeRoutine();
    }
}
