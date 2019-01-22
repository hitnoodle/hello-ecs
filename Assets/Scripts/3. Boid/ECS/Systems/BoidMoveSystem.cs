namespace HelloECS.Boid
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Transforms;
    using UnityEngine;
    using Unity.Burst;
    using Unity.Mathematics;

    [UpdateAfter(typeof(BoidsSystemGroup))]
    public class BoidMoveSystem : JobComponentSystem
    {
        [BurstCompile]
        struct BoidMoveJob : IJobProcessComponentData<Position, Rotation, Velocity, Acceleration>
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public float MinSpeed;
            [ReadOnly] public float MaxSpeed;

            public void Execute(ref Position pos, ref Rotation rot, ref Velocity vec, ref Acceleration accel)
            {
                float3 vecValue     = vec.Value;
                float3 posValue     = pos.Value;
                quaternion rotValue = rot.Value;
                float3 accelValue   = accel.Value;

                vecValue += accelValue * DeltaTime;

                float3 direction    = math.normalize(vecValue);
                float speed         = math.length(vecValue);

                vecValue = math.clamp(speed, MinSpeed, MaxSpeed) * direction;
                posValue += vecValue * DeltaTime;
                rotValue = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));

                vec.Value   = vecValue;
                pos.Value   = posValue;
                rot.Value   = rotValue;
                accel.Value = float3.zero;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var moveJob = new BoidMoveJob
            {
                DeltaTime   = Time.deltaTime,
                MinSpeed    = JobBoidSceneController.BoidParameter.MinSpeed,
                MaxSpeed    = JobBoidSceneController.BoidParameter.MaxSpeed,
            };

            return moveJob.Schedule(this, inputDeps);
        }
    }
}