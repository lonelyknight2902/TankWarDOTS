using System;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Utilities
{
    public struct NextMoveJob : IJob
    {
        public NativeArray<Constants.CellType> GameState;
        public int Width;
        public int Height;
        public int Player;
        public int Player1;
        public int Player2;
        public EntityCommandBuffer Ecb;
        public Entity PlayerEntity;
        
        public void Execute()
        {
            var newArray = new NativeArray<Constants.CellType>(GameState.Length, Allocator.Temp);
            newArray.CopyFrom(GameState);
            var moveList = new NativeList<Constants.Direction>(Allocator.Persistent);
            Constants.Direction bestMove;
            NativeList<int> result = new NativeList<int>(Allocator.Temp);
            if (Player == 1)
            {
                int value = Int32.MinValue;
                var nextMoves = Functions.PossibleMove(newArray, Width, Height, Player1);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(Width, Height, Player1, move);
                    newArray[index] = Constants.CellType.Blue;
                    // int moveValue = MinimaxAlphaBeta(newArray, width, height, 5, 2, index, player2, 10, 10);
                    new MinimaxJob
                    {
                        GameState = newArray,
                        Width = Width,
                        Height = Height,
                        Depth = 5,
                        Player = 2,
                        Player1 = index,
                        Player2 = Player2,
                        Alpha = Int32.MinValue,
                        Beta = Int32.MaxValue,
                        ReturnVal = result
                    }.Run();
                    // minimaxJob.Complete();
                    if (result[0] > value)
                    {
                        value = result[0];
                        moveList.Clear();
                        moveList.Add(move);
                    } else if (result[0] == value)
                    {
                        moveList.Add(move);
                    }
                }
            }
            else
            {
                Debug.Log("Possible move");
                int value = Int32.MaxValue;
                var nextMoves = Functions.PossibleMove(newArray, Width, Height, Player2);
                Debug.Log("Next moves:" + nextMoves.Length);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(Width, Height, Player2, move);
                    newArray[index] = Constants.CellType.Red;
                    new MinimaxJob
                    {
                        GameState = newArray,
                        Width = Width,
                        Height = Height,
                        Depth = 5,
                        Player = 1,
                        Player1 = index,
                        Player2 = Player2,
                        Alpha = Int32.MinValue,
                        Beta = Int32.MaxValue,
                        ReturnVal = result
                    }.Run();
                    // minimaxJob.Complete();
                    Debug.Log(move + ": " + result[0]);
                    if (result[0] < value) 
                    {
                        value = result[0];
                        moveList.Clear();
                        moveList.Add(move);
                    } else if (result[0] == value)
                    {
                        moveList.Add(move);
                    }
                    newArray[index] = Constants.CellType.Empty;
                }
            }

            if (moveList.Length > 0)
            {
                var baseTime = System.DateTime.Now.TimeOfDay.TotalSeconds;
                var randomData = Unity.Mathematics.Random.CreateFromIndex((uint) baseTime);
                Ecb.AddComponent(PlayerEntity, new NextMoveComponent
                {
                    NextMove = moveList[randomData.NextInt(0, moveList.Length)]
                    // NextMove = nextMove
                });
            }

            moveList.Dispose();
            newArray.Dispose();
            result.Dispose();
        }
    }

    public struct MinimaxJob : IJob
    {
        public NativeArray<Constants.CellType> GameState;
        public int Width;
        public int Height;
        public int Depth;
        public int Player;
        public int Player1;
        public int Player2;
        public int Alpha;
        public int Beta;
        public NativeList<int> ReturnVal;

        public void Execute()
        {
            if (Depth == 0 || Minimax.IsStuck(GameState, Width, Height, Player == 1 ? Player1 : Player2))
            {
                Minimax.EvaluateCell(GameState, Width, Height, Player1, Player2);
            }

            int value;
            NativeList<int> result = new NativeList<int>(Allocator.Temp);
            if (Player == 1)
            {
                // value = Int32.MinValue;
                ReturnVal[0] = Int32.MinValue;
                var nextMoves = Functions.PossibleMove(GameState, Width, Height, Player1);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(Width, Height, Player1, move);
                    GameState[index] = Constants.CellType.Blue;
                    // int moveValue = MinimaxAlphaBeta(gameState, width, height, depth - 1, 2, index, player2, alpha, beta);
                    new MinimaxJob
                    {
                        GameState = GameState,
                        Width = Width,
                        Height = Height,
                        Depth = 5,
                        Player = 2,
                        Player1 = index,
                        Player2 = Player2,
                        Alpha = Alpha,
                        Beta = Beta,
                        ReturnVal = result
                    }.Run();
                    if (result[0] > ReturnVal[0]) ReturnVal[0] = result[0];
                    GameState[index] = Constants.CellType.Empty;
                    Alpha = math.max(Alpha, ReturnVal[0]);
                    if (Beta <= Alpha) break;
                }
            }
            else
            {
                ReturnVal[0] = Int32.MaxValue;
                var nextMoves = Functions.PossibleMove(GameState, Width, Height, Player2);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(Width, Height, Player2, move);
                    GameState[index] = Constants.CellType.Red;
                    // int moveValue = MinimaxAlphaBeta(gameState, width, height, depth - 1, 1, player1,index, alpha, beta);
                    new MinimaxJob
                    {
                        GameState = GameState,
                        Width = Width,
                        Height = Height,
                        Depth = 5,
                        Player = 2,
                        Player1 = index,
                        Player2 = Player2,
                        Alpha = Alpha,
                        Beta = Beta,
                        ReturnVal = result
                    }.Run();
                    if (result[0] < ReturnVal[0]) ReturnVal[0] = result[0];
                    GameState[index] = Constants.CellType.Empty;
                    Beta = math.min(Beta, ReturnVal[0]);
                    if (Beta <= Alpha) break;
                }
            }

            result.Dispose();
        }
    }
}