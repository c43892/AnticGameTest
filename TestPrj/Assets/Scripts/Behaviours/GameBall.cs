using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class GameBall : MonoBehaviour
    {
        public Ball Ball;

        private void Update()
        {
            transform.localScale = Vector3.one * (float)Ball.Size * 2;
            transform.localPosition = new((float)Ball.Pos.x, (float)Ball.Pos.y, 0);
        }

        void OnDrawGizmos()
        {
            // Set the Gizmos color
            Gizmos.color = UnityEngine.Color.green;

            // Draw a sphere at the transform's position
            Gizmos.DrawLineList(new Vector3[]
            {
                new((float)Ball.AABB.MinX, (float)Ball.AABB.MinY, 0),
                new((float)Ball.AABB.MaxX, (float)Ball.AABB.MinY, 0),

                new((float)Ball.AABB.MaxX, (float)Ball.AABB.MinY, 0),
                new((float)Ball.AABB.MaxX, (float)Ball.AABB.MaxY, 0),

                new((float)Ball.AABB.MaxX, (float)Ball.AABB.MaxY, 0),
                new((float)Ball.AABB.MinX, (float)Ball.AABB.MaxY, 0),

                new((float)Ball.AABB.MinX, (float)Ball.AABB.MaxY, 0),
                new((float)Ball.AABB.MinX, (float)Ball.AABB.MinY, 0),
            });
        }
    }
}
