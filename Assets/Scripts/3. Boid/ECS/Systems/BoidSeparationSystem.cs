namespace HelloECS.Boid
{
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;

    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public class BoidSeparationSystem : JobComponentSystem
    {
        [BurstCompile]
        public struct BoidSeparationJob : IJobProcessComponentDataWithEntity<Position, Acceleration>
        {
            [ReadOnly] public float Weight;

            [ReadOnly] public BufferFromEntity<NeighborsEntityBuffer>   NeighboursFromEntity;
            [ReadOnly] public ComponentDataFromEntity<Position>         PositionFromEntity;

            public void Execute(Entity entity, int index, [ReadOnly] ref Position pos, ref Acceleration accel)
            {
                // Get neighbours
                var neighbors = NeighboursFromEntity[entity].Reinterpret<Entity>();
                if (neighbors.Length == 0) return;

                // Calc separation force
                float3 posValue = pos.Value;
                float3 force    = float3.zero;
                for (int i = 0; i < neighbors.Length; ++i)
                {
                    float3 neighbourPos = PositionFromEntity[neighbors[i]].Value;
                    force               += math.normalize(posValue - neighbourPos);
                }
                force /= neighbors.Length;

                // Separate
                float3 accelIncrease    = force * Weight;
                accel                   = new Acceleration { Value = accel.Value + accelIncrease };
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new BoidSeparationJob
            {
                Weight                  = JobBoidSceneController.BoidParameter.SeparationWeight,
                NeighboursFromEntity    = GetBufferFromEntity<NeighborsEntityBuffer>(true),
                PositionFromEntity      = GetComponentDataFromEntity<Position>(true),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}