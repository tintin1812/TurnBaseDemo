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

        public static int GenHpLost(AxieHolder attacker, AxieHolder target)
        {
            return GenHpLost(attacker.AttackerNumber, target.AttackerNumber);
        }

        private static int GenHpLost(int attackerNumber, int targetNumber)
        {
            var seek = (3 + attackerNumber - targetNumber) % 3;
            return seek switch
            {
                0 => 4,
                1 => 5,
                _ => 3
            };
        }
    }
}