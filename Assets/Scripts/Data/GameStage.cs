using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FairyGUI;
using Utility;
using Gui;
using Plugins.PathFinding;
using UnityEngine;

namespace Data
{
    public class GameStage
    {
        private Dictionary<int, AxieAni> _axieAniAll;
        private List<MapDataUtil.MoveToPos> _moveNext;
        private MapData _mapData;
        private IHomeScreenExtension _homeScreen;
        private Stack<MetaRevert> _revertAble;

        public void Init(MapData mapData, IHomeScreenExtension homeScreen, GameResource gameResource)
        {
            _mapData = mapData;
            _homeScreen = homeScreen;
            var charCom = homeScreen.MapContent.Character;
            var pos = mapData.GetPosStarEnd();
            _axieAniAll = new Dictionary<int, AxieAni>();
            foreach (var p in pos.starts)
            {
                var axieAni = AxieAni.Create(charCom, homeScreen.GetPos(p.y, p.x), gameResource.MatchResource.Attacker);
                _axieAniAll[mapData.GetTileIndex(p.y, p.x)] = axieAni;
                axieAni.FaceTo(true);
                axieAni.Init(AxieAni.AxieTeam.Attack, 32);
                axieAni.TilePos = p;
            }

            foreach (var p in pos.ends)
            {
                var axieAni = AxieAni.Create(charCom, homeScreen.GetPos(p.y, p.x), gameResource.MatchResource.Defender);
                _axieAniAll[mapData.GetTileIndex(p.y, p.x)] = axieAni;
                axieAni.TilePos = p;
                axieAni.Init(AxieAni.AxieTeam.Defend, 16);
            }

            // ReProcessMove
            _moveNext = MapDataUtil.FindPathingAttacker(mapData);

            _revertAble = new Stack<MetaRevert>();
        }

        private GTweener AxieAniMove(Tile start, Tile next)
        {
            // Update index tile cache
            _axieAniAll.Remove(_mapData.GetTileIndex(start.Row, start.Col), out var ani);
            _axieAniAll[_mapData.GetTileIndex(next.Row, next.Col)] = ani;
            ani.TilePos = next.Pos;
            ani.Status = AxieAni.AxieStatus.MarkMove;
            var timeMove = ani.DoAniMove();
            return ani.AxieCom.TweenMove(_homeScreen.GetPos(next.Row, next.Col), timeMove);
        }

        private void RemoveAxieAni(AxieAni axie)
        {
            _axieAniAll.Remove(_mapData.GetTileIndex(axie.TilePos.y, axie.TilePos.x), out var ani);
            if (axie != ani)
            {
                Debug.LogWarning("RemoveAxieAni not Same Cache");
            }
        }

        private void ReviverAxiAni(AxieAni axie)
        {
            _axieAniAll[_mapData.GetTileIndex(axie.TilePos.y, axie.TilePos.x)] = axie;
        }

        public bool CanRevertAble => _revertAble.Count > 0;

        public void DoPreviewStep(GTweenCallback onComplete)
        {
            if (_revertAble.Count <= 0)
            {
                return;
            }

            _revertAble.Pop().Do(onComplete);
        }

        public void DoNextStep(GTweenCallback onComplete)
        {
            TaskUtil.CallAwait(async () =>
            {
                var doRevert = new List<Func<GTweener>>();
                // Move
                await DoMoveStep(doRevert);
                if (doRevert.Count > 0)
                {
                    await TaskUtil.Delay(0.5f);
                }

                // Attack
                var doRevertAtt = new List<GTweenCallback>();
                await DoAttack(doRevertAtt);
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
                    await TaskUtil.Delay(0.5f);
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
                    await TaskUtil.Delay(0.5f);
                }

                // Update FindPathing
                _moveNext = MapDataUtil.FindPathingAttacker(_mapData);
                doRevert.Add(() =>
                {
                    _moveNext = MapDataUtil.FindPathingAttacker(_mapData);
                    return null;
                });

                if (doRevert.Count > 0)
                {
                    _revertAble.Push(new MetaRevert(doRevert));
                }

                onComplete();
            });
        }

        private Task DoMoveStep(List<Func<GTweener>> doRevert)
        {
            foreach (var axie in _axieAniAll.Values)
            {
                axie.Status = AxieAni.AxieStatus.MarkFind;
            }

            GTweener lastTweener = null;

            foreach (var move in _moveNext)
            {
                if (move.Next == null) continue;
                if (move.Next == move.Start) continue;
                var mapData = _mapData;
                mapData.DoMove(move.Start, move.Next);
                lastTweener = AxieAniMove(move.Start, move.Next);
                doRevert.Add(() =>
                {
                    mapData.DoMove(move.Next, move.Start);
                    return AxieAniMove(move.Next, move.Start);
                });
            }

            doRevert.Reverse();

            if (lastTweener != null)
            {
                var task = new TaskCompletionSource<bool>();
                lastTweener.OnComplete(() => { task.SetResult(true); });
            }

            return Task.CompletedTask;
        }

        private async Task DoAttack(List<GTweenCallback> doRevert)
        {
            foreach (var axie in _axieAniAll.Values)
            {
                if (axie.Team != AxieAni.AxieTeam.Attack) continue;
                if (axie.Status != AxieAni.AxieStatus.MarkFind) continue;
                var attackAble = axie.FindAttackAble(_axieAniAll);
                if (attackAble.Count == 0)
                {
                    axie.Status = AxieAni.AxieStatus.MarkIdle;
                    continue;
                }

                axie.Status = AxieAni.AxieStatus.MarkAttack;

                foreach (var beAttack in attackAble)
                {
                    if (!beAttack.IsAlive) continue;
                    if (!axie.IsAlive) continue;
                    var timeA = axie.DoAniAttack();
                    var timeB = beAttack.DoAniAttack();
                    await TaskUtil.Delay(Mathf.Max(timeA, timeB));
                    var hpLastAxie = axie.Hp;
                    var hpLastBeAtt = beAttack.Hp;
                    beAttack.BeAttack(5);
                    axie.BeAttack(5);
                    doRevert.Add(() =>
                    {
                        axie.DoRevertHp(hpLastAxie);
                        beAttack.DoRevertHp(hpLastBeAtt);
                    });
                }

                doRevert.Reverse();
            }
        }

        private async Task CheckDie(List<GTweenCallback> doRevert)
        {
            var axieDie = new List<AxieAni>();
            var all = _axieAniAll.Values;
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
                RemoveAxieAni(axie);
                var lastTile = _mapData.GetTile(axie.TilePos.y, axie.TilePos.x);
                _mapData.RemoveTile(axie.TilePos.y, axie.TilePos.x);
                doRevert.Add(() =>
                {
                    ReviverAxiAni(axie);
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