#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

using System;
using System.Runtime.CompilerServices;

namespace BIG
{
    public static class MathExtension
    {
        public const float DEG_2_RAD = 0.0174532924f;
        public const float RAD_2_DEG = 57.29578f;

        #region Float

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(this in float x, float y, float s) => x + s * (y - x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this in float value, float min = 0, float max = 1) => Math.Clamp(value, min, max);

        // John Carmack optimization used for fast SQRT inversion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe float FastInvSqrt(this float x)
        {
            float xhalf = 0.5f * x;
            int i = *(int*)&x;  // treat float as int
            i = 0x5f3759df - (i >> 1);  // initial guess
            x = *(float*)&i;  // treat int as float
            x = x * (1.5f - xhalf * x * x);  // Newton-Raphson iteration
            return x;
        }

        #endregion

        #region Vector2

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this in Vector2 a, in Vector2 b)
        {
            float diffX = a.X - b.X;
            float diffY = a.Y - b.Y;
            return MathF.Sqrt(diffX * diffX + diffY * diffY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(this Vector2 a, Vector2 b, float s)
        {
            a.X += s * (b.X - a.X);
            a.Y += s * (b.Y - a.Y);
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Clamp(this Vector2 value, float minValue, float maxValue)
        {
            if (value.X < minValue)
                value.X = minValue;
            else if (value.X > maxValue)
                value.X = maxValue;

            if (value.Y < minValue)
                value.Y = minValue;
            else if (value.Y > maxValue)
                value.Y = maxValue;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this in Vector2 a, in Vector2 b) => a.X * b.X + a.Y * b.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(this in Vector2 v) => MathF.Sqrt(v.X * v.X + v.Y * v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FastMagnitude(this in Vector2 v)
        {
            float x2 = v.X * v.X;
            float y2 = v.Y * v.Y;
            float lengthSquared = x2 + y2;

            // Use the fast inverse square root
            float invLength = lengthSquared.FastInvSqrt();

            // Multiply by the original length to get the magnitude
            return invLength * lengthSquared;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RandomPositionInRadius(this in Vector2 vector, float radius)
        {
            var angle = CollectionsExtension.Random.RandomAngle();
            var distance = Math.Sqrt(CollectionsExtension.Random.NextDouble()) * radius;
            var x = vector.X + distance * Math.Cos(angle);
            var y = vector.Y + distance * Math.Sin(angle);
            return new Vector2(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(this in Vector2 v) => MathF.Atan2(v.Y, v.X);
        #endregion

        #region Vector3
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RandomPositionInRadius(this in Vector3 vector, float radius)
        {
            var angle = CollectionsExtension.Random.RandomAngle();
            var distance = Math.Sqrt(CollectionsExtension.Random.NextDouble()) * radius;
            var x = vector.X + (distance * Math.Cos(angle));
            var z = vector.Z + (distance * Math.Sin(angle));
            return new Vector3((float)x, vector.Y, (float)z);
        }
        #endregion

        #region Rotation2D

        /// <summary>
        /// Calculate delta angle between two angles.
        /// Those angles represent 2D object rotation in only one dimension so those are single float values.
        /// </summary>
        /// <param name="currentAngle">Rotation of the first object.</param>
        /// <param name="targetAngle">Rotation of the second object.</param>
        /// <returns>Delta between those two angles that stays in between -180 and 180 degree </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngle(this in float currentAngle, in float targetAngle)
        {
            float delta = targetAngle - currentAngle;
            delta %= 360;

            if (delta > 180)
            {
                delta -= 360;
            }
            else if (delta < -180)
            {
                delta += 360;
            }

            return delta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateTowards(this ref float rotation, Vector2 direction, float step)
        {
            float desiredAngle = -MathF.Atan2(direction.X, direction.Y) * RAD_2_DEG;
            float deltaAngle = DeltaAngle(rotation, desiredAngle);

            if (deltaAngle > 180)
            {
                deltaAngle -= 360;
            }
            else if (deltaAngle < -180)
            {
                deltaAngle += 360;
            }

            // Increment or decrement the rotation angle based on the deltaAngle
            float rotationChange = step;

            if (MathF.Abs(deltaAngle) > rotationChange)
            {
                rotation += (deltaAngle > 0) ? rotationChange : -rotationChange;
            }
            else
            {
                rotation = desiredAngle;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveForward(this ref Vector2 position, in float rotation, float step)
        {
            float angleRadians = rotation * DEG_2_RAD;
            Vector2 forward = new Vector2(-MathF.Sin(angleRadians), MathF.Cos(angleRadians));
            position += forward * step;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveTowards(this ref Vector2 position, Vector2 direction)
        {
            position += direction;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this ref float currentAngle, in float step)
        {
            var result = currentAngle + step;
            if (result > 360)
            {
                currentAngle = result % 360;
                return;
            }

            if (result < -360)
            {
                currentAngle = result % -360;
                return;
            }

            if (result == 360 || result == -360)
            {
                currentAngle = 0;
                return;
            }

            currentAngle = result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpAngle(this in float currentAngle, float targetAngle, float t)
        {
            float delta = ((targetAngle - currentAngle) % 360 + 540) % 360 - 180;
            return currentAngle + delta * t;
        }
        #endregion
    }
}