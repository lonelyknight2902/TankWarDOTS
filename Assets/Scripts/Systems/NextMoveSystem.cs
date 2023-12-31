﻿using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Windows;
using Utilities;
using Input = UnityEngine.Input;

namespace Systems
{
    [UpdateAfter(typeof(NextPossibleMovesSystem))]
    public partial struct NextMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NextPossibleMovesComponent>();
            state.RequireForUpdate<PlayerTurnTagComponent>();
            state.RequireForUpdate<PlayerPositionIndexComponent>();
            state.RequireForUpdate<GameManagerComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var possibleMoves = SystemAPI.GetSingleton<NextPossibleMovesComponent>().PossibleMoves;
            var currentPlayer = SystemAPI.GetSingletonEntity<PlayerTurnTagComponent>();
            if (SystemAPI.HasComponent<PlayerTagComponent>(currentPlayer))
            {
                if (Input.GetKey(KeyCode.LeftArrow) && Constants.Direction.Left.In(possibleMoves))
                {
                    possibleMoves.Dispose();
                    state.EntityManager.RemoveComponent<NextPossibleMovesComponent>(currentPlayer);
                    state.EntityManager.AddComponentData(currentPlayer, new NextMoveComponent
                    {
                        NextMove = Constants.Direction.Left
                    });
                } else if (Input.GetKey(KeyCode.RightArrow) && Constants.Direction.Right.In(possibleMoves))
                {
                    possibleMoves.Dispose();
                    state.EntityManager.RemoveComponent<NextPossibleMovesComponent>(currentPlayer);
                    state.EntityManager.AddComponentData(currentPlayer, new NextMoveComponent
                    {
                        NextMove = Constants.Direction.Right
                    });
                } else if (Input.GetKey(KeyCode.UpArrow) && Constants.Direction.Up.In(possibleMoves))
                {
                    possibleMoves.Dispose();
                    state.EntityManager.RemoveComponent<NextPossibleMovesComponent>(currentPlayer);
                    state.EntityManager.AddComponentData(currentPlayer, new NextMoveComponent
                    {
                        NextMove = Constants.Direction.Up
                    });
                } else if (Input.GetKey(KeyCode.DownArrow) && Constants.Direction.Down.In(possibleMoves))
                {
                    possibleMoves.Dispose();
                    state.EntityManager.RemoveComponent<NextPossibleMovesComponent>(currentPlayer);
                    state.EntityManager.AddComponentData(currentPlayer, new NextMoveComponent
                    {
                        NextMove = Constants.Direction.Down
                    });
                }
            } else if (SystemAPI.HasComponent<BotTagComponent>(currentPlayer))
            {
                var baseTime = System.DateTime.Now.TimeOfDay.TotalSeconds;
                var randomData = Unity.Mathematics.Random.CreateFromIndex((uint) baseTime);
                var nextMove = possibleMoves[randomData.NextInt(0, possibleMoves.Length)];
                possibleMoves.Dispose();
                var playerId = state.EntityManager.GetComponentData<PlayerInfoComponent>(currentPlayer).id;
                var gamePos = SystemAPI.GetSingleton<PlayerPositionIndexComponent>();
                state.EntityManager.RemoveComponent<NextPossibleMovesComponent>(currentPlayer);
                var gameState = SystemAPI.GetSingleton<GameManagerComponent>();
                var nextBestMove = Minimax.NextMove(gameState.CellArray, gameState.Width, gameState.Height, playerId,
                    gamePos.Player1, gamePos.Player2);
                if (nextBestMove.Length != 0)
                {
                    state.EntityManager.AddComponentData(currentPlayer, new NextMoveComponent
                    {
                        NextMove = nextBestMove[randomData.NextInt(0, nextBestMove.Length)]
                        // NextMove = nextMove
                    });
                }
                else
                {
                    Debug.Log("GameOver");
                }

                nextBestMove.Dispose();
                // var ecb = new EntityCommandBuffer(Allocator.Persistent);
                // var job = new NextMoveJob
                // {
                //     GameState = gameState.CellArray,
                //     Width = gameState.Width,
                //     Height = gameState.Height,
                //     Player = playerId,
                //     Player1 = gamePos.Player1,
                //     Player2 = gamePos.Player2,
                //     Ecb = ecb,
                //     PlayerEntity = currentPlayer
                // };
                //
                // state.Dependency = job.Schedule(state.Dependency);
                // state.Dependency.Complete();
                // ecb.Dispose();
            }
        }

        public void OnDestroy(ref SystemState state)
        {
            
        }
    }
}