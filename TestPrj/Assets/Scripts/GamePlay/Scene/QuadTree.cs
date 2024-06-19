using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AnticGameTest
{
    // a quadtree is to store all the balls in a 2D space for quick inspection
    public class QuadTree<T>
    {
        // todo: will implement the real quad tree later, now just fill in the interfaces to make it functioning.

        public AABB<QuadTree<T>> WorldBound { get; private set; }

        public IEnumerable<IAABB<T>> AllObjs { get => objs; }

        private readonly List<IAABB<T>> objs = new();

        public QuadTree(Fix64 minX, Fix64 minY, Fix64 maxX, Fix64 maxY)
        {
            WorldBound = new(this, minX, minY, maxX, maxY);
        }

        public void Add(IAABB<T> obj)
        {
            if (objs.Contains(obj))
                throw new Exception("the given object is already in the tree");

            objs.Add(obj);
        }

        public void Add(IEnumerable<IAABB<T>> objs)
        {
            foreach (var obj in objs)
                Add(obj);
        }

        public void Remove(IAABB<T> obj)
        {
            if (!objs.Contains(obj))
                throw new Exception("the given object is not in the tree");

            objs.Remove(obj);
        }

        public bool InBound(Vec2 pt) => pt.x > WorldBound.MinX && pt.y > WorldBound.MinY && pt.x < WorldBound.MaxX && pt.y < WorldBound.MaxY;

        public void Clear()
        {
            objs.Clear();
        }

        public bool CheckCollision(AABBInfo a, AABBInfo b)
        {
            var ca = a.Centre;
            var cb = b.Centre;
            var halfSizeA = a.HalfSize;
            var halfSizeB = b.HalfSize;
            var dx = Fix64.Abs(cb.x - ca.x);
            var dy = Fix64.Abs(cb.y - ca.y);

            return dx < (halfSizeA.x + halfSizeB.x) && dy < (halfSizeA.y + halfSizeB.y);
        }

        public IEnumerable<IAABB<T>> CheckCollision(AABBInfo aabb)
        {
            List<IAABB<T>> targetObjs = new ();

            foreach (var obj in objs)
            {
                if (obj.AABB != aabb && CheckCollision(obj.AABB, aabb))
                {
                    targetObjs.Add(obj);
                }
            }

            return targetObjs;
        }

        public int Count => objs.Count;

        public void Update()
        {
            // since there is no tree yet, no updating is needed at the moment.
        }
    }
}
