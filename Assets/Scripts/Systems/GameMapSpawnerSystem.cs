using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

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
            var cellArray = new NativeArray<Constants.CellType>(gameMap.Width * gameMap.Height, Allocator.Persistent);
            var baseTime = System.DateTime.Now.TimeOfDay.TotalSeconds;
            state.EntityManager.AddComponent<GameManagerComponent>(gameManager);
            state.EntityManager.SetComponentData(gameManager, new GameManagerComponent
            {
                CellArray = cellArray,
                state = Constants.GameResult.Playing
            });
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var job = new GameMapSpawnerJob
            {
                Ecb = ecb,
                Height = gameMap.Height,
                Width = gameMap.Width,
                CellPrefab = gameMap.CellPrefab,
                WallPrefab = gameMap.WallPrefab
            };

            var populateJob = new PopulateCellArrayJob
            {
                cellArray = cellArray
            };

            var wallJob = new WallSpawnerJob
            {
                cellArray = cellArray,
                Ecb = ecb,
                Height = gameMap.Height,
                WallPrefab = gameMap.WallPrefab,
                Walls = gameMap.Wall,
                Width = gameMap.Width,
                seed = (int) (float) (baseTime + SystemAPI.Time.ElapsedTime),
            };
            
            state.Dependency = populateJob.ScheduleParallel(cellArray.Length, 100, state.Dependency);
            state.Dependency = job.Schedule(state.Dependency);
            state.Dependency = wallJob.Schedule(state.Dependency);
            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }

    public struct PopulateCellArrayJob : IJobFor
    {
        public NativeArray<Constants.CellType> cellArray;
        public void Execute(int index)
        {
            cellArray[index] = Constants.CellType.Empty;
        }
    }

    public struct GameMapSpawnerJob : IJob
    {
        public int Height;
        public int Width;
        public Entity CellPrefab;
        public Entity WallPrefab;
        public EntityCommandBuffer Ecb;

        public void Execute()
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    var newCell = Ecb.Instantiate(CellPrefab);
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
                }
            }
        }
    }

    public struct WallSpawnerJob : IJob
    {
        public int Height;
        public int Width;
        public int Walls;
        public Entity WallPrefab;
        public NativeArray<Constants.CellType> cellArray;
        public EntityCommandBuffer Ecb;
        public int seed;

        public void Execute()
        {
            var randomData = Unity.Mathematics.Random.CreateFromIndex((uint) seed);
            for (var i = 0; i < Walls / 2; i++)
            {
                var x = 0;
                var y = 0;
                while (x == 0 && y == 0)
                {
                    x = randomData.NextInt(0, Width / 2);
                    y = randomData.NextInt(0, Height);
                }
                var wall1 = Ecb.Instantiate(WallPrefab);
                var wall2 = Ecb.Instantiate(WallPrefab);
                Ecb.SetComponent(wall1, new LocalTransform
                {
                    Position = new float3(x, 1, y),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                Ecb.SetComponent(wall2, new LocalTransform
                {
                    Position = new float3(Width - 1 - x, 1, Height - 1 - y),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                cellArray[y * Width + x] = Constants.CellType.Wall;
                cellArray[(Height - y) * Width - 1 - x] = Constants.CellType.Wall;
            }
        }
    }
}
        