using System;
using Data;
using FairyGUI;
using Gui;
using UnityEngine;

namespace Utility
{
    public static class MapDataUtil
    {
        public static MapData GenExMap()
        {
            // 0: empty
            // 1: wall
            // 2: attacker
            // 3: defender
            var tiles = new short[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 1
                { 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 2
                { 0, 2, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 }, // 3
                { 0, 2, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 }, // 4
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, // 5
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 3, 0 }, // 6
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 3, 0 }, // 7
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3, 0 }, // 8
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 9
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 10
            };
            return new MapData(tiles);
        }

        public static void ApplyRenderMapData(GList listSlot, MapData mapData, GameData gameData)
        {
            listSlot.columnCount = mapData.Cols;
            listSlot.numItems = mapData.Cols * mapData.Rows;
            var defaultItemSize = listSlot.GetChildAt(0).size;
            var containerSize = new Vector2(defaultItemSize.x * mapData.Cols, defaultItemSize.y * mapData.Rows);
            listSlot.size = containerSize;
            listSlot.Center();
            // _homeScreen.Map.Content.Bg.size = containerSize;
            for (var col = 0; col < mapData.Cols; col++)
            {
                for (var row = 0; row < mapData.Rows; row++)
                {
                    var tile = mapData.GetTile(row, col);
                    var slot = (SlotAxie)listSlot.GetChildAt(mapData.GetTileIndex(row, col));
                    slot.Image.visible = false;
                    slot.Bg.visible = false;
                    switch (tile)
                    {
                        case MapData.TitleType.Empty:
                            slot.Bg.visible = true;
                            slot.Bg.color = Color.white;
                            break;
                        case MapData.TitleType.Wall:
                            // wall
                            slot.Bg.visible = true;
                            slot.Bg.color = Color.gray;
                            break;
                        case MapData.TitleType.Attacker:
                            slot.Image.visible = true;
                            slot.ReloadData(gameData.MatchData.Attacker);
                            break;
                        case MapData.TitleType.Defender:
                            slot.Image.visible = true;
                            slot.ReloadData(gameData.MatchData.Defender);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}