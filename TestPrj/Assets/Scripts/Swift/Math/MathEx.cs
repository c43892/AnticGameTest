using System;

namespace Swift.Math
{
    // some extension methods
    public static class MathEx
    {
        public static readonly Fix64 Pi = Fix64.Pi;
        public static readonly Fix64 HalfPi = Pi / 2;
        public static readonly Fix64 Pi2 = Pi * 2;

        public static readonly Fix64 Left = Fix64.Pi;
        public static readonly Fix64 Right = Fix64.Zero;
        public static readonly Fix64 Up = HalfPi;
        public static readonly Fix64 Down = HalfPi + Pi;

        public static readonly Fix64 Rad2Deg = 180 / Pi;
        public static readonly Fix64 Deg2Rad = Pi / 180;

        // The Euler angles (0-2pi)
        public static Fix64 Arc(this Vec2 v)
        {
            return MathEx.Atan(v.y, v.x);
        }

        public static int Dir(this Vec2 v)
        {
            return (int)(v.Arc() * 180 / Pi);
        }

        // Truncate to a given range
        public static float Clamp(this float v, float min, float max)
        {
            if (v < min)
                return min;
            else if (v > max)
                return max;
            else
                return v;
        }

        // Truncate to a given range
        public static Fix64 Clamp(this Fix64 v, Fix64 min, Fix64 max)
        {
            if (v < min)
                return min;
            else if (v > max)
                return max;
            else
                return v;
        }

        // Truncate to a given range
        public static Vec2 Clamp(this Vec2 v, Vec2 max) { return v.Clamp(Vec2.Zero, max); }
        public static Vec2 Clamp(this Vec2 v, Vec2 min, Vec2 max)
        {
            if (v.x >= min.x && v.y >= min.y && v.x <= max.x && v.y <= max.y)
                return v;

            return new Vec2(v.x.Clamp(min.x, max.x), v.y.Clamp(min.y, max.y));
        }

        // Calculates steering from the current orientation to the target orientation and limits the maximum angle of rotation
        public static Fix64 CalcArcTurn2(Vec2 nowDirVec, Vec2 turn2DirVec, Fix64 maxAbs)
        {
            var arcFrom = nowDirVec.Arc();
            var arcTo = turn2DirVec.Arc();
            return CalcArc4Turn2(arcFrom, arcTo, maxAbs);
        }

        // Calculates steering from the current orientation to the target orientation and limits the maximum angle of rotation
        public static Fix64 CalcDir4Turn2(Fix64 nowDir, Fix64 turn2Dir, Fix64 maxAbs)
        {
            return CalcArc4Turn2(nowDir.Dir2Arc(), turn2Dir.Dir2Arc(), maxAbs.Dir2Arc()).Arc2Dir();
        }

        // Calculates steering from the current orientation to the target orientation and limits the maximum angle of rotation
        public static Fix64 CalcArc4Turn2(Fix64 nowArc, Fix64 turn2Arc, Fix64 maxAbs)
        {
            var tv = (turn2Arc - nowArc).RangeInPi();
            if (Fix64.Abs(maxAbs) < Fix64.Abs(tv))
                return tv > 0 ? maxAbs : -maxAbs;
            else
                return tv;
        }

        // Normalize the specified angle to [-180, 180)
        public static Fix64 RangeIn180(this Fix64 dir)
        {
            var d = dir;
            while (d >= 180)
                d -= 360;

            while (d < -180)
                d += 360;

            return d;
        }

        // Normalize the specified angle to [-180, 180)
        public static Fix64 RangeInPi(this Fix64 arc)
        {
            var d = arc;
            while (d >= Pi)
                d -= Pi2;

            while (d < -Pi)
                d += Pi2;

            return d;
        }

        // take an absolute value
        public static Fix64 Abs(this Fix64 v)
        {
            return v >= 0 ? v : -v;
        }

        // trigonometric functions

        public static Fix64 Cos(Fix64 arc)
        {
            return Fix64.Cos(arc);
        }

        public static Fix64 Sin(Fix64 arc)
        {
            return Fix64.Sin(arc);
        }

        public static Fix64 Tan(Fix64 arc)
        {
            return Fix64.Tan(arc);
        }

        public static Fix64 Atan(Fix64 y, Fix64 x)
        {
            return Fix64.Atan2(y, x);
        }

        public static Fix64 Sqrt(Fix64 v)
        {
            return Fix64.Sqrt(v);
        }

        public static Fix64 Max(Fix64 a, Fix64 b)
        {
            return a >= b ? a : b;
        }

        public static Fix64 Min(Fix64 a , Fix64 b)
        {
            return a <= b ? a : b;
        }

        public static Fix64 Arc2Dir(this Fix64 arc)
        {
            return arc * 180 / Pi;
        }

        public static Fix64 Dir2Arc(this Fix64 dir)
        {
            return dir * Pi / 180;
        }
    }
}
