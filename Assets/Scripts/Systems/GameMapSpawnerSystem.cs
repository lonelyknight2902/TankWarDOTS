using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities;

namespace Systems
{
    public partial struct GameMapSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameMapComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameMap = SystemAPI.GetSingleton<GameMapComponent>();
            var gameManager = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponent<GameManagerComponent>(gameManager);
            state.EntityManager.SetComponentData(gameManager, new GameManagerComponent
            {
                CellArray = new NativeArray<Constants.CellType>(gameMap.Width * gameMap.Height, Allocator.Persistent),
            });
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var job = new GameMapSpawnerJob
            {
                Ecb = ecb,
                Height = gameMap.Height,
                Width = gameMap.Width,
                Prefab = gameMap.Cell,
                // CellArray = state.EntityManager.GetComponentData<GameManagerComponent>(gameManager).CellArray,
            };

            state.Dependency = job.Schedule(state.Dependency);
            state.Enabled = false;
        }
    }

    public struct GameMapSpawnerJob : IJob
    {
        public int Height;
        public int Width;
        public Entity Prefab;
        public EntityCommandBuffer Ecb;
        // public NativeArray<Color> CellArray;

        public void Execute()
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    var newCell = Ecb.Instantiate(Prefab);
                    Ecb.SetComponent(newCell, new LocalTransform
                    {
                        Position = new float3(j, 0 , i),
                        Rotation = quaternion.identity,
                        Scale = 1f
                    });
                    Ecb.AddComponent(newCell, new CellComponent
                    {
                        Index = i * Width + j,
                        State = Constants.CellType.Empty
                    });
                    // CellArray[i * Width + j] = Color.Empty;
                }
            }
        }
    }
}
        