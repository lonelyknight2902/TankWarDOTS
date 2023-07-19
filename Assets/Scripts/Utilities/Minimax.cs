using System;
using Unity.Collections;
using UnityEngine;

namespace Utilities
{
    public static class Minimax
    {
        public static NativeList<Constants.Direction> NextMove(NativeArray<Constants.CellType> gameState, int width, int height, int player, int player1, int player2)
        {
            var newArray = new NativeArray<Constants.CellType>(gameState.Length, Allocator.Temp);
            newArray.CopyFrom(gameState);
            var moveList = new NativeList<Constants.Direction>(Allocator.Persistent);
            Debug.Log(newArray[0]);
            Constants.Direction bestMove;
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
                Debug.Log("Possible move");
                int value = Int32.MaxValue;
                var nextMoves = Functions.PossibleMove(newArray, width, height, player2);
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
                        moveList.Clear();
                        moveList.Add(move);
                    } else if (moveValue == value)
                    {
                        moveList.Add(move);
                    }
                    newArray[index] = Constants.CellType.Empty;
                }
            }

            newArray.Dispose();
            return moveList;
        }
    
        public static int MinimaxAlphaBeta(NativeArray<Constants.CellType> gameState, int width, int height, int depth, int player, int player1, int player2, int alpha, int beta)
        {
            if (depth == 0 || IsStuck(gameState, width, height, player == 1 ? player1 : player2))
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
    
        public static int EvaluateCell(NativeArray<Constants.CellType> gameState, int width, int height, int player1, int player2)
        {
            int score = 0;
            if (IsStuck(gameState, width, height, player1))
            {
                score += -width*height;
                foreach (var cell in gameState)
                {
                    if (cell == Constants.CellType.Blue)
                    {
                        score += 1;
                    }
                }
            } 
            if (IsCenter(width, height, player1))
            {
                score += 50;
            }
            score += Functions.PossibleMove(gameState, width, height, player1).Length;
            if (IsStuck(gameState, width, height, player2))
            {
                score += width*height;
                foreach (var cell in gameState)
                {
                    if (cell == Constants.CellType.Red)
                    {
                        score += -1;
                    }
                }
            } 
            if (IsCenter(width, height, player2))
            {
                score += -50;
            }
            
            score += -Functions.PossibleMove(gameState, width, height, player2).Length;

            foreach (var cell in gameState)
            {
                if (cell == Constants.CellType.Blue)
                {
                    score += 1;
                }
                else if (cell == Constants.CellType.Red)
                {
                    score -= 1;
                }
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

        // public static NativeList<Constants.CellType> GetCenterBlock(int gameState, int width, int height)
        // {
        //     
        // }

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