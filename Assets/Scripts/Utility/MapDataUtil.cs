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
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // 1
                { 1, 0, 0, 0, 0, 0, 0, 3, 3, 0, 3, 1 }, // 2
                { 1, 2, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1 }, // 3
                { 1, 2, 0, 0, 1, 2, 1, 0, 0, 0, 3, 1 }, // 4
                { 1, 2, 0, 0, 1, 2, 1, 0, 0, 0, 0, 1 }, // 5
                { 1, 0, 0, 0, 1, 3, 1, 0, 0, 3, 0, 1 }, // 6
                { 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1 }, // 7
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 3, 0, 1 }, // 8
                { 1, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 1 }, // 9
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // 10
            };
            return new MapData(tiles);
        }

        public static Vector2 GetPosByTile(GList listSlot, MapData mapData, int row, int col)
        {
            var p = listSlot.GetChildAt(mapData.GetTileIndex(row, col)).xy;
            return p;
        }

        public static void RenderMap(GList listSlot, MapData mapData)
        {
            listSlot.columnCount = mapData.Cols;
            listSlot.numItems = mapData.Cols * mapData.Rows;
            var defaultItemSize = listSlot.GetChildAt(0).size;
            var containerSize = new Vector2(defaultItemSize.x * mapData.Cols, defaultItemSize.y * mapData.Rows);
            listSlot.size = containerSize;
            listSlot.Center();
            listSlot.EnsureBoundsCorrect();
            for (var col = 0; col < mapData.Cols; col++)
            {
                for (var row = 0; row < mapData.Rows; row++)
                {
                    var tile = mapData.GetTile(row, col);
                    var slot = (SlotMap)listSlot.GetChildAt(mapData.GetTileIndex(row, col));
                    slot.Image.visible = false;
                    slot.Number.text = "";
                    slot.BgWall.visible = false;
                    switch (tile)
                    {
                        case MapData.TitleType.Empty:
                            break;
                        case MapData.TitleType.Wall:
                            slot.BgWall.visible = true;
                            break;
                        case MapData.TitleType.Attacker:
                            break;
                        case MapData.TitleType.Defender:
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

        private static (List<Vector2Int> starts, List<Vector2Int> ends) GetPosWithPriorityHpLow(this BattleStage battleStage)
        {
            var attacker = battleStage.Attackers.OrderBy(it => it.Hp).ToList();
            var starts = new List<Vector2Int>();
            foreach (var axieHolder in attacker)
            {
                starts.Add(axieHolder.TilePos);
            }

            var defenders = battleStage.Defenders.OrderBy(it => it.Hp).ToList();
            var ends = new List<Vector2Int>();

            foreach (var axieHolder in defenders)
            {
                ends.Add(axieHolder.TilePos);
            }

            return (starts, ends);
        }

        public static List<MoveToPos> FindPathingAttacker(this BattleStage battleStage)
        {
            var mapData = battleStage.MapData;
            var tileGrid = new TileGrid(mapData.Cols, mapData.Rows);
            var pos = battleStage.GetPosWithPriorityHpLow();
            var starts = pos.starts.Select(p => tileGrid.GetTile(p.y, p.x)).ToList();
            var ends = pos.ends.Select(p => tileGrid.GetTile(p.y, p.x)).ToList();
            var steps = DoFindMultiPosToPos(starts, ends, tileGrid, mapData);

            // Show Indicator
            for (var index = 0; index < steps.Count; index++)
            {
                var step = steps[index];
                var axie = battleStage.GetAxieAt(step.Start.Row, step.Start.Col);
                if (step.Opponent != null)
                {
                    axie.ShowIndicatorAttack(step.Opponent.Col - step.Start.Col, step.Opponent.Row - step.Start.Row);
                }
                else if (step.Next != step.Start)
                {
                    axie.ShowIndicatorMove(step.Next.Col - step.Start.Col, step.Next.Row - step.Start.Row);
                }
                else
                {
                    axie.HideIndicator();
                }
            }

            return steps;
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
            DoFindMultiPosToPosSub(result, map, ends, tileGrid, mapData);
            // Check SubMove when can't move 
            foreach (var r1 in result)
            {
                if (r1.Opponent != null) continue;
                if (r1.Start != r1.Next) continue;
                if (r1.SubNext == null) continue;
                var isCanMove = true;
                foreach (var r2 in result)
                {
                    if (r1 == r2) continue;
                    if (r1.SubNext != r2.Next) continue;
                    isCanMove = false;
                    break;
                }

                if (!isCanMove) continue;
                // Set NextSub is Next
                r1.Next = r1.SubNext;
            }

            return result;
        }

        private static void DoFindMultiPosToPosSub(List<MoveToPos> result, List<MoveToPos> moveCheck, List<Tile> ends, TileGrid tileGrid, MapData mapData)
        {
            var preFind = new List<MetaFindPos>();
            foreach (var find in moveCheck)
            {
                if (find.Finish) continue;
                // FindPaths
                tileGrid.ResetGridWall(mapData);

                foreach (var f in result)
                {
                    if (f == find) continue;
                    if (f.Next != null) tileGrid.SetExpensiveArea(f.Next.Row, f.Next.Col);
                    if (f.Des != null) tileGrid.SetExpensiveArea(f.Des.Row, f.Des.Col);
                }

                var r = tileGrid.FindPathMultiEnd(find.Start, ends, PathFinder.FindPath_Dijkstra_MultiEnd);
                preFind.Add(new MetaFindPos()
                {
                    Ref = find, //
                    Paths = r.paths, //
                });
                if (result.Count <= 0 && r.paths.Count >= 3)
                {
                    find.SubNext = r.paths[1];
                }
            }

            if (preFind.Count <= 0)
            {
                return;
            }

            preFind = preFind.OrderBy(it => it.Paths.Count == 0 ? tileGrid.TileWeightExpensive : it.Paths.Count).ToList();
            var pMin = preFind.First();
            if (pMin.Paths.Count <= 4)
            {
                // no moveAble
                pMin.UpdateFound();
                result.Add(pMin.Ref);
                // Check other path can move 
                DoFindMultiPosToPosSub(result, moveCheck, ends, tileGrid, mapData);
            }
            else
            {
                foreach (var pCheck in preFind)
                {
                    if (pCheck.Paths == null || pCheck.Paths.Count <= 2)
                    {
                        break;
                    }

                    // Check Paths Can Move
                    var isCanMove = true;
                    var posNextStep = pCheck.Paths[1];
                    foreach (var r in moveCheck)
                    {
                        if (r == pCheck.Ref) continue;
                        var posStand = r.Next ?? r.Start;
                        if (posStand != posNextStep) continue;
                        isCanMove = false;
                        break;
                    }

                    if (!isCanMove) continue;
                    pCheck.UpdateFound();
                    result.Add(pCheck.Ref);
                }

                // Check other path can move 
                DoFindMultiPosToPosSub(result, moveCheck, ends, tileGrid, mapData);
            }
        }

        public class MoveToPos
        {
            public Tile Start;
            public Tile Next;
            public Tile Des;
            public Tile Opponent;
            public bool Finish;

            // option sub when can't move
            public Tile SubNext;
        }

        private class MetaFindPos
        {
            public MoveToPos Ref;
            public List<Tile> Paths;

            public void UpdateFound()
            {
                Debug.Assert(Ref.Finish == false);
                Ref.Finish = true;
                if (Paths is { Count: 2 })
                {
                    Ref.Opponent = Paths.Last();
                }

                if (Paths is not { Count: > 2 })
                {
                    Ref.Next = Ref.Start;
                }
                else
                {
                    Ref.Next = Paths[1];
                    if (Paths.Count is > 3 and <= 4)
                    {
                        Ref.Des = Paths[^2];
                    }
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