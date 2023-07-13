using Unity.Entities;

namespace Components
{
    public struct GameMapComponent : IComponentData
    {
        public int height;
        public int width;
        public Entity cell;
    }
}