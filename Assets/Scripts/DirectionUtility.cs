using UnityEngine;

public enum Direction { UP, DOWN, LEFT, RIGHT, COUNT }

public static class DirectionUtility
{
    private static Vector3[] _directionalMovements =
    {
        Vector3.forward, -Vector3.forward, Vector3.left, Vector3.right
    };

    public static Vector3 GetDirectionalMovement(Direction direction)
        => direction == Direction.COUNT ? Vector3.zero : _directionalMovements[(int)direction];
}