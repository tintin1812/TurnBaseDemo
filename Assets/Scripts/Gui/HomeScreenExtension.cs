using System.Collections.Generic;
using Data;
using FairyGUI;
using Plugins.PathFinding;
using UnityEngine;
using Utility;

// ReSharper disable All
namespace Gui
{
    public class HomeScreenExtension
    {
        private const string PathPackAge = "Gui/Gui";
        private HomeScreen _homeScreen;

        public void Init(GameData gameData)
        {
            FontManager.RegisterFont(FontManager.GetFont("m6x11"), "");
            UIPackage.AddPackage(PathPackAge);
            GuiBinder.BindAll();
            // Map Sound
            UIConfig.buttonSound = (NAudioClip)UIPackage.GetItemAsset("Gui", "sfx_pop");
            _homeScreen = HomeScreen.CreateInstance();
            _homeScreen.MakeFullScreen();
            GRoot.inst.AddChild(_homeScreen);
            InitZoom();
            // TestAStar(gameData);
            var mapdata = MapDataUtil.GenExMap();
            var listSlot = _homeScreen.Map.Content.ListSlot;
            MapDataUtil.RenderMapData(listSlot, mapdata);
            var charCom = _homeScreen.Map.Content.Character;
            charCom.size = listSlot.size;
            charCom.xy = listSlot.xy;
            var pos = mapdata.GetPosStarEnd();
            List<AxieAni> attackers = new List<AxieAni>();
            Dictionary<int, AxieAni> axieAniAttack = new Dictionary<int, AxieAni>();
            Dictionary<int, AxieAni> axieAniDefend = new Dictionary<int, AxieAni>();
            foreach (var p in pos.starts)
            {
                var axieAni = AxieAni.Create(charCom, MapDataUtil.GetPosByTile(listSlot, mapdata, p.y, p.x), gameData.MatchData.Attacker);
                axieAniAttack[mapdata.GetTileIndex(p.y, p.x)] = axieAni;
            }

            foreach (var p in pos.ends)
            {
                var axieAni = AxieAni.Create(charCom, MapDataUtil.GetPosByTile(listSlot, mapdata, p.y, p.x), gameData.MatchData.Defender);
                axieAniDefend[mapdata.GetTileIndex(p.y, p.x)] = axieAni;
            }

            var movesNext = MapDataUtil.FindPathingAttacker(mapdata);

            _homeScreen.BtNext.setOnClick(() =>
            {
                // Move
                foreach (var move in movesNext)
                {
                    if (move.Next == move.Start) continue;
                    if (move.Next == null) continue;
                    mapdata.DoMove(move);
                    // var ani = axieAniAll[];
                    axieAniAttack.Remove(mapdata.GetTileIndex(move.Start.Row, move.Start.Col), out AxieAni ani);
                    axieAniAttack[mapdata.GetTileIndex(move.Next.Row, move.Next.Col)] = ani;
                    ani.AxieCom.TweenMove(MapDataUtil.GetPosByTile(listSlot, mapdata, move.Next.Row, move.Next.Col), 0.5f);
                }

                // MapDataUtil.RenderMapData(_homeScreen.Map.Content.ListSlot, mapdata);
                movesNext = MapDataUtil.FindPathingAttacker(mapdata);
            });
        }

        private void InitZoom()
        {
            // new DragGesture(_homeScreen.Map.Content);
            _homeScreen.Map.Content.draggable = true;
            _homeScreen.SliderZoom.value = 50;
            SetZoom(_homeScreen.Map, (float)_homeScreen.SliderZoom.value); //
            _homeScreen.SliderZoom.onChanged.Add(() =>
            {
                SetZoom(_homeScreen.Map, (float)_homeScreen.SliderZoom.value); //   
            });

            var gesture = new PinchGesture(_homeScreen.Map.Content);
            gesture.onAction.Add(() =>
            {
                var vTo = (float)_homeScreen.SliderZoom.value + gesture.delta;
                _homeScreen.SliderZoom.value = Mathf.Clamp(vTo, 0.0f, 1.0f);
                SetZoom(_homeScreen.Map, (float)_homeScreen.SliderZoom.value); //
            });
        }

        private static void SetZoom(GObject map, float percent)
        {
            const float min = 0.5f;
            const float max = 1.4f;
            var scaleTo = min + percent * (max - min) * 0.01f;
            map.scale = new Vector2(scaleTo, scaleTo);
        }

        private void TestAStar(GameData gameData)
        {
            var tileGrid = new TileGrid(20, 15);
            tileGrid.CreateExpensiveArea(3, 3, 9, 1);
            tileGrid.CreateExpensiveArea(3, 11, 1, 9);
            var start = tileGrid.GetTile(9, 2);
            var endList = new List<Tile>();
            endList.Add(tileGrid.GetTile(7, 14));
            endList.Add(tileGrid.GetTile(9, 15));
            endList.Add(tileGrid.GetTile(10, 14));
            // tileGrid.FindPath(start, end, PathFinder.FindPath_AStar);
            var r = tileGrid.FindPathMultiEnd(start, endList, PathFinder.FindPath_Dijkstra_MultiEnd);
            var paths = r.paths;
            var listSlot = _homeScreen.Map.Content.ListSlot;
            listSlot.columnCount = tileGrid.Cols;
            listSlot.numItems = tileGrid.Tiles.Length;
            var defaultItemSize = listSlot.GetChildAt(0).size;
            var containerSize = new Vector2(defaultItemSize.x * tileGrid.Cols, defaultItemSize.y * tileGrid.Rows);
            listSlot.size = containerSize;
            listSlot.Center();
            _homeScreen.Map.Content.Bg.size = containerSize;
            for (var col = 0; col < tileGrid.Cols; col++)
            {
                for (var row = 0; row < tileGrid.Rows; row++)
                {
                    var tile = tileGrid.GetTile(row, col);
                    var slot = (SlotMap)listSlot.GetChildAt(tileGrid.GetTileIndex(row, col));
                    slot.Number.text = tile.Cost < tileGrid.TileWeightExpensive ? tile.Cost.ToString() : "";
                    slot.Bg.visible = true;
                    slot.Image.visible = false;
                    if (start == tile)
                    {
                        slot.Image.ReloadData(gameData.MatchData.Attacker);
                    }
                    else if (endList.Find(it => it == tile) != null)
                    {
                        slot.Image.ReloadData(gameData.MatchData.Defender);
                    }
                    else if (tile.Weight == tileGrid.TileWeightExpensive)
                    {
                        // wall
                        slot.Bg.color = Color.gray;
                        slot.Image.visible = false;
                    }
                    else
                    {
                        slot.Bg.color = Color.white;
                    }
                }
            }

            foreach (var tile in paths)
            {
                var slot = (SlotMap)listSlot.GetChildAt(tileGrid.GetTileIndex(tile.Row, tile.Col));
                slot.Bg.visible = true;
                slot.Bg.color = Color.green;
            }
        }
    }
}