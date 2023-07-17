using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MoveComponent : IComponentData
    {
        public float3 From;
        public float3 To;
    }
}