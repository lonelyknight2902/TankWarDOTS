using Components;
using Unity.Entities;
using Unity.Rendering;
using Utilities;

namespace Systems
{
    public partial struct CellStateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameMaterialsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var materials = SystemAPI.GetSingleton<GameMaterialsComponent>();
            foreach (var (cellState, cellMaterial) in SystemAPI.Query<RefRO<CellComponent>, RefRW<MaterialMeshInfo>>())
            {
                cellMaterial.ValueRW.MaterialID = cellState.ValueRO.State switch
                {
                    Constants.CellType.Empty =>  materials.Empty,
                    Constants.CellType.Blue => materials.Blue,
                    Constants.CellType.Red => materials.Red,
                    Constants.CellType.Wall => materials.Wall,
                    _ => materials.Empty
                };
            }
        }
    }
}