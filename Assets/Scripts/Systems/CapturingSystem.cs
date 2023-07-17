using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;
using Utilities;

namespace Systems
{
    [UpdateAfter(typeof(MoveSystem))]
    public partial struct CapturingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CellComponent>();
            state.RequireForUpdate<GameManagerComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var cellArray = SystemAPI.GetSingleton<GameManagerComponent>().CellArray;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            // var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            // foreach (var (localTransform, playerInfo, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerInfoComponent>>().WithEntityAccess())
            // {
            //     var job = new PlayerCaptureJob
            //     {
            //         Player = entity,
            //         CellArray = cellArray,
            //         PlayerPosition = localTransform.ValueRO.Position,
            //         PlayerType = playerInfo.ValueRO.type,
            //         Ecb = ecb
            //     };
            //     state.Dependency = job.Schedule(state.Dependency);
            // }

            foreach (var (captured, entity) in SystemAPI.Query<RefRO<CapturedComponent>>().WithEntityAccess())
            {
                cellArray[captured.ValueRO.index] = captured.ValueRO.PlayerCaptured;
                ecb.RemoveComponent<CapturedComponent>(entity);
                ecb.AddComponent<AddTerritoriesScoreTag>(entity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    // public partial struct PlayerCaptureJob : IJobEntity
    // {
    //     public Entity Player;
    //     public float3 PlayerPosition;
    //     public Constants.TankType PlayerType;
    //     public NativeArray<Constants.CellType> CellArray;
    //     public EntityCommandBuffer Ecb;
    //     private void Execute(in LocalTransform localTransform, ref CellComponent cellComponent)
    //     {
    //         if (PlayerPosition.x == localTransform.Position.x &&
    //             PlayerPosition.z == localTransform.Position.z && cellComponent.State == Constants.CellType.Empty)
    //         {
    //             cellComponent.State = PlayerType == Constants.TankType.Blue ? Constants.CellType.Blue : Constants.CellType.Red;
    //             CellArray[cellComponent.Index] = cellComponent.State;
    //             Ecb.AddComponent<AddTerritoriesScoreTag>(Player);
    //         }
    //     }
    // }
}