using Unity.Entities;

namespace Components
{
    public struct PlayerPositionIndexComponent : IComponentData
    {
        public int Player1;
        public int Player2;
    }
}