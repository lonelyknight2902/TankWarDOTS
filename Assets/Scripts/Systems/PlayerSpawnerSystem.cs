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
            var game = SystemAPI.GetSingleton<GameMapComponent>();
            var player1 = state.EntityManager.Instantiate(playerPrefab);
            var player2 = state.EntityManager.Instantiate(playerPrefab);
            state.EntityManager.SetComponentData(player1, new LocalTransform
            {
                Position = Functions.GetCellPosition(game.Width, game.Height, 0) + new float3(0,1,0),
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
                id = 1,
                positionIndex = 0,
                type = Constants.TankType.Blue,
                territories = 0
            });

            state.EntityManager.AddComponentData(player1, new CapturedComponent
            {
                index = 0,
                PlayerCaptured = Constants.CellType.Blue
            });

            // state.EntityManager.AddComponent<NextPossibleMovesComponent>(player1);
            
            state.EntityManager.SetComponentData(player2, new LocalTransform
            {
                Position = Functions.GetCellPosition(game.Width, game.Height, game.Width * game.Height - 1) + new float3(0,1,0),
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
                id = 2,
                positionIndex = game.Width * game.Height - 1,
                type = Constants.TankType.Red,
                territories = 0
            });
            
            state.EntityManager.AddComponentData(player1, new CapturedComponent
            {
                index = game.Width * game.Height - 1,
                PlayerCaptured = Constants.CellType.Red
            });
            
            // state.EntityManager.AddComponent<NextPossibleMovesComponent>(player1);

            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}