using System;
using Unity.Collections;
using UnityEngine;

namespace Utilities
{
    public static class Minimax
    {
        public static Constants.Direction NextMove(NativeArray<Constants.CellType> gameState, int width, int height, int player, int player1, int player2)
        {
            Debug.Log(width + " " + height);
            var newArray = new NativeArray<Constants.CellType>(gameState.Length, Allocator.Persistent);
            newArray.CopyFrom(gameState);
            Debug.Log(newArray[0]);
            Constants.Direction bestMove = Constants.Direction.Down;
            if (player == 1)
            {
                int value = Int32.MinValue;
                var nextMoves = Functions.PossibleMove(newArray, width, height, player1);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(width, height, player1, move);
                    newArray[index] = Constants.CellType.Blue;
                    int moveValue = MinimaxAlphaBeta(newArray, width, height, 10, 2, index, player2, 10, 10);
                    if (moveValue > value)
                    {
                        value = moveValue;
                        bestMove = move;
                    }
                }
            }
            else
            {
                Debug.Log("Possible move");
                int value = Int32.MaxValue;
                Debug.Log(player2);
                var nextMoves = Functions.PossibleMove(newArray, width, height, player2);
                Debug.Log(newArray[player2 - 1] + " " + newArray[player2 - width]);
                Debug.Log("Next moves:" + nextMoves.Length);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(width, height, player2, move);
                    newArray[index] = Constants.CellType.Red;
                    int moveValue = MinimaxAlphaBeta(newArray, width, height, 10, 1, player1,index, 10, 10);
                    Debug.Log(move + ": " + moveValue);
                    if (moveValue < value) 
                    {
                        value = moveValue;
                        bestMove = move;
                    }

                    newArray[index] = Constants.CellType.Empty;
                }
            }

            newArray.Dispose();
            return bestMove;
        }
    
        public static int MinimaxAlphaBeta(NativeArray<Constants.CellType> gameState, int width, int height, int depth, int player, int player1, int player2, int alpha, int beta)
        {
            if (depth == 0)
            {
                return EvaluateCell(gameState, width, height, player, player1, player2);
            }

            int value;
            if (player == 1)
            {
                value = Int32.MinValue;
                var nextMoves = Functions.PossibleMove(gameState, width, height, player1);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(width, height, player1, move);
                    gameState[index] = Constants.CellType.Blue;
                    int moveValue = MinimaxAlphaBeta(gameState, width, height, depth - 1, 2, index, player2, alpha, beta);
                    if (moveValue > value) value = moveValue;
                    gameState[index] = Constants.CellType.Empty;
                }
            }
            else
            {
                value = Int32.MaxValue;
                var nextMoves = Functions.PossibleMove(gameState, width, height, player2);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(width, height, player2, move);
                    gameState[index] = Constants.CellType.Red;
                    int moveValue = MinimaxAlphaBeta(gameState, width, height, depth - 1, 1, player1,index, alpha, beta);
                    if (moveValue < value) value = moveValue;
                    gameState[index] = Constants.CellType.Empty;
                }
            }

            return value;
        }
    
        public static int EvaluateCell(NativeArray<Constants.CellType> gameState, int width, int height, int player, int player1, int player2)
        {
            int score = 0;
            if (player == 1)
            {
                if (IsStuck(gameState, width, height, player1))
                {
                    return -10;
                } 
                if (IsCenter(width, height, player1))
                {
                    score += 5;
                }
                else
                {
                    score = Functions.PossibleMove(gameState, width, height, player1).Length;
                }
            } else if (player == 2)
            {
                if (IsStuck(gameState, width, height, player1))
                {
                    return 10;
                } 
                if (IsCenter(width, height, player1))
                {
                    score += -5;
                }
                
                score += -Functions.PossibleMove(gameState, width, height, player1).Length;
            }

            return score;
        }

        public static bool IsCenter(int width, int height, int cell)
        {
            var pos = Functions.GetCellPositionInt2(width, height, cell);
            var centerX = width % 2 == 1 && pos.x == width / 2 ||
                           width % 2 == 0 && (pos.x == width / 2 || pos.x == width / 2 - 1);
            var centerY = height % 2 == 1 && pos.x == height / 2 ||
                           height % 2 == 0 && (pos.x == height / 2 || pos.x == height / 2 - 1);

            return centerX && centerY;
        }

        public static bool IsStuck(NativeArray<Constants.CellType> gameState, int width, int height, int index)
        {
            if (Functions.PossibleMove(gameState, width, height, index).Length == 0)
            {
                return true;
            }
            return false;
        }
        
        
    }
}