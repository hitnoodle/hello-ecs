namespace HelloECS.Boid
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    public class JobBoidSceneController : MonoBehaviour
    {
        public static BoidParameter BoidParameter;

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

        private Boundary        _Boundary;
        private EntityManager   _EntityManager;

        void Awake()
        {
            BoxCollider boundaryCollider = BoundaryTransform.GetComponent<BoxCollider>();

            _Boundary       = new Boundary(BoundaryTransform.position + boundaryCollider.center, boundaryCollider.size);
            _EntityManager  = World.Active.GetOrCreateManager<EntityManager>();

            BoidParameter   = Parameter;
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
            NativeArray<Entity> entities    = new NativeArray<Entity>(count, Allocator.Temp);

            Unity.Mathematics.Random random = new Unity.Mathematics.Random();
            random.InitState();

            _EntityManager.Instantiate(EntityPrefab, entities);
            for (int i = 0; i < count; i++)
            {
                float xVal = random.NextFloat(_Boundary.Left    , _Boundary.Right);
                float yVal = random.NextFloat(_Boundary.Bottom  , _Boundary.Top);
                float zVal = random.NextFloat(_Boundary.Back    , _Boundary.Forward);

                _EntityManager.SetComponentData(entities[i], new Position   { Value = new float3(xVal, yVal, zVal) });
                _EntityManager.SetComponentData(entities[i], new Rotation   { Value = random.NextQuaternionRotation() });
                _EntityManager.SetComponentData(entities[i], new Bound      { Top = _Boundary.Top, Bottom = _Boundary.Bottom, Left = _Boundary.Left, Right = _Boundary.Right, Forward = _Boundary.Forward, Back = _Boundary.Back });
            }

            entities.Dispose();

            EntityCount += count;
            _FPS.UpdateCount(EntityCount);
        }
    }

}
