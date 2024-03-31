using System;
using System.Collections.Generic;
using FairyGUI;
using Utility;
using Gui;
using Plugins.PathFinding;

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
            }

            foreach (var p in pos.ends)
            {
                var axieAni = AxieAni.Create(charCom, homeScreen.GetPos(p.y, p.x), gameResource.MatchResource.Defender);
                _axieAniAll[mapData.GetTileIndex(p.y, p.x)] = axieAni;
            }

            // ReProcessMove
            _moveNext = MapDataUtil.FindPathingAttacker(mapData);

            _revertAble = new Stack<MetaRevert>();
        }

        private GTweener AxieAniMove(Tile start, Tile next)
        {
            _axieAniAll.Remove(_mapData.GetTileIndex(start.Row, start.Col), out var ani);
            _axieAniAll[_mapData.GetTileIndex(next.Row, next.Col)] = ani;
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
            GTweener lastTweener = null;
            var doRevert = new List<Func<GTweener>>();
            foreach (var move in _moveNext)
            {
                if (move.Next == move.Start) continue;
                if (move.Next == null) continue;
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
            doRevert.Add(() =>
            {
                _moveNext = MapDataUtil.FindPathingAttacker(_mapData);
                return null;
            });
            _revertAble.Push(new MetaRevert(doRevert));
            _moveNext = MapDataUtil.FindPathingAttacker(_mapData);
            lastTweener?.OnComplete(onComplete);
        }

        private class MetaRevert
        {
            private readonly List<Func<GTweener>> _wait;

            public MetaRevert(List<Func<GTweener>> wait)
            {
                _wait = wait;
            }

            public void Do(GTweenCallback onComplete)
            {
                GTweener lastTweener = null;
                foreach (var d in _wait)
                {
                    var t = d();
                    if (t != null)
                    {
                        lastTweener = t;
                    }
                }

                lastTweener?.OnComplete(onComplete);
            }
        }
    }
}