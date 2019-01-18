using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSceneController : MonoBehaviour
{
    [Header("Boid")]
    public Transform        BoundaryTransform;
    public BoidParameter    Parameter;

    [Header("Entities")]
    public GameObject EntityPrefab;
    public int EntityCount          = 0;
    public int EntityInitialCount   = 1000;
    public int EntityIncrement      = 500;

    [Header("UI")]
    public FPSText _FPS;

    private Boundary _Boundary;

    void Awake()
    {
        BoxCollider boundaryCollider    = BoundaryTransform.GetComponent<BoxCollider>();
        _Boundary                       = new Boundary(BoundaryTransform.position + boundaryCollider.center, boundaryCollider.size);

        // Make sure we start from 0
        Boid.AllBoids = new List<Boid>();
    }

    void Start()
    {
        AddEntity(EntityInitialCount);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddEntity(EntityIncrement);
        }
    }

    void AddEntity(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float xVal = Random.Range(_Boundary.Left    , _Boundary.Right);
            float yVal = Random.Range(_Boundary.Bottom  , _Boundary.Top);
            float zVal = Random.Range(_Boundary.Back    , _Boundary.Forward);

            GameObject entityObject         = Instantiate(EntityPrefab);
            entityObject.transform.position = new Vector3(xVal, yVal, zVal);
            entityObject.transform.rotation = Random.rotation;

            Boid boid = entityObject.GetComponent<Boid>();
            boid.Initialize(_Boundary, Parameter);

            Boid.AllBoids.Add(boid);
        }

        EntityCount += count;
        _FPS.UpdateCount(EntityCount);
    }
}
