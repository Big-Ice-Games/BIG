using System;

namespace BIG
{
    public enum Direction
    {
        North,
        East,
        South,
        West,
    }

    public static class DirectionExtensions
    {
        public static IntVector2 ToVector(this Direction direction)
        {
            return direction switch
            {
                Direction.North => new IntVector2(0, 1),
                Direction.East => new IntVector2(1, 0),
                Direction.South => new IntVector2(0, -1),
                Direction.West => new IntVector2(-1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static Direction Opposite(this Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static Direction FromVector(this in IntVector2 vector)
        {
            if (vector.X == 0 && vector.Y == 1) return Direction.North;
            if (vector.X == 1 && vector.Y == 0) return Direction.East;
            if (vector.X == 0 && vector.Y == -1) return Direction.South;
            if (vector.X == -1 && vector.Y == 0) return Direction.West;
            throw new ArgumentException("Invalid vector for direction conversion.");
        }

        public static bool IsDiagonal(this in IntVector2 a, IntVector2 b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);

            return dx == 1 && dy == 1;
        }

        public static Direction GetDirection(this IntVector2 from, IntVector2 to)
        {
            var delta = to - from;
            int absX = Math.Abs(delta.X);
            int absY = Math.Abs(delta.Y);

            if (absX >= absY)
                return delta.X >= 0 ? Direction.East : Direction.West;
            else
                return delta.Y >= 0 ? Direction.North : Direction.South;
        }
    }
}
