using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Rendering;
using Utilities;

namespace Systems
{
    public partial struct CapturingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CellComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var cellArray = SystemAPI.GetSingleton<GameManagerComponent>().CellArray;
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (localTransform, playerInfo) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerInfoComponent>>())
            {
                var job = new PlayerCaptureJob
                {
                    CellArray = cellArray,
                    PlayerPosition = localTransform.ValueRO.Position,
                    PlayerType = playerInfo.ValueRO.type,
                    Ecb = ecb
                };
                state.Dependency = job.Schedule(state.Dependency);
            }
        }
    }

    public partial struct PlayerCaptureJob : IJobEntity
    {
        public float3 PlayerPosition;
        public Constants.TankType PlayerType;
        public NativeArray<Constants.CellType> CellArray;
        public EntityCommandBuffer Ecb;
        private void Execute(in LocalTransform localTransform, ref CellComponent cellComponent)
        {
            if (PlayerPosition.x == localTransform.Position.x &&
                PlayerPosition.z == localTransform.Position.z && cellComponent.State == Constants.CellType.Empty)
            {
                cellComponent.State = PlayerType == Constants.TankType.Blue ? Constants.CellType.Blue : Constants.CellType.Red;
                CellArray[cellComponent.Index] = cellComponent.State;
            }
        }
    }
}