using System;
using System.Collections.Generic;
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
            return ani.AxieCom.TweenMove(_homeScreen.GetPos(next.Row, next.Col), 0.5f);
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
            foreach (var axie in _axieAniAll.Values)
            {
                axie.Status = AxieAni.AxieStatus.MarkFind;
            }

            GTweener lastTweener = null;
            var doRevert = new List<Func<GTweener>>();
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

            foreach (var axie in _axieAniAll.Values)
            {
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
                    beAttack.BeAttack(5);
                }
            }

            doRevert.Reverse();
            doRevert.Add(() =>
            {
                _moveNext = MapDataUtil.FindPathingAttacker(_mapData);
                return null;
            });
            _revertAble.Push(new MetaRevert(doRevert));
            _moveNext = MapDataUtil.FindPathingAttacker(_mapData);
            lastTweener?.OnComplete(onComplete);
        }
    }
}