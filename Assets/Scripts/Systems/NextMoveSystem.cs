using Components;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(NextPossibleMovesSystem))]
    public partial struct NextMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NextPossibleMovesComponent>();
            state.RequireForUpdate<PlayerTurnTagComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var currentPlayer = SystemAPI.GetSingletonEntity<PlayerTurnTagComponent>();
            var possibleMoves = SystemAPI.GetSingleton<NextPossibleMovesComponent>().PossibleMoves;
            var baseTime = System.DateTime.Now.TimeOfDay.TotalSeconds;
            var randomData = Unity.Mathematics.Random.CreateFromIndex((uint) baseTime);
            var nextMove = possibleMoves[randomData.NextInt(0, possibleMoves.Length)];
            state.EntityManager.RemoveComponent<NextPossibleMovesComponent>(currentPlayer);
            state.EntityManager.AddComponentData(currentPlayer, new NextMoveComponent
            {
                NextMove = nextMove
            });
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}