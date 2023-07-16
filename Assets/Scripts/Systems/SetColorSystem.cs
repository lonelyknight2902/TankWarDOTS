using Components;
using Unity.Entities;
using Unity.VisualScripting;

namespace Systems
{
    [UpdateAfter(typeof(GameMapSpawnerSystem))]
    public partial struct SetColorSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameManagerComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var gameManager = SystemAPI.GetSingleton<GameManagerComponent>();
            var cellArray = gameManager.CellArray;
            foreach (var cell in SystemAPI.Query<RefRW<CellComponent>>())
            {
                cell.ValueRW.State = cellArray[cell.ValueRO.Index];
            }
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}