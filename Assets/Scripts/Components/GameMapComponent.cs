using Unity.Entities;

namespace Components
{
    public struct GameMapComponent : IComponentData
    {
        public int Height;
        public int Width;
        public int Wall;
        public Entity CellPrefab;
        public Entity WallPrefab;
    }
}