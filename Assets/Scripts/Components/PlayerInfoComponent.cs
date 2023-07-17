using Unity.Entities;
using Utilities;

namespace Components
{
    public struct PlayerInfoComponent : IComponentData
    {
        public int id;
        public int positionIndex;
        public Constants.TankType type;
        public int territories;
    }
}