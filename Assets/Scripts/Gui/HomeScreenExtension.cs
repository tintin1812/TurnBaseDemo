using Data;
using FairyGUI;
using Plugins.PathFinding;
using UnityEngine;
using Utility;

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
            MapDataUtil.ApplyRenderMapData(_homeScreen.Map.Content.ListSlot, MapDataUtil.GenExMap(), gameData);
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
            var end = tileGrid.GetTile(7, 14);
            tileGrid.FindPath(start, end, PathFinder.FindPath_AStar);
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
                    var slot = (SlotAxie)listSlot.GetChildAt(tileGrid.GetTileIndex(row, col));
                    slot.Number.text = tile.Cost < tileGrid.TileWeightExpensive ? tile.Cost.ToString() : "";
                    if (tile.Cost >= tileGrid.TileWeightExpensive)
                    {
                        // wall
                        slot.Bg.visible = true;
                        slot.Bg.color = Color.gray;
                        slot.Image.visible = false;
                    }
                    else if (start == tile)
                    {
                        slot.ReloadData(gameData.MatchData.Attacker);
                    }
                    else if (end == tile)
                    {
                        slot.ReloadData(gameData.MatchData.Defender);
                    }
                    else
                    {
                        // slot.Image.visible = false;
                    }
                }
            }

            var tileRender = end.PrevTile;
            while (tileRender != null)
            {
                var slot = (SlotAxie)listSlot.GetChildAt(tileGrid.GetTileIndex(tileRender.Row, tileRender.Col));
                slot.Bg.visible = true;
                slot.Bg.color = Color.green;
                tileRender = tileRender.PrevTile;
                slot.Image.visible = false;
                if (tileRender == start)
                {
                    break;
                }
            }
        }
    }
}