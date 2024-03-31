using System;
using System.Collections.Generic;
using Utility;
using Gui;

namespace Data
{
    public class GameStage
    {
        private Dictionary<int, AxieAni> _axieAniAttack;
        private Dictionary<int, AxieAni> _axieAniDefend;
        private List<MapDataUtil.MoveToPos> _moveNext;
        private MapData _mapData;
        private IHomeScreenExtension _homeScreen;
        // private void 

        public void Init(MapData mapData, IHomeScreenExtension homeScreen, GameResource gameResource)
        {
            _mapData = mapData;
            _homeScreen = homeScreen;
            var charCom = homeScreen.MapContent.Character;
            var pos = mapData.GetPosStarEnd();
            _axieAniAttack = new Dictionary<int, AxieAni>();
            _axieAniDefend = new Dictionary<int, AxieAni>();
            foreach (var p in pos.starts)
            {
                var axieAni = AxieAni.Create(charCom, homeScreen.GetPos(p.y, p.x), gameResource.MatchResource.Attacker);
                _axieAniAttack[mapData.GetTileIndex(p.y, p.x)] = axieAni;
            }

            foreach (var p in pos.ends)
            {
                var axieAni = AxieAni.Create(charCom, homeScreen.GetPos(p.y, p.x), gameResource.MatchResource.Defender);
                _axieAniDefend[mapData.GetTileIndex(p.y, p.x)] = axieAni;
            }

            // ReProcessMove
            _moveNext = MapDataUtil.FindPathingAttacker(mapData);
        }

        public void DoPreviewStep()
        {
        }

        public void DoNextStep()
        {
            // Move
            foreach (var move in _moveNext)
            {
                if (move.Next == move.Start) continue;
                if (move.Next == null) continue;
                var mapData = _mapData;
                mapData.DoMove(move);
                // var ani = axieAniAll[];
                _axieAniAttack.Remove(mapData.GetTileIndex(move.Start.Row, move.Start.Col), out AxieAni ani);
                _axieAniAttack[mapData.GetTileIndex(move.Next.Row, move.Next.Col)] = ani;
                ani.AxieCom.TweenMove(_homeScreen.GetPos(move.Next.Row, move.Next.Col), 0.5f);
            }

            // MapDataUtil.RenderMapData(_homeScreen.Map.Content.ListSlot, mapdata);
            _moveNext = MapDataUtil.FindPathingAttacker(_mapData);
        }
    }
}