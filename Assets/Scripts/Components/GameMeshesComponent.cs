using Unity.Entities;
using UnityEngine.Rendering;

namespace Components
{
    public struct GameMeshesComponent : IComponentData
    {
        public BatchMeshID PlayerMesh;
    }
}