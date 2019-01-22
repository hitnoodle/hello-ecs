using Unity.Entities;

namespace HelloECS.Boid
{
    [InternalBufferCapacity(4)]
    public unsafe struct NeighborsEntityBuffer : IBufferElementData
    {
        public Entity Value;
    }
}
