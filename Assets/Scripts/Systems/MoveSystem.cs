using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utilities;

namespace Systems
{
    public partial struct MoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NextMoveComponent>();
            state.RequireForUpdate<PlayerTurnTagComponent>();
            state.RequireForUpdate<GameManagerComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var nextMove = SystemAPI.GetSingleton<NextMoveComponent>().NextMove;
            var currentPlayer = SystemAPI.GetSingletonEntity<PlayerTurnTagComponent>();
            var playerInfo = state.EntityManager.GetComponentData<PlayerInfoComponent>(currentPlayer);
            var gameManager = SystemAPI.GetSingleton<GameManagerComponent>();
            var gameTurn = SystemAPI.GetSingletonEntity<GameTurnComponent>();
            var currentIndex = playerInfo.positionIndex;
            var speed = 1;
            
            var destinationIndex = nextMove switch
            {
                Constants.Direction.Down => currentIndex - gameManager.Width,
                Constants.Direction.Up => currentIndex + gameManager.Height,
                Constants.Direction.Left => currentIndex - 1,
                Constants.Direction.Right => currentIndex + 1,
                _ => currentIndex
            };
            
            var direction = nextMove switch
            {
                Constants.Direction.Down => new float3(0,0,-1),
                Constants.Direction.Up => new float3(0,0,1),
                Constants.Direction.Left => new float3(-1,0,0),
                Constants.Direction.Right => new float3(1,0,0),
                _ => new float3(0,0,0)
            };
            
            var playerTransform = state.EntityManager.GetComponentData<LocalTransform>(currentPlayer);
            state.EntityManager.SetComponentData(currentPlayer, new LocalTransform()
            {
                Position = playerTransform.Position + direction * speed * SystemAPI.Time.DeltaTime,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            // playerTransform.Position += direction * speed * SystemAPI.Time.DeltaTime;
            var playerPosition = playerTransform.Position;
            var destinationPosition = Functions.GetCellPosition(gameManager.Width, gameManager.Height, destinationIndex);
            var distancesq = (destinationPosition.x - playerPosition.x) * (destinationPosition.x - playerPosition.x) +
                             (destinationPosition.z - playerPosition.z) * (destinationPosition.z - playerPosition.z);
            if (distancesq < 0.01f)
            {
                state.EntityManager.SetComponentData(currentPlayer, new LocalTransform()
                {
                    Position = destinationPosition + new float3(0,1,0),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                state.EntityManager.SetComponentData(currentPlayer, new PlayerInfoComponent
                {
                    id = playerInfo.id,
                    positionIndex = destinationIndex,
                    territories = playerInfo.territories,
                    type = playerInfo.type
                });
                state.EntityManager.RemoveComponent<NextMoveComponent>(currentPlayer);
                state.EntityManager.SetComponentData(gameTurn, new GameTurnComponent
                {
                    Turn = playerInfo.id == 1 ? 2 : 1
                });
                state.EntityManager.AddComponentData(currentPlayer, new CapturedComponent
                {
                    index = destinationIndex,
                    PlayerCaptured = playerInfo.type == Constants.TankType.Blue
                        ? Constants.CellType.Blue
                        : Constants.CellType.Red
                });
                state.EntityManager.RemoveComponent<PlayerTurnTagComponent>(currentPlayer);
                state.EntityManager.AddComponent<DetermineTurnState>(gameTurn);
            }
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}