using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Components
{
    public struct GameManagerComponent : IComponentData
    {
        public int Width;
        public int Height;
        public NativeArray<Constants.CellType> CellArray;
        public Constants.GameResult State;
    }
}