namespace HelloECS.Boid
{
    using Unity.Mathematics;
    using Unity.Entities;

    [System.Serializable]
    public struct Velocity : IComponentData
    {
        public float3 Value;
    }

    public class VelocityComponent : ComponentDataWrapper<Velocity> { }
}