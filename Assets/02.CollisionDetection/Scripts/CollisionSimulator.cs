using System.Collections.Generic;
using UnityEngine;

public class CollisionData
{
    public Vector3 CollisionPoint { get; }

    public Vector3 ReflectionVelocity { get; }

    public CollisionData(Vector3 collisionPoint, Vector3 reflectionVelocity)
    {
        CollisionPoint = collisionPoint;
        ReflectionVelocity = reflectionVelocity;
    }
}

public class CollisionSimulator : MonoBehaviour
{
    [SerializeField] private Plane _targetPlane = null;
    
    [SerializeField] private float _ballRadius = 0.5f;

    public float BallRadius => _ballRadius;
    
    private Vector3 _collisionPoint = Vector3.zero;

    public CollisionData DetectCollision(List<MovementData> movementDatum)
    {
        for (int i = 0; i < movementDatum.Count - 1; i++)
        {
            float d_i = Vector3.Dot(movementDatum[i].Position, _targetPlane.PlaneNormal) - _ballRadius;
            
            float d_i1 = Vector3.Dot(movementDatum[i + 1].Position, _targetPlane.PlaneNormal) - _ballRadius;

            if (Mathf.Sign(d_i) * Mathf.Sign(d_i1) < 0)
            {
                float f = d_i / (d_i - d_i1);

                Vector3 distance = movementDatum[i + 1].Position -
                                   movementDatum[i].Position;

                _collisionPoint = movementDatum[i].Position + distance * f;

                Vector3 reflectVelocity = GetBounceBackSpeed(movementDatum[i].Velocity) +
                                       GetTangentSpeed(movementDatum[i].Velocity);
                
                return new CollisionData(_collisionPoint, reflectVelocity);
            }
        }

        return null;
    }

    public CollisionData DetectCollision(MovementData prev, MovementData next)
    {
        float d_i = Vector3.Dot(prev.Position, _targetPlane.PlaneNormal) - _ballRadius;
            
        float d_i1 = Vector3.Dot(next.Position, _targetPlane.PlaneNormal) - _ballRadius;

        if (Mathf.Sign(d_i) * Mathf.Sign(d_i1) < 0)
        {
            float f = d_i / (d_i - d_i1);

            Vector3 distance = next.Position - prev.Position;

            _collisionPoint = prev.Position + distance * f;

            Vector3 reflectVelocity = GetBounceBackSpeed(prev.Velocity) +
                                      GetTangentSpeed(prev.Velocity);
                
            return new CollisionData(_collisionPoint, reflectVelocity);
        }
        
        return null;
    }

    private Vector3 GetBounceBackSpeed(Vector3 velocity)
    {
        Vector3 bounceBackSpeed =
            -_targetPlane.RestituionCoef * Vector3.Dot(velocity, _targetPlane.PlaneNormal) * _targetPlane.PlaneNormal;


        return bounceBackSpeed;
    }

    private Vector3 GetTangentSpeed(Vector3 velocity)
    {
        Vector3 tangentSpeed = (1 - _targetPlane.FrictionCoef) *
                               (velocity - Vector3.Dot(velocity, _targetPlane.PlaneNormal) * _targetPlane.PlaneNormal);

        return tangentSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_collisionPoint, 1f);
    }
}
