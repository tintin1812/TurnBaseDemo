using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FairyGUI;
using Utility;
using Gui;
using UnityEngine;

namespace Data
{
    public class GameStage
    {
        private BattleStage _battleStage;
        private List<MapDataUtil.MoveToPos> _preMove;
        private MapData _mapData;
        private Stack<MetaRevert> _revertAble;

        public void Init(MapData mapData, IHomeScreenExtension homeScreen, GameResource gameResource)
        {
            _mapData = mapData;
            _battleStage = new BattleStage();
            _battleStage.Init(mapData, homeScreen, gameResource);

            // ReProcessMove
            _preMove = _battleStage.FindPathingAttacker();
            _revertAble = new Stack<MetaRevert>();
        }

        public bool CanNextStep
        {
            get
            {
                if (_battleStage.Attackers.Count <= 0) return false;
                if (_battleStage.Defenders.Count <= 0) return false;
                if (_preMove == null) return false;
                return _preMove.Count > 0;
            }
        }

        public bool CanRevertAble => _revertAble.Count > 0;

        public void DoPreviewStep(GTweenCallback onComplete)
        {
            if (_revertAble.Count <= 0)
            {
                return;
            }

            _battleStage.HideAllAxieIndicator();
            _revertAble.Pop().Do(onComplete);
        }

        public void DoNextStep(GTweenCallback onComplete)
        {
            TaskUtil.CallAwait(async () =>
            {
                _battleStage.HideAllAxieIndicator();
                var doRevert = new List<Func<GTweener>>();
                // Move
                await DoMoveStep(doRevert);

                // Attack
                var doRevertAtt = new List<GTweenCallback>();
                await DoAttack(doRevertAtt);

                if (doRevert.Count <= 0 && doRevertAtt.Count <= 0)
                {
                    onComplete();
                    return;
                }

                if (doRevertAtt.Count > 0)
                {
                    doRevert.Insert(0, () =>
                    {
                        foreach (var call in doRevertAtt)
                        {
                            call();
                        }

                        return null;
                    });
                }

                // CheckDie
                var doRevertDie = new List<GTweenCallback>();
                await CheckDie(doRevertDie);
                if (doRevertDie.Count > 0)
                {
                    doRevert.Insert(0, () =>
                    {
                        foreach (var call in doRevertDie)
                        {
                            call();
                        }

                        return null;
                    });
                }

                await TaskUtil.Delay(0.1f);
                // Update FindPathing
                _preMove = _battleStage.FindPathingAttacker();
                doRevert.Add(() =>
                {
                    _preMove = _battleStage.FindPathingAttacker();
                    return null;
                });

                if (doRevert.Count > 0)
                {
                    _revertAble.Push(new MetaRevert(doRevert));
                }

                onComplete();
            });
        }

        private async Task DoMoveStep(List<Func<GTweener>> doRevert)
        {
            foreach (var axie in _battleStage.AxieAll.Values)
            {
                axie.Status = AxieHolder.AxieStatus.MarkFind;
            }

            GTweener lastTweener = null;

            foreach (var move in _preMove)
            {
                if (move.Next == null) continue;
                if (move.Next == move.Start) continue;
                var mapData = _mapData;
                mapData.DoMove(move.Start, move.Next);
                lastTweener = _battleStage.AxieAniMove(move.Start, move.Next, false);
                doRevert.Add(() =>
                {
                    mapData.DoMove(move.Next, move.Start);
                    return _battleStage.AxieAniMove(move.Next, move.Start, true);
                });
            }

            doRevert.Reverse();

            if (lastTweener != null)
            {
                var task = new TaskCompletionSource<object>();
                lastTweener.OnComplete(() => { task.SetResult(true); });
                await task.Task;
            }
        }

        private async Task DoAttack(List<GTweenCallback> doRevert)
        {
            var attackers = _battleStage.Attackers.OrderBy(it => it.Hp).ToList();
            var defenders = _battleStage.Defenders.OrderBy(it => it.Hp).ToList();
            foreach (var attacker in attackers)
            {
                if (attacker.Team != AxieHolder.AxieTeam.Attack) continue;
                if (attacker.Status != AxieHolder.AxieStatus.MarkFind) continue;
                var attackAble = attacker.FindAttackAble(defenders);
                if (attackAble.Count == 0)
                {
                    attacker.Status = AxieHolder.AxieStatus.MarkIdle;
                    continue;
                }

                attacker.Status = AxieHolder.AxieStatus.MarkAttack;

                foreach (var beAttack in attackAble)
                {
                    if (!beAttack.IsAlive) continue;
                    if (!attacker.IsAlive) continue;
                    attacker.FaceToPos(beAttack.TilePos.x);
                    beAttack.FaceToPos(attacker.TilePos.x);
                    var timeA = attacker.DoAniAttack();
                    var timeB = beAttack.DoAniAttack();
                    await TaskUtil.Delay(Mathf.Max(timeA, timeB));
                    var hpLastAxie = attacker.Hp;
                    var hpLastBeAtt = beAttack.Hp;

                    var damageAtt = GameStageUtil.GenHpLost(attacker, beAttack);
                    var damageDefend = GameStageUtil.GenHpLost(beAttack, attacker);
                    Util.ShowNotiText($"Attacker deal {damageAtt} dmg, Defend deal {damageAtt} dmg");
                    beAttack.BeAttack(damageAtt);
                    attacker.BeAttack(damageDefend);
                    var attack = beAttack;
                    doRevert.Add(() =>
                    {
                        attacker.DoRevertHp(hpLastAxie);
                        attack.DoRevertHp(hpLastBeAtt);
                    });
                }

                doRevert.Reverse();
            }
        }

        private async Task CheckDie(List<GTweenCallback> doRevert)
        {
            var axieDie = new List<AxieHolder>();
            var all = _battleStage.AxieAll.Values;
            foreach (var axie in all)
            {
                if (axie.IsAlive) continue;
                axieDie.Add(axie);
            }

            if (axieDie.Count <= 0) return;
            var timeAni = 0.0f;
            foreach (var _ in axieDie)
            {
                var axie = _;
                timeAni = Mathf.Max(axie.DoAniDie(), timeAni);
                _battleStage.RemoveAxieAni(axie);
                var lastTile = _mapData.GetTile(axie.TilePos.y, axie.TilePos.x);
                _mapData.RemoveTile(axie.TilePos.y, axie.TilePos.x);
                doRevert.Add(() =>
                {
                    _battleStage.ReviverAxiAni(axie);
                    _mapData.SetTile(axie.TilePos.y, axie.TilePos.x, lastTile);
                });
            }

            await TaskUtil.Delay(timeAni);
            foreach (var axie in axieDie)
            {
                axie.AxieCom.visible = false;
                doRevert.Add(() => { axie.AxieCom.visible = true; });
            }

            doRevert.Reverse();
        }
    }
}