namespace HelloECS.Boid
{
    using Unity.Mathematics;
    using Unity.Entities;

    [System.Serializable]
    public struct Acceleration : IComponentData
    {
        public float3 Value;
    }

    public class AccelerationComponent : ComponentDataWrapper<Acceleration> { }
}