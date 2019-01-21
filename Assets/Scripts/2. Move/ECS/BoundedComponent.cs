﻿namespace HelloECS.ECS
{
    using Unity.Entities;

    [System.Serializable]
    public struct Bound : IComponentData
    {
        public float Top;
        public float Bottom;
        public float Left;
        public float Right;
    }

    public class BoundedComponent : ComponentDataWrapper<Bound> { }
}
