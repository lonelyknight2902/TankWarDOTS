using Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(PlayerSpawnerSystem))]
    public partial struct GameTurnSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameTurnComponent>();
            state.RequireForUpdate<DetermineTurnState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameState = SystemAPI.GetSingletonEntity<DetermineTurnState>();
            var turn = SystemAPI.GetSingleton<GameTurnComponent>().Turn;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (info, entity) in SystemAPI.Query<RefRO<PlayerInfoComponent>>().WithEntityAccess())
            {
                if (info.ValueRO.id == turn)
                {
                    ecb.AddComponent<PlayerTurnTagComponent>(entity);
                }
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            state.EntityManager.RemoveComponent<DetermineTurnState>(gameState);
            state.EntityManager.AddComponent<CalculateNextPossibleMovesState>(gameState);
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}