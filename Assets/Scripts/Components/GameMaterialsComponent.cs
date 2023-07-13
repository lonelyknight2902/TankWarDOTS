using Unity.Entities;
using UnityEngine.Rendering;

namespace Components
{
    public struct GameMaterialsComponent : IComponentData
    {
        public BatchMaterialID Empty;
        public BatchMaterialID Blue;
        public BatchMaterialID Red;
        public BatchMaterialID Wall;
    }
}