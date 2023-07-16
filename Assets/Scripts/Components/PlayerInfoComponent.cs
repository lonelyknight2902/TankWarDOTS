using Unity.Entities;
using Utilities;

namespace Components
{
    public struct PlayerInfoComponent : IComponentData
    {
        public Constants.TankType type;
        public int territories;
    }
}