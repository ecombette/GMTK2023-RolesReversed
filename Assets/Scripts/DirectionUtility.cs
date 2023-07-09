using UnityEngine;

public enum Direction { UP, DOWN, LEFT, RIGHT, NONE }

public static class DirectionUtility
{
    private static Vector3[] _directionalMovements =
    {
        Vector3.forward, -Vector3.forward, Vector3.left, Vector3.right
    };

    private static int _directionCount = 4;

    public static int DirectionCount => _directionCount;
    public static Vector3 GetDirectionalMovement(Direction direction)
        => direction == Direction.NONE ? Vector3.zero : _directionalMovements[(int)direction];
}