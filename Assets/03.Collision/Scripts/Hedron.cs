using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaceData
{
    public Vector3 FaceNormal { get; }
    
    public List<Vector3> FaceBounds { get; }

    public float D { get; }
    
    public FaceData(Vector3 faceNormal, List<Vector3> faceBounds, float d)
    {
        FaceNormal = faceNormal;
        FaceBounds = faceBounds;
        D = d;
    }
}

[ExecuteAlways]
public class Hedron : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;

    [Range(0, 1)] [SerializeField] private float _restituionCoef = 0; 
    public float RestituionCoef => _restituionCoef;
    
    [Range(0, 1)] [SerializeField] private float _frictionCoef = 0;
    public float FrictionCoef => _frictionCoef;

    private List<FaceData> _faceDatum;

    public List<FaceData> FaceDatum
    {
        get
        { 
            if (_faceDatum == null)
                InitFacesOnHedron();

            return _faceDatum;
        }
    }

    public bool IsPointInPolygon(FaceData face, Vector3 point)
    {
        List<float> values = new List<float>()
        {
            Mathf.Abs(face.FaceNormal.x),
            Mathf.Abs(face.FaceNormal.y), 
            Mathf.Abs(face.FaceNormal.z)
        };

        int maxValAxis = values.IndexOf(values.Max());

        Vector3 projectionPlane = Vector3.one;

        switch (maxValAxis)
        {
            case 0:
                projectionPlane.x = 0;
                break;
            case 1:
                projectionPlane.y = 0;
                break;
            case 2:
                projectionPlane.z = 0;
                break;
        }
        
        Vector2 projectedPoint = ProjectPoint(projectionPlane, point);
        
        List<Vector2> polygonPoints = new List<Vector2>();
        foreach (Vector3 planePoint in face.FaceBounds)
        {
            polygonPoints.Add(ProjectPoint(projectionPlane, planePoint));
        }

        polygonPoints = SortPolygonPoints(polygonPoints);
        
        List<Vector2> projectedEdgeVectors = new List<Vector2>();
        List<Vector2> xhitToVerticeVectors = new List<Vector2>();

        for (int i = 0; i < polygonPoints.Count; i++)
        {
            xhitToVerticeVectors.Add(projectedPoint - polygonPoints[i]);
            
            if (i == polygonPoints.Count - 1)
            {
                projectedEdgeVectors.Add(polygonPoints[0] - polygonPoints[i]);
                
                continue;
            }
            
            projectedEdgeVectors.Add(polygonPoints[i + 1] - polygonPoints[i]);
        }

        int sign = (int) Mathf.Sign((projectedEdgeVectors[0].x * xhitToVerticeVectors[0].y) - (projectedEdgeVectors[0].y * xhitToVerticeVectors[0].x));

        bool iter = sign > 0;
        
        for (int i = 0; i < projectedEdgeVectors.Count; i++)
        {
            float val1 = projectedEdgeVectors[i].x * xhitToVerticeVectors[i].y;

            float val2 =  xhitToVerticeVectors[i].x * projectedEdgeVectors[i].y;

            float det = val1 - val2;
            
            bool val = det > 0;

            if (iter != val)
                return false;
        }
        
        return true;
    }

    private void InitFacesOnHedron()
    {
        _faceDatum = new List<FaceData>();

        for (int i = 0; i < _meshFilter.sharedMesh.vertices.Length; i++)
        {
            Vector3 normal = transform.TransformDirection(_meshFilter.sharedMesh.normals[i]).normalized;
            Vector3 vertexWorldPos = transform.TransformPoint(_meshFilter.sharedMesh.vertices[i]);

            FaceData holder = _faceDatum.SingleOrDefault(planeData => planeData.FaceNormal == normal);

            if (holder == null)
            {
                float d = -Vector3.Dot(normal, vertexWorldPos);
                
                _faceDatum.Add(new FaceData(normal, new List<Vector3>() { vertexWorldPos }, d));
            }
            else
            {
                holder.FaceBounds.Add(vertexWorldPos);
            }
        }
    }

    private List<Vector2> SortPolygonPoints(List<Vector2> polygonPoints)
    {
        Vector2 centerPoint = Vector3.zero;

        foreach (Vector2 polygonPoint in polygonPoints)
        {
            centerPoint += polygonPoint;
        }

        centerPoint /= polygonPoints.Count;
        
        Dictionary<Vector3, float> angles = new Dictionary<Vector3, float>();
        foreach (Vector2 polygonPoint in polygonPoints)
        {
            float angle = Mathf.Atan2(polygonPoint.y - centerPoint.y, polygonPoint.x - centerPoint.x) * Mathf.Rad2Deg;

            if (angle < 0)
                angle += 360;

            angles.Add(polygonPoint, angle);
        }
        
        List<Vector2> sortedPoints = new List<Vector2>();

        foreach (KeyValuePair<Vector3,float> kvp in angles.OrderBy(key => key.Value).ToList())
        {
            sortedPoints.Add(kvp.Key);
        }
        
        return sortedPoints;
    }
    
    private Vector2 ProjectPoint(Vector3 plane, Vector3 point)
    {
        if (plane.x == 0)
            return new Vector2(point.y, point.z);
        
        if (plane.y == 0)
            return new Vector2(point.z, point.x);
        
        if (plane.z == 0)
            return new Vector2(point.x, point.y);
        
        return Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
        if (_faceDatum != null)
        {
            foreach (FaceData datum in _faceDatum)
            {
                Vector3 center = Vector3.zero;

                foreach (Vector3 planePoint in datum.FaceBounds)
                {
                    Gizmos.DrawSphere(planePoint, 0.25f);
                    
                    // Handles.Label(planePoint, datum.PlanePoints.IndexOf(planePoint).ToString());
                    
                    center += planePoint;
                }

                center /= datum.FaceBounds.Count;
                
                Gizmos.DrawSphere(center, 0.25f);

                Gizmos.DrawLine(center, center + datum.FaceNormal);
            }
        }
    }
}
