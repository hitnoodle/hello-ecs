namespace HelloECS.ECS
{
    using Unity.Entities;

    [System.Serializable]
    public struct MoveSpeed : IComponentData
    {
        public float X;
        public float Z;
    }

    public class MoveSpeedComponent : ComponentDataWrapper<MoveSpeed> { }
}