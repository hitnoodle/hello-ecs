using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class JobECSSceneController : MonoBehaviour
{
    [Header("Bounds")]
    public Transform TopLeft;
    public Transform TopRight;
    public Transform BottomLeft;
    public Transform BottomRight;

    [Header("Entities")]
    public GameObject EntityPrefab;
    public int EntityCount          = 0;
    public int EntityInitialCount   = 1000;
    public int EntityIncrement      = 500;
    public Vector2 SpeedMin;
    public Vector2 SpeedMax;

    [Header("UI")]
    public FPSText _FPS;

    private float _topBound;
    private float _bottomBound;
    private float _leftBound;
    private float _rightBound;

    private EntityManager _entityManager;

    private void Awake()
    {
        _topBound       = TopRight.position.z;
        _bottomBound    = BottomLeft.position.z;
        _leftBound      = BottomLeft.position.x;
        _rightBound     = TopRight.position.x;

        _entityManager = World.Active.GetOrCreateManager<EntityManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AddEntity(EntityInitialCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddEntity(EntityIncrement);
        }
    }

    void AddEntity(int count)
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
        _entityManager.Instantiate(EntityPrefab, entities);

        for (int i = 0; i < count; i++)
        {
            float xVal      = UnityEngine.Random.Range(_leftBound, _rightBound);
            float zVal      = UnityEngine.Random.Range(_bottomBound, _topBound);
            float xSpeed    = UnityEngine.Random.Range(SpeedMin.x, SpeedMax.x);
            float zSpeed    = UnityEngine.Random.Range(SpeedMin.y, SpeedMax.y);

            _entityManager.SetComponentData(entities[i], new Position { Value = new float3(xVal, 0, zVal) });
            _entityManager.SetComponentData(entities[i], new Rotation { Value = new quaternion(0, 1, 0, 0) });
            _entityManager.SetComponentData(entities[i], new MoveSpeed { X = xSpeed, Z = zSpeed });
            _entityManager.SetComponentData(entities[i], new Bound { Top = _topBound, Bottom = _bottomBound, Left = _leftBound, Right = _rightBound });
        }

        entities.Dispose();

        EntityCount += count;
        _FPS.UpdateCount(EntityCount);
    }
}
