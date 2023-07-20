using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Utilities
{
    public static class Minimax
    {
        public static NativeList<Constants.Direction> NextMove(NativeArray<Constants.CellType> gameState, int width, int height, int player, int player1, int player2)
        {
            var start = Time.time;
            var newArray = new NativeArray<Constants.CellType>(gameState.Length, Allocator.Temp);
            newArray.CopyFrom(gameState);
            var moveList = new NativeList<Constants.Direction>(Allocator.Persistent);
            Constants.Direction bestMove;
            if (player == 1)
            {
                int value = Int32.MinValue;
                var nextMoves = Functions.PossibleMove(newArray, width, height, player1);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(width, height, player1, move);
                    newArray[index] = Constants.CellType.Blue;
                    int moveValue = MinimaxAlphaBeta(newArray, width, height, 7, 2, index, player2, Int32.MinValue, Int32.MaxValue);
                    if (moveValue > value)
                    {
                        value = moveValue;
                        moveList.Clear();
                        moveList.Add(move);
                    } else if (moveValue == value)
                    {
                        moveList.Add(move);
                    }
                }
            }
            else
            {
                // Debug.Log("Possible move");
                int value = Int32.MaxValue;
                var nextMoves = Functions.PossibleMove(newArray, width, height, player2);
                // Debug.Log("Next moves:" + nextMoves.Length);
                foreach (var move in nextMoves)
                {
                    int index = Functions.GetIndex(width, height, player2, move);
                    newArray[index] = Constants.CellType.Red;
                    int moveValue = MinimaxAlphaBeta(newArray, width, height, 12, 1, player1,index, Int32.MinValue, Int32.MaxValue);
                    // Debug.Log(move + ": " + moveValue);
                    if (moveValue < value) 
                    {
                        value = moveValue;
                        moveList.Clear();
                        moveList.Add(move);
                    } else if (moveValue == value)
                    {
                        if(!Constants.Direction.Down.In(moveList) && !Constants.Direction.Left.In(moveList)) moveList.Add(move);
                    }
                    newArray[index] = Constants.CellType.Empty;
                }
            }
            // Debug.Log((Time.time - start).ToString());
            newArray.Dispose();
            return moveList;
        }
    
        public static int MinimaxAlphaBeta(NativeArray<Constants.CellType> gameState, int width, int height, int depth, int player, int player1, int player2, int alpha, int beta)
        {
            if (depth == 0 || IsStuck(gameState, width, height, player1) || IsStuck(gameState, width, height, player2))
            {
                return EvaluateCell(gameState, width, height, player1, player2);
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
                    alpha = math.max(alpha, value);
                    if (beta <= alpha) break;
                }

                nextMoves.Dispose();
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
                    beta = math.min(beta, value);
                    if (beta <= alpha) break;
                }

                nextMoves.Dispose();
            }

            return value;
        }
    
        public static int EvaluateCell(NativeArray<Constants.CellType> gameState, int width, int height, int player1, int player2)
        {
            int score = 0;
            if (IsStuck(gameState, width, height, player1))
            {
                score += -1000;
                foreach (var cell in gameState)
                {
                    if (cell == Constants.CellType.Blue)
                    {
                        score += 1;
                    }
                }
            } 
            // if (IsCenter(width, height, player1))
            // {
            //     score += 50;
            // }

            var possibleMovePlayer1 = Functions.PossibleMove(gameState, width, height, player1);
            score += possibleMovePlayer1.Length;
            if (IsStuck(gameState, width, height, player2))
            {
                score += 1000;
                foreach (var cell in gameState)
                {
                    if (cell == Constants.CellType.Red)
                    {
                        score += -1;
                    }
                }
            } 
            // if (IsCenter(width, height, player2))
            // {
            //     score += -50;
            // }

            var possibleMovePlayer2 = Functions.PossibleMove(gameState, width, height, player2);
            if (possibleMovePlayer2.Length == 2)
            {
                score += -3;
            } else if (possibleMovePlayer2.Length == 1)
            {
                score += -2;
            }
            else if(possibleMovePlayer2.Length == 3)
            {
                score += -1;
            }

            var center = GetCenterCell(width, height);
            foreach (var cell in center)
            {
                if (gameState[cell] == Constants.CellType.Blue)
                {
                    score += 50;
                } else if (gameState[cell] == Constants.CellType.Red)
                {
                    score -= 50;
                }
            }

            if (Functions.IsAdjacent(width, player1, player2))
            {
                score = 0;
            }

            center.Dispose();
            possibleMovePlayer1.Dispose();
            possibleMovePlayer2.Dispose();
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

        public static NativeList<int> GetCenterCell(int width, int height)
        {
            var result = new NativeList<int>(Allocator.Persistent);
            if (height % 2 == 0)
            {
                if (width % 2 == 0)
                {
                    result.Add(height / 2 * width + width / 2 - 1);
                    result.Add(height / 2 * width + width / 2);
                    result.Add((height / 2 + 1) * width + width / 2 - 1);
                    result.Add((height / 2 + 1) * width + width / 2);
                }
                else
                {
                    result.Add(height / 2 * width + width / 2);
                    result.Add((height / 2 + 1) * width + width / 2);
                }
            }
            else
            {
                if (width % 2 == 0)
                {
                    result.Add(height / 2 * width + width / 2 - 1);
                    result.Add(height / 2 * width + width / 2);
                }
                else
                {
                    result.Add(height / 2 * width + width / 2);
                }
            }

            return result;
        }

        

        // public static NativeList<Constants.CellType> GetCenterBlock(int gameState, int width, int height)
        // {
        //     
        // }

        public static bool IsStuck(NativeArray<Constants.CellType> gameState, int width, int height, int index)
        {
            var possibleMove = Functions.PossibleMove(gameState, width, height, index);
            if (possibleMove.Length == 0)
            {
                return true;
            }

            possibleMove.Dispose();
            return false;
        }
        
        
    }
}