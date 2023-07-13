using Unity.Entities;
using Utilities;

namespace Components
{
    public struct CellComponent : IComponentData
    {
        public int Index;
        public Constants.CellType State;
    }
    
    
}