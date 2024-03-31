using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.PathFinding
{
    public class TileGrid
    {
        private const int TileWeightDefault = 1;
        public int TileWeightExpensive { set; get; }
        private const int TileWeightInfinity = int.MaxValue;
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public Tile[] Tiles { get; private set; }

        public TileGrid(int cols, int rows)
        {
            Cols = cols;
            Rows = rows;
            Tiles = new Tile[Rows * Cols];
            for (var r = 0; r < Rows; r++)
            {
                for (var c = 0; c < Cols; c++)
                {
                    var tile = new Tile(r, c, TileWeightDefault);
                    var index = GetTileIndex(r, c);
                    Tiles[index] = tile;
                }
            }

            TileWeightExpensive = rows * cols;
            ResetGrid();
        }

        public void SetExpensiveArea(int row, int col)
        {
            GetTile(row, col).Weight = TileWeightExpensive;
        }

        public void ResetWeight(int row, int col)
        {
            GetTile(row, col).Weight = TileWeightDefault;
        }

        public void CreateExpensiveArea(int row, int col, int width, int height)
        {
            CreateExpensiveArea(row, col, width, height, TileWeightExpensive);
        }

        private void CreateExpensiveArea(int row, int col, int width, int height, int weight)
        {
            for (var r = row; r < row + height; r++)
            {
                for (var c = col; c < col + width; c++)
                {
                    var tile = GetTile(r, c);
                    if (tile != null)
                    {
                        tile.Weight = weight;
                    }
                }
            }
        }

        private void ResetGrid()
        {
            foreach (var tile in Tiles)
            {
                tile.Cost = 0;
                tile.PrevTile = null;
            }
        }

        public List<IVisualStep> FindPath(Tile start, Tile end, Func<TileGrid, Tile, Tile, List<IVisualStep>, List<Tile>> pathFindingFunc)
        {
            ResetGrid();

            var steps = new List<IVisualStep>();
            pathFindingFunc(this, start, end, steps);
            return steps;
            // foreach (var step in steps)
            // {
            //     step.Execute();
            //     // yield return new WaitForFixedUpdate();
            // }
        }

        public (List<IVisualStep>, List<Tile> paths) FindPathMultiEnd(Tile start, List<Tile> endsList, Func<TileGrid, Tile, List<Tile>, List<IVisualStep>, List<Tile>> pathFindingFunc)
        {
            ResetGrid();
            var steps = new List<IVisualStep>();
            var paths = pathFindingFunc(this, start, endsList, steps);
            return (steps, paths);
        }

        public Tile GetTile(int row, int col)
        {
            if (!IsInBounds(row, col))
            {
                return null;
            }

            return Tiles[GetTileIndex(row, col)];
        }

        public IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            var right = GetTile(tile.Row, tile.Col + 1);
            if (right != null)
            {
                yield return right;
            }

            var up = GetTile(tile.Row - 1, tile.Col);
            if (up != null)
            {
                yield return up;
            }

            var left = GetTile(tile.Row, tile.Col - 1);
            if (left != null)
            {
                yield return left;
            }

            var down = GetTile(tile.Row + 1, tile.Col);
            if (down != null)
            {
                yield return down;
            }
        }

        private bool IsInBounds(int row, int col)
        {
            var rowInRange = row >= 0 && row < Rows;
            var colInRange = col >= 0 && col < Cols;
            return rowInRange && colInRange;
        }

        public int GetTileIndex(int row, int col)
        {
            return row * Cols + col;
        }

        public static Color TileColorDefault = new Color(0.86f, 0.83f, 0.83f);
        public static Color TileColorExpensive = new Color(0.19f, 0.65f, 0.43f);
        public static Color TileColorInfinity = new Color(0.37f, 0.37f, 0.37f);
        public static Color TileColorStart = Color.green;
        public static Color TileColorEnd = Color.red;
        public static Color TileColorPath = new Color(0.73f, 0.0f, 1.0f);
        public static Color TileColorVisited = new Color(0.75f, 0.55f, 0.38f);

        public static Color TileColorFrontier = new Color(0.4f, 0.53f, 0.8f);
        /*
        private IEnumerator _pathRoutine;

        private void Update()
        {
            var start = GetTile(9, 2);
            var end = GetTile(7, 14);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_BFS);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_Dijkstra);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_AStar);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                StopPathCoroutine();
                _pathRoutine = FindPath(start, end, PathFinder.FindPath_GreedyBestFirstSearch);
                StartCoroutine(_pathRoutine);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopPathCoroutine();
                ResetGrid();
                start.SetColor(TileColor_Start);
                end.SetColor(TileColor_End);
            }
        }

        private void StopPathCoroutine()
        {
            if (_pathRoutine != null)
            {
                StopCoroutine(_pathRoutine);
                _pathRoutine = null;
            }
        }
        */
    }
}