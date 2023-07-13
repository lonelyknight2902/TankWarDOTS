using Unity.Entities;

namespace Components
{
    public struct CellComponent : IComponentData
    {
        public int index;
        public Color state;
    }
    
    public enum Color
    {
        Empty,
        Wall,
        Red,
        Blue,
    }
    
}