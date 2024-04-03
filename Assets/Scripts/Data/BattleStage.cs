using System;
using System.Collections.Generic;
using FairyGUI;
using Gui;
using Plugins.PathFinding;
using UnityEngine;
using Utility;

namespace Data
{
    public class BattleStage
    {
        private MapData _mapData;
        private IHomeScreenExtension _homeScreen;
        public List<AxieHolder> Attackers { get; private set; }
        public List<AxieHolder> Defenders { get; private set; }
        public Dictionary<int, AxieHolder> AxieAll { get; private set; }

        public void Init(MapData mapData, IHomeScreenExtension homeScreen, GameResource gameResource)
        {
            _mapData = mapData;
            _homeScreen = homeScreen;
            AxieAll = new Dictionary<int, AxieHolder>();
            Attackers = new List<AxieHolder>();
            Defenders = new List<AxieHolder>();
            var charCom = homeScreen.MapContent.Character;
            var pos = mapData.GetPosStarEnd();
            foreach (var p in pos.starts)
            {
                var axieAni = AxieHolder.Create(charCom, homeScreen.GetPos(p.y, p.x), gameResource.MatchResource.Attacker);
                AxieAll[mapData.GetTileIndex(p.y, p.x)] = axieAni;
                axieAni.FaceToRight(true);
                axieAni.Init(AxieHolder.AxieTeam.Attack, 32);
                axieAni.TilePos = p;
                Attackers.Add(axieAni);
            }

            foreach (var p in pos.ends)
            {
                var axieAni = AxieHolder.Create(charCom, homeScreen.GetPos(p.y, p.x), gameResource.MatchResource.Defender);
                AxieAll[mapData.GetTileIndex(p.y, p.x)] = axieAni;
                axieAni.TilePos = p;
                axieAni.Init(AxieHolder.AxieTeam.Defend, 16);
                Defenders.Add(axieAni);
            }
        }

        public GTweener AxieAniMove(Tile start, Tile next, bool isRevert)
        {
            // Update index tile cache
            AxieAll.Remove(_mapData.GetTileIndex(start.Row, start.Col), out var ani);
            AxieAll[_mapData.GetTileIndex(next.Row, next.Col)] = ani;
            ani.TilePos = next.Pos;
            ani.Status = AxieHolder.AxieStatus.MarkMove;
            if (!isRevert)
            {
                var timeMove = ani.DoAniMove();
                return ani.AxieCom.TweenMove(_homeScreen.GetPos(next.Row, next.Col), timeMove - 0.2f).SetDelay(0.2f);
            }
            else
            {
                return ani.AxieCom.TweenMove(_homeScreen.GetPos(next.Row, next.Col), 0.5f).SetDelay(0.1f);
            }
        }

        public void RemoveAxieAni(AxieHolder axie)
        {
            AxieAll.Remove(_mapData.GetTileIndex(axie.TilePos.y, axie.TilePos.x), out var ani);
            if (axie != ani)
            {
                Debug.LogWarning("RemoveAxieAni not Same Cache");
            }

            switch (axie.Team)
            {
                case AxieHolder.AxieTeam.Attack:
                    Attackers.Remove(axie);
                    break;
                case AxieHolder.AxieTeam.Defend:
                    Defenders.Remove(axie);
                    break;
                case AxieHolder.AxieTeam.Non:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ReviverAxiAni(AxieHolder axie)
        {
            AxieAll[_mapData.GetTileIndex(axie.TilePos.y, axie.TilePos.x)] = axie;
            switch (axie.Team)
            {
                case AxieHolder.AxieTeam.Attack:
                    Attackers.Add(axie);
                    break;
                case AxieHolder.AxieTeam.Defend:
                    Defenders.Add(axie);
                    break;
                case AxieHolder.AxieTeam.Non:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}