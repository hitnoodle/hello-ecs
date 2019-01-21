namespace HelloECS.ECS
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Transforms;
    using UnityEngine;
    using Unity.Burst;
    using Unity.Mathematics;

    public class MovementSystem : JobComponentSystem
    {
        [BurstCompile]
        struct MovementJob : IJobProcessComponentData<Position, MoveSpeed, Bound>
        {
            public float DeltaTime;

            public void Execute(ref Position pos, ref MoveSpeed moveSpeed, [ReadOnly] ref Bound bound)
            {
                float3 value = pos.Value;

                if (value.z <= bound.Bottom || value.z >= bound.Top)
                {
                    moveSpeed.Z *= -1;
                }

                if (value.x <= bound.Left || value.x >= bound.Right)
                {
                    moveSpeed.X *= -1;
                }

                value.x += DeltaTime * moveSpeed.X;
                value.z += DeltaTime * moveSpeed.Z;

                pos.Value = value;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            MovementJob moveJob = new MovementJob
            {
                DeltaTime = Time.deltaTime
            };

            JobHandle moveHandle = moveJob.Schedule(this, inputDeps);

            return moveHandle;
        }
    }
}