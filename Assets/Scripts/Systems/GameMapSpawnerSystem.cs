using Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public partial struct GameMapSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameMapComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            GameMapComponent gameMap = SystemAPI.GetSingleton<GameMapComponent>();
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var job = new GameMapSpawnerJob
            {
                ecb = ecb,
                height = gameMap.height,
                width = gameMap.width,
                prefab = gameMap.cell
            };

            state.Dependency = job.Schedule(state.Dependency);


        }
    }

    public struct GameMapSpawnerJob : IJob
    {
        public int height;
        public int width;
        public Entity prefab;
        public EntityCommandBuffer ecb;

        public void Execute()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Entity newCell = ecb.Instantiate(prefab);
                    ecb.SetComponent(newCell, new LocalTransform
                    {
                        Position = new float3(j, 0 , i),
                        Rotation = quaternion.identity,
                        Scale = 1f,
                    });
                }
            }
        }
    }
}
        