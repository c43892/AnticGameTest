using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class AABBInfo
    {
        public Fix64 MinX { get; set; }
        public Fix64 MinY { get; set; }
        public Fix64 MaxX { get; set; }
        public Fix64 MaxY { get; set; }

        public Vec2 Centre { get => new((MaxX - MinX) / 2 + MinX, (MaxY - MinY) / 2 + MinY); }
        public Vec2 Size { get => new(MaxX - MinX, MaxY - MinY); }
        public Vec2 HalfSize { get => Size / 2; }

        public AABBInfo(Fix64 minX, Fix64 minY, Fix64 maxX, Fix64 maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }

    public interface IAABB<T>
    {
        AABB<T> AABB { get; }
    }

    public class AABB<T> : AABBInfo
    {
        public T Id { get; set; }

        public AABB(T id, Fix64 minX, Fix64 minY, Fix64 maxX, Fix64 maxY)
            : base(minX, minY, maxX, maxY)
        {
            Id = id;
        }
    }
}
