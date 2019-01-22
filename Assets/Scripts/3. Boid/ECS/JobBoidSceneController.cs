namespace HelloECS.Boid
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using UnityEngine;

    public class JobBoidSceneController : MonoBehaviour
    {
        public static BoidParameter BoidParameter;

        [Header("Boid")]
        public Transform        BoundaryTransform;
        public BoidParameter    Parameter;

        [Header("Entities")]
        public Vector3              EntityScale         = new Vector3(1, 1, 4);
        public MeshInstanceRenderer EntityRenderer;
        public int                  EntityCount          = 0;
        public int                  EntityInitialCount   = 1000;
        public int                  EntityIncrement      = 500;

        [Header("UI")]
        public FPSText _FPS;

        private Boundary        _Boundary;
        private EntityManager   _EntityManager;
        private EntityArchetype _EntityArchetype;

        void Awake()
        {
            BoxCollider boundaryCollider = BoundaryTransform.GetComponent<BoxCollider>();

            _Boundary           = new Boundary(BoundaryTransform.position + boundaryCollider.center, boundaryCollider.size);
            _EntityManager      = World.Active.GetOrCreateManager<EntityManager>();
            _EntityArchetype    = _EntityManager.CreateArchetype(
                                                                    typeof(Position),
                                                                    typeof(Rotation),
                                                                    typeof(Scale),
                                                                    typeof(Bound),
                                                                    typeof(Velocity),
                                                                    typeof(Acceleration),
                                                                    typeof(NeighborsEntityBuffer),
                                                                    typeof(MeshInstanceRenderer));

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
            Unity.Mathematics.Random random = new Unity.Mathematics.Random();
            random.InitState();

            for (int i = 0; i < count; i++)
            {
                float xVal = random.NextFloat(_Boundary.Left    , _Boundary.Right);
                float yVal = random.NextFloat(_Boundary.Bottom  , _Boundary.Top);
                float zVal = random.NextFloat(_Boundary.Back    , _Boundary.Forward);

                Entity entity = _EntityManager.CreateEntity(_EntityArchetype);

                _EntityManager.SetComponentData(entity, new Position        { Value = new float3(xVal, yVal, zVal) });
                _EntityManager.SetComponentData(entity, new Rotation        { Value = random.NextQuaternionRotation() });
                _EntityManager.SetComponentData(entity, new Scale           { Value = new float3(EntityScale.x, EntityScale.y, EntityScale.z) });
                _EntityManager.SetComponentData(entity, new Bound           { Top = _Boundary.Top, Bottom = _Boundary.Bottom, Left = _Boundary.Left, Right = _Boundary.Right, Forward = _Boundary.Forward, Back = _Boundary.Back });
                _EntityManager.SetComponentData(entity, new Velocity        { Value = random.NextFloat3Direction() * Parameter.InitialSpeed });
                _EntityManager.SetComponentData(entity, new Acceleration    { Value = float3.zero });

                _EntityManager.SetSharedComponentData(entity, EntityRenderer);
            }

            EntityCount += count;
            _FPS.UpdateCount(EntityCount);
        }
    }

}
