using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonCollisionData : CollisionData
{
    public float HitTime { get; }
    
    public FaceData Face { get; }
    
    public PolygonCollisionData(
        Vector3 collisionPoint, 
        Vector3 reflectionVelocity,
        float hitTime,
        FaceData face) 
        : base(collisionPoint, reflectionVelocity)
    {
        HitTime = hitTime;
        Face = face;
    }
}

public class PolygonGeometryBasedCollisionSimulator : MonoBehaviour, 
    ICollisionSimulator
{
    [SerializeField] private List<Hedron> _hedrons = null;
    
    [SerializeField] private MovementSimulator _movementSimulator = null;

    [SerializeField] private float _hitTolerance = 0.00f;

    [SerializeField] private PolygonVisualizer _polygonVisualizer = null;

    public void ResetSim()
    {
        _polygonVisualizer.ResetPolygons();
    }

    public CollisionData DetectCollision(List<MovementData> movementRange)
    {
        for (int i = 0; i < movementRange.Count - 1; i++)
        {
            CollisionData data = DetectCollision(movementRange[i], movementRange[i + 1]);

            if (data != null)
            {
                return data;
            }
        }
        
        return null;
    }
    
    public CollisionData DetectCollision(MovementData prev, MovementData next)
    {
        List<PolygonCollisionData> collisionDatum = new List<PolygonCollisionData>();
        
        foreach (Hedron polygon in _hedrons)
        {
            foreach (FaceData plane in polygon.FaceDatum)
            {
                float hitTime = GetCollisionTime(plane, prev);
            
                if (hitTime >= 0 - _hitTolerance && hitTime <= _movementSimulator.TimeStep + _hitTolerance)
                {
                    Vector3 collisionPoint = GetCollisionPoint(prev, hitTime);
                
                    if (polygon.IsPointInPolygon(plane, collisionPoint))
                    {
                        Vector3 reflectVelocity = GetBounceBackSpeed(polygon, prev.Velocity, plane) +
                                                  GetTangentSpeed(polygon, prev.Velocity, plane);
                    
                        collisionDatum.Add(new PolygonCollisionData(collisionPoint, reflectVelocity, hitTime, plane));
                    }
                }
            }
        }
        
        if (collisionDatum.Count == 0)
            return null;
        
        collisionDatum.OrderBy(i => i.HitTime);

        _polygonVisualizer.DrawPolygon(collisionDatum[0].Face.FaceBounds);
        
        return collisionDatum[0];
    }
    
    private Vector3 GetBounceBackSpeed(Hedron polygon, Vector3 velocity, FaceData face)
    {
        Vector3 bounceBackSpeed =
            - polygon.RestituionCoef * Vector3.Dot(velocity, face.FaceNormal) * face.FaceNormal;


        return bounceBackSpeed;
    }

    private Vector3 GetTangentSpeed(Hedron polygon, Vector3 velocity, FaceData face)
    {
        Vector3 tangentSpeed = (1 - polygon.FrictionCoef) *
                               (velocity - Vector3.Dot(velocity, face.FaceNormal) * face.FaceNormal);

        return tangentSpeed;
    }

    private float GetCollisionTime(FaceData face, MovementData particleInitialMovement)
    {
        float dotProd = Vector3.Dot(face.FaceBounds[0] - particleInitialMovement.Position, face.FaceNormal);

        float divider = Vector3.Dot(particleInitialMovement.Velocity, face.FaceNormal);

        if (divider == 0)
            return -1f;
        
        float t_hit = dotProd / divider; 
        
        return t_hit;
    }

    private Vector3 GetCollisionPoint(MovementData particleMovementData, float hitTime)
    {
        return particleMovementData.Position + hitTime * particleMovementData.Velocity;
    }
}
