using Swift.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    // component is a basic concept in my system structure. it refers to a module with specific functions. components will be holded ba a component container.
    // a component is usually initialized at game start, not created/destroyed dynamically as the game running.
    public abstract class Component
    {
        public abstract string Name { get; }

        public virtual int Priority { get; } = 0;

        public ComponentContainer Container = null;
    }

    // a component contains only one instance of each component type at most
    public class ComponentContainer
    {
        private readonly SortedDictionary<int, List<Component>> components = new();

        public T Get<T>() where T : class
        {
            foreach (var pq in components.Keys)
            {
                foreach(var com in components[pq])
                {
                    if (com is T)
                    {
                        return com as T;
                    }
                }
            }

            return null;
        }

        public void Add<T>(T com) where T : Component
        {
            if (Get<T>() != null)
                throw new Exception($"a component of {typeof(T)} already exsits");

            List<Component> pq;
            if (components.ContainsKey(com.Priority))
            {
                pq = components[com.Priority];
            }
            else
            {
                pq = new();
                com.Container = this;
                components.Add(com.Priority, pq);
            }

            pq.Add(com);
        }

        public bool Del(Component com)
        {
            foreach (var pq in components.Keys)
            {
                if (components[pq].Contains(com))
                {
                    components[pq].Remove(com);
                    com.Container = null;
                    return true;
                }
            }

            return false;
        }

        public Component Del<T>() where T : Component
        {
            foreach (var pq in components.Keys)
            {
                foreach (var com in components[pq])
                {
                    if (com is T)
                    {
                        components[pq].Remove(com);
                        com.Container = null;

                        if (components.Count == 0)
                        {
                            components.Remove(pq);
                        }

                        return com;
                    }
                }
            }

            return null;
        }

        // run all components in order of priority
        public void Foreach(Action<Component> travel)
        {
            foreach (var pq in components.Values)
            {
                foreach (var com in pq)
                {
                    travel(com);
                }
            }
        }

        public void Foreach<T>(Action<T> travel) where T : class
        {
            foreach (var pq in components.Values)
            {
                foreach (var com in pq)
                {
                    if (com is T)
                        travel(com as T);
                }
            }
        }
    }

    public interface IFrameDriven
    {
        void OnElapsed(Fix64 te);
    }

    public interface IClearable
    {
        void Clear();
    }
}
