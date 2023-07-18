using Components;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems
{
    public partial struct NextPossibleMovesSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTurnTagComponent>();
            state.RequireForUpdate<GameManagerComponent>();
            state.RequireForUpdate<CalculateNextPossibleMovesState>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var currentPlayer = SystemAPI.GetSingletonEntity<PlayerTurnTagComponent>();
            var playerIndex = state.EntityManager.GetComponentData<PlayerInfoComponent>(currentPlayer).positionIndex;
            var gameManager = SystemAPI.GetSingleton<GameManagerComponent>();
            var gameState = SystemAPI.GetSingletonEntity<CalculateNextPossibleMovesState>();
            var moveList = new NativeList<Constants.Direction>(Allocator.Persistent);
            var upIndex = playerIndex + gameManager.Width;
            var downIndex = playerIndex - gameManager.Width;
            var leftIndex = playerIndex - 1;
            var rightIndex = playerIndex + 1;

            if (upIndex < gameManager.Width * gameManager.Height &&
                gameManager.CellArray[upIndex] == Constants.CellType.Empty)
            {
                moveList.Add(Constants.Direction.Up);
            }

            if (downIndex >= 0 && gameManager.CellArray[downIndex] == Constants.CellType.Empty)
            {
                moveList.Add(Constants.Direction.Down);
            }

            if (playerIndex % gameManager.Width != 0 && gameManager.CellArray[leftIndex] == Constants.CellType.Empty)
            {
                moveList.Add(Constants.Direction.Left);
            }

            if ((playerIndex + 1) % gameManager.Width != 0 && rightIndex < gameManager.Height * gameManager.Width && gameManager.CellArray[rightIndex] == Constants.CellType.Empty)
            {
                moveList.Add(Constants.Direction.Right);
            }

            state.EntityManager.RemoveComponent<CalculateNextPossibleMovesState>(gameState);

            state.EntityManager.AddComponentData(currentPlayer, new NextPossibleMovesComponent
            {
                PossibleMoves = moveList
            });
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}