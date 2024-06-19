using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnticGameTest
{
    public class PlayerRoot : MonoBehaviour
    {
        public PlayerOpIndicator[] IndicatorModels;

        readonly Dictionary<string, PlayerOpIndicator> indicators = new();

        public PlayerOpIndicator AddPlayerOpIndicator(string id, int color, GameBall targetBall)
        {
            var indicator = Instantiate(IndicatorModels[color]);
            indicator.TargetBall = targetBall;
            indicators[id] = indicator;

            indicator.transform.SetParent(transform);
            indicator.gameObject.SetActive(true);

            return indicator;
        }
    }
}
