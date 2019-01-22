namespace HelloECS.Boid
{
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public class BoidAlignmentSystem : JobComponentSystem
    {
        [BurstCompile]
        public struct BoidAlignmentJob : IJobProcessComponentDataWithEntity<Velocity, Acceleration>
        {
            [ReadOnly] public float Weight;

            [ReadOnly] public BufferFromEntity<NeighborsEntityBuffer>   NeighboursFromEntity;
            [ReadOnly] public ComponentDataFromEntity<Velocity>         VelocityFromEntity;

            public void Execute(Entity entity, int index, [ReadOnly] ref Velocity velocity, ref Acceleration accel)
            {
                // Get neighbours
                var neighbors = NeighboursFromEntity[entity].Reinterpret<Entity>();
                if (neighbors.Length == 0) return;

                // Calc neighbours average velocity
                float3 averageVelocity = float3.zero;
                for (int i = 0; i < neighbors.Length; ++i)
                {
                    averageVelocity += VelocityFromEntity[neighbors[i]].Value;
                }
                averageVelocity /= neighbors.Length;

                // Align
                float3 accelIncrease    = (averageVelocity - velocity.Value) * Weight;
                accel                   = new Acceleration { Value = accel.Value + accelIncrease };
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new BoidAlignmentJob
            {
                Weight                  = JobBoidSceneController.BoidParameter.AlignmentWeight,
                NeighboursFromEntity    = GetBufferFromEntity<NeighborsEntityBuffer>(true),
                VelocityFromEntity      = GetComponentDataFromEntity<Velocity>(true),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}