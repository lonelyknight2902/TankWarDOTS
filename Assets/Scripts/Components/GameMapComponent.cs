using Unity.Entities;

namespace Components
{
    public struct GameMapComponent : IComponentData
    {
        public int Height;
        public int Width;
        public Entity Cell;
    }
}