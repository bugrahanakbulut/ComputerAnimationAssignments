using UnityEngine;

public class Plane : MonoBehaviour
{
    [SerializeField] private BoxCollider _boxCollider = null;
    
    [SerializeField] private Vector3 _planeNormal = Vector3.zero;
    public Vector3 PlaneNormal => _planeNormal.normalized;

    [Range(0, 1)] [SerializeField] private float _restituionCoef = 0.853f;
    public float RestituionCoef => _restituionCoef;

    [Range(0, 1)] [SerializeField] private float _frictionCoef = 0.00f;
    public float FrictionCoef => _frictionCoef;

    [SerializeField] private Material _planeMaterial = null;

    [SerializeField] private float _planeLength = 150;
    
    private void Awake()
    {
        InitPlane();
    }

    private void Update()
    {
        transform.up = PlaneNormal;

        _boxCollider.material.bounciness = _restituionCoef;
        _boxCollider.material.dynamicFriction = _frictionCoef;
        _boxCollider.material.staticFriction = _frictionCoef;
    }

    private void InitPlane()
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = _planeMaterial;
        
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-_planeLength, 0, -_planeLength),
            new Vector3(_planeLength, 0, -_planeLength),
            new Vector3(-_planeLength, 0, _planeLength),
            new Vector3(_planeLength, 0, _planeLength)
        };
        
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            Vector3.up,
            Vector3.up,
            Vector3.up,
            Vector3.up
        };
        
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        meshFilter.mesh = mesh;

        transform.up = PlaneNormal;
    }
}
