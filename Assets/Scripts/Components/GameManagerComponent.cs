using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Components
{
    public struct GameManagerComponent : IComponentData
    {
        public NativeArray<Constants.CellType> CellArray;
        public Constants.GameResult state;
    }
}