using Unity.Entities;
using Utilities;

namespace Components
{
    public struct NextMoveComponent : IComponentData
    {
        public Constants.Direction NextMove;
    }
}