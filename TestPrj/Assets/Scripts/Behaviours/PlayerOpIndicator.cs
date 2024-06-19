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
        public SphereCollider Collider;

        public GameBall TargetBall { get; set; }

        public event Action<Vec2> OnShot = null;

        bool dragging = false;
        Vector3 offset;

        public bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                Collider.enabled = value;
            }
        } bool activated;

        public void OnDrag()
        {
            if (!Activated)
                return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = worldPos - TargetBall.transform.position;
            offset.z = 0;
            dragging = true;
        }

        public void OnEndDrag()
        {
            if (!Activated)
                return;

            if (offset.magnitude >= 1)
            {
                OnShot?.Invoke(new Vec2(-offset.x, -offset.y));
            }

            offset = Vector3.zero;
            dragging = false;
        }

        // sometimes the cursor will go out of the scene then OnEndDrag won't be triggered
        private void OnMouseUp()
        {
            if (!Activated || !dragging)
                return;

            OnEndDrag();
        }

        private void Update()
        {
            transform.position = TargetBall.transform.position + offset;

            if (!Activated)
                return;

            if (dragging)
            {
                var arc = -MathEx.Arc(new(-offset.x, offset.y));
                transform.transform.localEulerAngles = new Vector3(0, 0, (float)MathEx.Arc2Dir(arc));
            }
            else
            {
                transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * 500);
            }
        }
    }
}
