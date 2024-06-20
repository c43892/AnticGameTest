using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace AnticGameTest
{
    // a quadtree is to store all the balls in a 2D space for quick inspection
    public class QuadTree<T>
    {
        public const int MAX_LEVEL = 5;
        public const int OBJ_NUM_SPLIT = 5;

        public int Level { get; private set; }

        public QuadTree<T> Parent { get; private set; }

        public QuadTree<T>[] Children { get; private set; }

        public AABB<QuadTree<T>> AABB { get; private set; }

        public IEnumerable<IAABB<T>> AllObjs { get => objs; }

        readonly List<IAABB<T>> objs = new();

        public QuadTree(Fix64 minX, Fix64 minY, Fix64 maxX, Fix64 maxY, QuadTree<T> parent = null)
        {
            AABB = new(this, minX, minY, maxX, maxY);
            Parent = parent;
            Level = parent == null ? 0 : parent.Level + 1;
        }

        void AddInternal(IAABB<T> obj)
        {
            if (objs.Contains(obj))
                throw new Exception("the given object is already in the tree");

            objs.Add(obj);
        }

        public void Add(IAABB<T> obj)
        {
            AddInternal(obj);
            TrySplitting();
        }

        public void Add(IEnumerable<IAABB<T>> objs)
        {
            foreach (var obj in objs)
                AddInternal(obj);

            TrySplitting();
        }

        public void Remove(IAABB<T> obj)
        {
            if (objs.Contains(obj))
            {
                objs.Remove(obj);
                TryMergingUp();
            }
            else if (Children != null)
            {
                foreach (var child in Children)
                    child.Remove(obj);
            }
        }

        public bool InBound(Vec2 pt) => pt.x > AABB.MinX && pt.y > AABB.MinY && pt.x < AABB.MaxX && pt.y < AABB.MaxY;

        public bool InBound(AABBInfo aabb) => AABB.MinX <= aabb.MinX && AABB.MaxX >= aabb.MaxX && AABB.MinY <= aabb.MinY && AABB.MaxY >= aabb.MaxY;

        public void Clear()
        {
            objs.Clear();

            if (Children != null)
            {
                foreach (var child in Children)
                    child.Clear();
            }

            Children = null;
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
                    targetObjs.Add(obj);
            }

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    if (CheckCollision(child.AABB, aabb))
                        targetObjs.AddRange(child.CheckCollision(aabb));
                }
            }

            return targetObjs;
        }

        public int Count => objs.Count;

        void TrySplitting()
        {
            if (objs.Count <= OBJ_NUM_SPLIT || Level >= MAX_LEVEL)
                return;

            if (Children == null)
            {
                var l = AABB.MinX;
                var cx = l + AABB.HalfSize.x;
                var r = AABB.MaxX;
                var t = AABB.MinY;
                var cy = t + AABB.HalfSize.y;
                var b = AABB.MaxY;

                Children = new QuadTree<T>[]
                {
                    new(l, t, cx, cy, this),
                    new(cx, t, r, cy, this),
                    new(l, cy, cx, b, this),
                    new(cx, cy, r, b, this),
                };
            }

            var i = 0;
            while (i < objs.Count)
            {
                var obj = objs[i];
                var putDown = false;
                foreach (var child in Children)
                {
                    if (child.InBound(obj.AABB))
                    {
                        objs.Remove(obj);
                        child.AddInternal(obj);
                        putDown = true;
                        break;
                    }
                }

                if (!putDown)
                    i++;
            }

            foreach (var child in Children)
                child.TrySplitting();
        }

        void MergeChildren()
        {
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    child.MergeChildren();
                    objs.AddRange(child.AllObjs);
                }
            }

            Children = null;
        }

        void TryMergingUp()
        {
            var objsCount = objs.Count;

            if (Children != null)
                objsCount += Children.Sum(child => child.objs.Count) + objs.Count;

            if (objsCount <= OBJ_NUM_SPLIT)
            {
                MergeChildren();
                Parent?.TryMergingUp();
            }
        }

        public void Update()
        {
            MergeChildren();
            TrySplitting();
        }
    }
}
