using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using FairyGUI;
using Gui;
using Plugins.PathFinding;
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
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }, // 1
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }, // 2
                { 0, 2, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 }, // 3
                { 0, 2, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 }, // 4
                { 0, 2, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 }, // 5
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 3, 0 }, // 6
                { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 }, // 7
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0 }, // 8
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }, // 9
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }, // 10
            };
            return new MapData(tiles);
        }

        public static Vector2 GetPosByTile(GList listSlot, MapData mapData, int row, int col)
        {
            var p = listSlot.GetChildAt(mapData.GetTileIndex(row, col)).xy;
            return p;
        }

        public static void RenderMapData(GList listSlot, MapData mapData)
        {
            listSlot.columnCount = mapData.Cols;
            listSlot.numItems = mapData.Cols * mapData.Rows;
            var defaultItemSize = listSlot.GetChildAt(0).size;
            var containerSize = new Vector2(defaultItemSize.x * mapData.Cols, defaultItemSize.y * mapData.Rows);
            listSlot.size = containerSize;
            listSlot.Center();
            listSlot.EnsureBoundsCorrect();
            // _homeScreen.Map.Content.Bg.size = containerSize;
            for (var col = 0; col < mapData.Cols; col++)
            {
                for (var row = 0; row < mapData.Rows; row++)
                {
                    var tile = mapData.GetTile(row, col);
                    var slot = (SlotMap)listSlot.GetChildAt(mapData.GetTileIndex(row, col));
                    slot.Image.visible = false;
                    slot.Bg.visible = false;
                    slot.Number.text = "";
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
                            slot.Number.text = "W";
                            break;
                        case MapData.TitleType.Attacker:
                            slot.Bg.visible = true;
                            slot.Bg.color = Color.white;
                            // slot.Image.visible = true;
                            // slot.ReloadData(gameData.MatchData.Attacker);
                            // slot.Number.text = "A";
                            break;
                        case MapData.TitleType.Defender:
                            slot.Bg.visible = true;
                            slot.Bg.color = Color.white;
                            // slot.Image.visible = true;
                            // slot.ReloadData(gameData.MatchData.Defender);
                            // slot.Number.text = "D";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public static (List<Vector2Int> starts, List<Vector2Int> ends) GetPosStarEnd(this MapData mapData)
        {
            var starts = new List<Vector2Int>();
            var ends = new List<Vector2Int>();
            for (var col = 0; col < mapData.Cols; col++)
            {
                for (var row = 0; row < mapData.Rows; row++)
                {
                    var tile = mapData.GetTile(row, col);
                    switch (tile)
                    {
                        case MapData.TitleType.Empty:
                            break;
                        case MapData.TitleType.Wall:
                            break;
                        case MapData.TitleType.Attacker:
                            starts.Add(new Vector2Int(col, row));
                            break;
                        case MapData.TitleType.Defender:
                            ends.Add(new Vector2Int(col, row));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return (starts, ends);
        }

        public static List<MoveToPos> FindPathingAttacker(MapData mapData)
        {
            var tileGrid = new TileGrid(mapData.Cols, mapData.Rows);
            var pos = mapData.GetPosStarEnd();
            var starts = pos.starts.Select(p => tileGrid.GetTile(p.y, p.x)).ToList();
            var ends = pos.ends.Select(p => tileGrid.GetTile(p.y, p.x)).ToList();
            var moves = DoFindMultiPosToPos(starts, ends, tileGrid, mapData);
            // Debug render
            /*
            for (var index = 0; index < moves.Count; index++)
            {
                var move = moves[index];
                var slotStart = (SlotMap)listSlot.GetChildAt(tileGrid.GetTileIndex(move.Start.Row, move.Start.Col));
                var x = move.Next.Col - move.Start.Col;
                var y = move.Next.Row - move.Start.Row;
                slotStart.Number.text = $"{x};{y}";
            }
            */

            return moves;
        }

        private static void ResetGridWall(this TileGrid tileGrid, MapData mapData)
        {
            for (var col = 0; col < mapData.Cols; col++)
            {
                for (var row = 0; row < mapData.Rows; row++)
                {
                    var tile = mapData.GetTile(row, col);
                    if (tile == MapData.TitleType.Wall)
                    {
                        tileGrid.SetExpensiveArea(row, col);
                    }
                    else
                    {
                        tileGrid.ResetWeight(row, col);
                    }
                }
            }
        }

        private static List<MoveToPos> DoFindMultiPosToPos(List<Tile> starts, List<Tile> ends, TileGrid tileGrid, MapData mapData)
        {
            var map = new List<MoveToPos>();
            foreach (var start in starts)
            {
                map.Add(new MoveToPos() { Start = start });
            }

            var result = new List<MoveToPos>();
            DoFindMultiPosToPosSub(result, map, starts, ends, tileGrid, mapData);
            return result;
        }

        private static void DoFindMultiPosToPosSub(List<MoveToPos> result, List<MoveToPos> moveCheck, List<Tile> starts, List<Tile> ends, TileGrid tileGrid, MapData mapData)
        {
            var preFind = new List<MetaFindMultiPosToPos>();
            foreach (var find in moveCheck)
            {
                if (find.Finish) continue;
                // FindPaths
                tileGrid.ResetGridWall(mapData);

                foreach (var f in moveCheck)
                {
                    if (f == find) continue;
                    if (f.Next != null)
                    {
                        tileGrid.SetExpensiveArea(f.Next.Row, f.Next.Col);
                    }
                }

                var r = tileGrid.FindPathMultiEnd(find.Start, ends, PathFinder.FindPath_Dijkstra_MultiEnd);
                preFind.Add(new MetaFindMultiPosToPos()
                {
                    Ref = find, //
                    Paths = r.paths, //
                });
            }

            if (preFind.Count <= 0)
            {
                return;
            }

            preFind = preFind.OrderBy(it => it.Paths.Count == 0 ? tileGrid.TileWeightExpensive : it.Paths.Count).ToList();
            var pMin = preFind.First();
            if (pMin.Paths.Count <= 2)
            {
                // no moveAble
                pMin.UpdateFound();
                result.Add(pMin.Ref);
                // Check other path can move 
                DoFindMultiPosToPosSub(result, moveCheck, starts, ends, tileGrid, mapData);
            }
            else
            {
                for (var idx = 0; idx < preFind.Count; idx++)
                {
                    var pCheck = preFind[idx];
                    if (pCheck.Paths == null || pCheck.Paths.Count <= 2)
                    {
                        break;
                    }

                    // Check can move to
                    var posMoveTo = pCheck.Paths[1];
                    var isCanMove = true;
                    foreach (var m in moveCheck)
                    {
                        if (m == pCheck.Ref) continue;
                        if (m.Next != null)
                        {
                            if (m.Next != posMoveTo) continue;
                            isCanMove = false;
                            break;
                        }
                        else
                        {
                            if (m.Start != posMoveTo) continue;
                            isCanMove = false;
                            break;
                        }
                    }

                    if (!isCanMove) continue;
                    pCheck.UpdateFound();
                    result.Add(pCheck.Ref);
                }

                // Check other path can move 
                DoFindMultiPosToPosSub(result, moveCheck, starts, ends, tileGrid, mapData);
            }
        }

        public class MoveToPos
        {
            public Tile Start;
            public Tile Next;
            public bool Finish;
        }

        private class MetaFindMultiPosToPos
        {
            public MoveToPos Ref;
            public List<Tile> Paths;

            public void UpdateFound()
            {
                Ref.Finish = true;
                if (Paths == null || Paths.Count <= 2)
                {
                    Ref.Next = Ref.Start;
                }
                else
                {
                    Ref.Next = Paths[1];
                }
            }
        }

        public static void DoMove(this MapData mapData, Tile from, Tile to)
        {
            if (to == from) return;
            if (to == null) return;
            if (mapData.GetTile(to.Row, to.Col) != MapData.TitleType.Empty)
            {
                Debug.LogWarning("Tile move next is not Empty!");
                return;
            }

            var typeCurrent = mapData.GetTile(from.Row, from.Col);
            if (typeCurrent != MapData.TitleType.Attacker)
            {
                Debug.LogWarning("Tile move start is not Attacker!");
                return;
            }

            mapData.SetTile(from.Row, from.Col, MapData.TitleType.Empty);
            mapData.SetTile(to.Row, to.Col, typeCurrent);
        }

        public static void RemoveTile(this MapData mapData, int row, int col)
        {
            if (mapData.GetTile(row, col) == MapData.TitleType.Empty)
            {
                Debug.LogWarning("Tile request Remove is  Empty!");
                return;
            }

            mapData.SetTile(row, col, MapData.TitleType.Empty);
        }
    }
}