using Unity.Collections;
using Unity.Entities;

namespace Components
{
    public struct GameManagerComponent : IComponentData
    {
        public NativeArray<Entity> cellArray;
    }
}