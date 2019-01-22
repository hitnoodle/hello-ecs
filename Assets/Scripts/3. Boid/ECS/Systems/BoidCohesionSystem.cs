namespace HelloECS.Boid
{
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;

    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public class BoidCohesionSystem : JobComponentSystem
    {
        [BurstCompile]
        public struct BoidCohesionJob : IJobProcessComponentDataWithEntity<Position, Acceleration>
        {
            [ReadOnly] public float Weight;

            [ReadOnly] public BufferFromEntity<NeighborsEntityBuffer>   NeighboursFromEntity;
            [ReadOnly] public ComponentDataFromEntity<Position>         PositionFromEntity;

            public void Execute(Entity entity, int index, [ReadOnly] ref Position pos, ref Acceleration accel)
            {
                // Get neighbours
                var neighbors = NeighboursFromEntity[entity].Reinterpret<Entity>();
                if (neighbors.Length == 0) return;

                // Calc average position
                var averagePos = float3.zero;
                for (int i = 0; i < neighbors.Length; ++i)
                {
                    averagePos += PositionFromEntity[neighbors[i]].Value;
                }
                averagePos /= neighbors.Length;

                // Move to there
                var accelIncrease   = (averagePos - pos.Value) * Weight;
                accel               = new Acceleration { Value = accel.Value + accelIncrease };
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new BoidCohesionJob
            {
                Weight                  = JobBoidSceneController.BoidParameter.CohesionWeight,
                NeighboursFromEntity    = GetBufferFromEntity<NeighborsEntityBuffer>(true),
                PositionFromEntity      = GetComponentDataFromEntity<Position>(true),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}