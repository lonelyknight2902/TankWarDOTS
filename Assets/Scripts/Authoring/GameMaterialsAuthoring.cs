using Components;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
    public class GameMaterialsAuthoring : MonoBehaviour
    {
        public Material empty;
        public Material blue;
        public Material red;
        public Material wall;

        public class GameMaterialsBaker : Baker<GameMaterialsAuthoring>
        {
            public override void Bake(GameMaterialsAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameMaterialsComponent
                {
                    Blue = hybridRenderer.RegisterMaterial(authoring.blue),
                    Red = hybridRenderer.RegisterMaterial(authoring.red),
                    Empty = hybridRenderer.RegisterMaterial(authoring.empty),
                    Wall = hybridRenderer.RegisterMaterial(authoring.wall)
                });
            }
        }
    }
}