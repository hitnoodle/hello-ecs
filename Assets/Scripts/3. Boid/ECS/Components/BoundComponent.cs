namespace HelloECS.Boid
{
    using Unity.Entities;

    [System.Serializable]
    public struct Bound : IComponentData
    {
        public float Top;
        public float Bottom;
        public float Left;
        public float Right;
        public float Forward;
        public float Back;
    }

    public class BoundComponent : ComponentDataWrapper<Bound> { }
}