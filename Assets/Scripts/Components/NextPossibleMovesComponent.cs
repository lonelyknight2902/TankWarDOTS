using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Components
{
    public struct NextPossibleMovesComponent : IComponentData
    {
        public NativeList<Constants.Direction> PossibleMoves;
    }
}