using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class Ball : IAABB<Ball>
    {
        public Vec2 Pos
        {
            get => pos;
            set
            {
                pos = value;
                UpdateAABB();
            }
        } Vec2 pos;

        public int Color { get; private set; }

        public Fix64 Size {
            get => size;
            set
            {
                size = value;
                UpdateAABB();
            }
        } Fix64 size;

        public AABB<Ball> AABB { get; private set; } // the default AABB is based on the ball's MaxSize

        public Ball(Vec2 pos, Fix64 size, int color)
        {
            AABB = new(this, 0, 0, 0, 0);
            Size = size;
            Pos = pos;
            Color = color;
        }

        public void UpdateAABB()
        {
            AABB.MinX = Pos.x - Size;
            AABB.MinY = Pos.y - Size;
            AABB.MaxX = Pos.x + Size;
            AABB.MaxY = Pos.y + Size;
        }
    }
}
