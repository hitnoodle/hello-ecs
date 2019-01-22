namespace HelloECS.Boid
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Transforms;
    using UnityEngine;
    using Unity.Burst;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(BoidsSystemGroup))]
    public class BoidWallSystem : JobComponentSystem
    {
        [BurstCompile]
        struct BoidWallJob : IJobProcessComponentData<Position, Acceleration, Bound>
        {
            [ReadOnly] public float Threshold;
            [ReadOnly] public float Weight;

            public void Execute([ReadOnly] ref Position pos, ref Acceleration accel, [ReadOnly] ref Bound bound)
            {
                float3 accelUpdate  = accel.Value;

                accelUpdate +=  CalculateAccelAgainstBoundary(bound.Left    - pos.Value.x   , Vector3.right)    +
                                CalculateAccelAgainstBoundary(bound.Bottom  - pos.Value.y   , Vector3.up)       +
                                CalculateAccelAgainstBoundary(bound.Back    - pos.Value.z   , Vector3.forward)  +
                                CalculateAccelAgainstBoundary(bound.Right   - pos.Value.x   , Vector3.left)     +
                                CalculateAccelAgainstBoundary(bound.Top     - pos.Value.y   , Vector3.down)     +
                                CalculateAccelAgainstBoundary(bound.Forward - pos.Value.z   , Vector3.back);

                accel.Value = accelUpdate;
            }

            private float3 CalculateAccelAgainstBoundary(float distance, Vector3 direction)
            {
                if (distance < Threshold)
                {
                    return direction * (Weight / Mathf.Abs(distance / Threshold));
                }

                return float3.zero;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var wallJob = new BoidWallJob
            {
                Threshold   = JobBoidSceneController.BoidParameter.WallDistance,
                Weight      = JobBoidSceneController.BoidParameter.WallWeight,
            };

            return wallJob.Schedule(this, inputDeps);
        }
    }
}
