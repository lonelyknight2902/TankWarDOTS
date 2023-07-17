using Unity.Entities;

namespace Components
{
    public struct GameTurnComponent : IComponentData
    {
        public int Turn;
    }
}