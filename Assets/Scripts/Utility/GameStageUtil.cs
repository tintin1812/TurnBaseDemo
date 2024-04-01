using System.Collections.Generic;
using Gui;
using UnityEngine;

namespace Utility
{
    public static class GameStageUtil
    {
        public static List<AxieAni> FindAttackAble(this AxieAni target, Dictionary<int, AxieAni> axieAniAll)
        {
            var result = new List<AxieAni>();
            foreach (var axieCheck in axieAniAll.Values)
            {
                if (!axieCheck.IsAlive) continue;
                if (target == axieCheck) continue;
                if (target.Team == axieCheck.Team) continue;
                var v = target.TilePos - axieCheck.TilePos;
                var d = Mathf.Abs(v.x) + Mathf.Abs(v.y);
                if (d == 1)
                {
                    result.Add(axieCheck);
                }
            }

            return result;
        }
    }
}