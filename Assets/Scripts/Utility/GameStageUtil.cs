using System.Collections.Generic;
using Gui;
using UnityEngine;

namespace Utility
{
    public static class GameStageUtil
    {
        public static List<AxieHolder> FindAttackAble(this AxieHolder attacker, List<AxieHolder> defenders)
        {
            var result = new List<AxieHolder>();
            foreach (var axieCheck in defenders)
            {
                if (!axieCheck.IsAlive) continue;
                if (attacker == axieCheck) continue;
                if (attacker.Team == axieCheck.Team) continue;
                var v = attacker.TilePos - axieCheck.TilePos;
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