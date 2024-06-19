using Swift.Math;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AnticGameTest
{
    public class PlayerOpIndicator : MonoBehaviour
    {
        public GameBall TargetBall;

        public event Action<Vec2> OnShot = null;

        Vector3 offset;

        public void OnDrag()
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = worldPos - TargetBall.transform.position;
            offset.z = 0;
        }

        public void OnEndDrag()
        {
            if (offset.magnitude >= 1)
            {
                OnShot?.Invoke(new Vec2(-offset.x, -offset.y));
            }

            offset = Vector3.zero;
        }

        private void Update()
        {
            transform.position = TargetBall.transform.position + offset;
            var arc = -MathEx.Arc(new(-offset.x, offset.y));
            transform.transform.localEulerAngles = new Vector3(0, 0, (float)MathEx.Arc2Dir(arc));
        }
    }
}
