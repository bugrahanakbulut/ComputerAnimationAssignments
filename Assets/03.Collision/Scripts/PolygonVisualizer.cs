using System.Collections.Generic;
using BLUE.PoolingSystem;
using UnityEngine;

public class PolygonVisualizer : MonoBehaviour
{
    [SerializeField] private BallFactory _ballFactory = null;
    
    private Pool<BallActivationInfo, BallPoolObject, BallFactory> _pool;

    private Pool<BallActivationInfo, BallPoolObject, BallFactory> _Pool
    {
        get
        {
            if (_pool == null)
                _pool = new Pool<BallActivationInfo, BallPoolObject, BallFactory>
                    (_ballFactory, 5);

            return _pool;
        }
    }

    public void DrawPolygon(List<Vector3> points)
    {
        // _Pool.DeactivateAll();
        
        if (points == null)
            return;
        
        foreach (Vector3 point in points)
        {
            _Pool.ActivatePoolObject(new BallActivationInfo(point));
        }
    }

    public void ResetPolygons()
    {
        _Pool.DeactivateAll();
    }
}
