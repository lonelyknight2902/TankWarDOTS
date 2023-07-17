using Unity.Mathematics;

namespace Utilities
{
    public static class Functions
    {
        public static float3 GetCellPosition(int width, int height, int index)
        {
            return new float3(index % width, 0, (int) index / height);
        }
    }
}