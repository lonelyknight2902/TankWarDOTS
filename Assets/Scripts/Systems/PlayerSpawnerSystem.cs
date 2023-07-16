using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Utilities;

namespace Systems
{
    [UpdateAfter(typeof(GameMapSpawnerSystem))]
    public partial struct PlayerSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameMeshesComponent>();
            state.RequireForUpdate<GameMaterialsComponent>();
            state.RequireForUpdate<PlayerComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var playerPrefab = SystemAPI.GetSingleton<PlayerComponent>().Player;
            var material = SystemAPI.GetSingleton<GameMaterialsComponent>();
            var mesh = SystemAPI.GetSingleton<GameMeshesComponent>();
            var player1 = state.EntityManager.Instantiate(playerPrefab);
            var player2 = state.EntityManager.Instantiate(playerPrefab);
            state.EntityManager.SetComponentData(player1, new LocalTransform
            {
                Position = new float3(0,1,0),
                Rotation = quaternion.identity,
                Scale = 1f
            });

            state.EntityManager.SetComponentData(player1, new MaterialMeshInfo
            {
                MaterialID = material.Blue,
                MeshID = mesh.PlayerMesh
            });

            state.EntityManager.AddComponentData(player1, new PlayerInfoComponent
            {
                type = Constants.TankType.Blue,
                territories = 0
            });
            
            state.EntityManager.SetComponentData(player2, new LocalTransform
            {
                Position = new float3(9,1,9),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            
            state.EntityManager.SetComponentData(player2, new MaterialMeshInfo
            {
                MaterialID = material.Red,
                MeshID = mesh.PlayerMesh
            });
            
            state.EntityManager.AddComponentData(player2, new PlayerInfoComponent
            {
                type = Constants.TankType.Red,
                territories = 0
            });

            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}