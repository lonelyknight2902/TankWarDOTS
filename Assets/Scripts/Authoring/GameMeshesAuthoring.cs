using Components;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
    public class GameMeshesAuthoring : MonoBehaviour
    {
        public Mesh playerMesh;

        public class GameMeshesBaker : Baker<GameMeshesAuthoring>
        {
            public override void Bake(GameMeshesAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameMeshesComponent
                {
                    PlayerMesh = hybridRenderer.RegisterMesh(authoring.playerMesh)
                });
            }
        }
    }
}