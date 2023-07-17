using Components;
using Unity.Collections;
using Unity.Entities;

namespace Systems
{
    public partial struct AddTerritoriesScoreSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AddTerritoriesScoreTag>();
            state.RequireForUpdate<GameManagerComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameManager = SystemAPI.GetSingletonEntity<GameManagerComponent>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (playerInfo, entity) in SystemAPI.Query<RefRW<PlayerInfoComponent>>().WithEntityAccess().WithAll<AddTerritoriesScoreTag>())
            {
                playerInfo.ValueRW.territories += 1;
                ecb.RemoveComponent<AddTerritoriesScoreTag>(entity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            // var playerInfo = state.EntityManager.GetComponentData<PlayerInfoComponent>(player);
            
            // state.EntityManager.SetComponentData(player, new PlayerInfoComponent
            // {
            //     id = playerInfo.id,
            //     positionIndex = playerInfo.positionIndex,
            //     territories = playerInfo.territories + 1,
            //     type = playerInfo.type
            // });
            //
            // state.EntityManager.SetComponentData(gameManager, new GameTurnComponent
            // {
            //     Turn = playerInfo.id == 1 ? 2 : 1
            // });
            // state.EntityManager.RemoveComponent<PlayerTurnTagComponent>(player);
            // state.EntityManager.AddComponent<DetermineTurnState>(gameManager);
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}