namespace HelloECS.Boid
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    [UpdateBefore(typeof(BoidsSystemGroup))]
    public class BoidNeighbourDetectionSystem : JobComponentSystem
    {
        [BurstCompile]
        public struct BoidNeighbourDetectionJob : IJobProcessComponentDataWithEntity<Position, Velocity>
        {
            [ReadOnly] public float ThresholdAngle;
            [ReadOnly] public float ThresholdDistance;

            [ReadOnly] public EntityArray                               Entities;
            [ReadOnly] public ComponentDataFromEntity<Position>         PositionFromEntity;
            [ReadOnly] public BufferFromEntity<NeighborsEntityBuffer>   NeighboursFromEntity;

            public void Execute(Entity entity, int index, [ReadOnly] ref Position pos, [ReadOnly] ref Velocity vec)
            {
                // Clear current neighbour
                NeighboursFromEntity[entity].Clear();

                float angleThreshold    = math.cos(math.radians(ThresholdAngle));
                float3 entityPos        = pos.Value;
                float3 entityDir        = math.normalize(vec.Value);

                // Iterate all entities
                for (int i = 0; i < Entities.Length; ++i)
                {
                    // Ignore if self
                    var neighbor = Entities[i];
                    if (neighbor == entity) continue;

                    // Calculate distance between current and neighbour
                    float3 neighbourPos = PositionFromEntity[neighbor].Value;
                    float3 to           = neighbourPos - entityPos;
                    float dist          = math.length(to);

                    if (dist < ThresholdDistance)
                    {
                        // Check angle if we're in distance threshold
                        float3 neighbordir  = math.normalize(to);
                        float prod          = math.dot(entityDir, neighbordir);

                        // Add to current
                        if (prod > angleThreshold)
                        {
                            NeighboursFromEntity[entity].Add(new NeighborsEntityBuffer { Value = neighbor });
                        }
                    }
                }
            }
        }

        protected ComponentGroup _Group;

        protected override void OnCreateManager()
        {
            _Group = GetComponentGroup(typeof(Position), typeof(Velocity), typeof(NeighborsEntityBuffer));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new BoidNeighbourDetectionJob
            {
                ThresholdAngle          = JobBoidSceneController.BoidParameter.NeighbourFOV,
                ThresholdDistance       = JobBoidSceneController.BoidParameter.NeighbourDistance,
                Entities                = _Group.GetEntityArray(),
                PositionFromEntity      = GetComponentDataFromEntity<Position>(true),
                NeighboursFromEntity    = GetBufferFromEntity<NeighborsEntityBuffer>(false),
            };

            return job.Schedule(this, inputDeps);
        }
    }
}