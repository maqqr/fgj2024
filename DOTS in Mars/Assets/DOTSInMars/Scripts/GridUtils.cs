using Unity.Mathematics;

public static class GridUtils
{
    public static int3 ToGridPosition(in float3 position)
    {
        return new int3
        {
            x = (int)math.floor(position.x),
            y = (int)math.floor(position.y),
            z = (int)math.floor(position.z),
        };
    }

    public static float3 FromGridPosition(in int3 gridPosition)
    {
        return new float3
        {
            x = gridPosition.x,
            y = gridPosition.y,
            z = gridPosition.z,
        };
    }
}
