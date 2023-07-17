using Unity.Entities;
using Utilities;

namespace Components
{
    public struct CapturedComponent : IComponentData
    {
        public Constants.CellType PlayerCaptured;
        public int index;
    }
}