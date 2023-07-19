using Unity.Collections;
using Unity.Mathematics;

namespace Utilities
{
    public static class Functions
    {
        public static float3 GetCellPosition(int width, int height, int index)
        {
            return new float3(index % width, 0, (int) index / height);
        }
        
        public static int2 GetCellPositionInt2(int width, int height, int index)
        {
            return new int2(index % width,  (int) index / height);
        }
        
        public static bool In(this Constants.Direction val, NativeList<Constants.Direction> values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (val == values[i]) return true;
            }

            return false;
        }

        public static int GetIndex(int width, int height, int index, Constants.Direction direction)
        {
            return direction switch
            {
                Constants.Direction.Up => index + width,
                Constants.Direction.Down => index - width,
                Constants.Direction.Left => index - 1,
                Constants.Direction.Right => index + 1
            };
        }

        public static bool IsValidMove(NativeArray<Constants.CellType> cellArray, int width, int height, int index, Constants.Direction direction)
        {
            int directionIndex = GetIndex(width, height, index, direction);
            return direction switch
            {
                Constants.Direction.Up => directionIndex < width * height && cellArray[directionIndex] == Constants.CellType.Empty,
                Constants.Direction.Down => directionIndex >= 0 && cellArray[directionIndex] == Constants.CellType.Empty,
                Constants.Direction.Left => index % width != 0 && cellArray[directionIndex] == Constants.CellType.Empty,
                Constants.Direction.Right => (index + 1) % width != 0 && cellArray[directionIndex] == Constants.CellType.Empty,
                _ => false
            };
        }

        public static NativeList<Constants.Direction> PossibleMove(NativeArray<Constants.CellType> cellArray, int width, int height, int index)
        {
            NativeList<Constants.Direction> moves = new NativeList<Constants.Direction>(Allocator.Persistent);
            if (IsValidMove(cellArray, width, height, index, Constants.Direction.Up)) moves.Add(Constants.Direction.Up);
            if (IsValidMove(cellArray, width, height, index, Constants.Direction.Down)) moves.Add(Constants.Direction.Down);
            if (IsValidMove(cellArray, width, height, index, Constants.Direction.Left)) moves.Add(Constants.Direction.Left);
            if (IsValidMove(cellArray, width, height, index, Constants.Direction.Right)) moves.Add(Constants.Direction.Right);
            return moves;
        }
    }
}