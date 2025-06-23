using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Game2D.Engine
{
    public class RandomMap : GameWorld
    {
        private int[,] grid;
        private int rows, cols;
        private int cellSize = 72;
        private (int x, int y) startCell;
        private (int x, int y) portalCell;
        private Random rng = new Random();

        public enum MapDifficulty { Easy, Medium, Hard, Hardcore }

        public RandomMap(Canvas canvas, MapDifficulty difficulty) : base(canvas)
        {
            rows = (int)(canvas.ActualHeight / cellSize);
            cols = (int)(canvas.ActualWidth / cellSize);
            if (rows < 5) rows = 12; // fallback
            if (cols < 5) cols = 20;
            grid = new int[rows, cols];
            startCell = (1, 1);
            portalCell = (rows - 2, cols - 2);
            GenerateMap(difficulty);
            PlaceWalls();
        }

        private void GenerateMap(MapDifficulty difficulty)
        {
            // 1. Очистить сетку
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    grid[r, c] = 0;
            // 2. Внешние стены
            for (int r = 0; r < rows; r++)
            {
                grid[r, 0] = 1;
                grid[r, cols - 1] = 1;
            }
            for (int c = 0; c < cols; c++)
            {
                grid[0, c] = 1;
                grid[rows - 1, c] = 1;
            }
            // 3. Случайные стены
            int wallCount = difficulty switch
            {
                MapDifficulty.Easy => (rows * cols) / 12,
                MapDifficulty.Medium => (rows * cols) / 9,
                MapDifficulty.Hard => (rows * cols) / 7,
                MapDifficulty.Hardcore => (rows * cols) / 5,
                _ => (rows * cols) / 12
            };
            // 4. Гарантируем путь: генерируем случайный путь от старта к порталу
            var path = GeneratePath(startCell, portalCell);
            var pathSet = new HashSet<(int,int)>(path);
            // 5. Ставим одиночные стены, не перекрывая путь
            int placed = 0;
            while (placed < wallCount)
            {
                int r = rng.Next(1, rows - 1);
                int c = rng.Next(1, cols - 1);
                if (grid[r, c] == 0 && !pathSet.Contains((r, c)))
                {
                    grid[r, c] = 1;
                    placed++;
                }
            }
            // 6. Добавляем длинные горизонтальные стены
            int hWallCount = difficulty switch
            {
                MapDifficulty.Easy => 1,
                MapDifficulty.Medium => 2,
                MapDifficulty.Hard => 3,
                MapDifficulty.Hardcore => 4,
                _ => 1
            };
            for (int i = 0; i < hWallCount; i++)
            {
                int r = rng.Next(2, rows - 2);
                int cStart = rng.Next(1, cols - 5);
                int len = rng.Next(3, 6); // длина горизонтальной стены
                for (int l = 0; l < len && cStart + l < cols - 1; l++)
                {
                    if (grid[r, cStart + l] == 0 && !pathSet.Contains((r, cStart + l)))
                        grid[r, cStart + l] = 1;
                }
            }
            // 7. Гарантируем отсутствие стен на старте и у портала
            grid[startCell.x, startCell.y] = 0;
            grid[portalCell.x, portalCell.y] = 0;
        }

        private List<(int,int)> GeneratePath((int x, int y) from, (int x, int y) to)
        {
            // Простой random walk с bias к порталу
            var path = new List<(int,int)>();
            int x = from.x, y = from.y;
            path.Add((x, y));
            while ((x, y) != to)
            {
                var options = new List<(int,int)>();
                if (x < to.x) options.Add((x + 1, y));
                if (y < to.y) options.Add((x, y + 1));
                if (x > to.x) options.Add((x - 1, y));
                if (y > to.y) options.Add((x, y - 1));
                // Добавим случайности
                if (rng.NextDouble() < 0.3)
                {
                    if (x > 1) options.Add((x - 1, y));
                    if (y > 1) options.Add((x, y - 1));
                    if (x < rows - 2) options.Add((x + 1, y));
                    if (y < cols - 2) options.Add((x, y + 1));
                }
                var next = options[rng.Next(options.Count)];
                if (!path.Contains(next))
                {
                    x = next.Item1;
                    y = next.Item2;
                    path.Add((x, y));
                }
                else
                {
                    // если зациклились, выходим
                    break;
                }
            }
            return path;
        }

        private void PlaceWalls()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (grid[r, c] == 1)
                    {
                        // Горизонтальная или вертикальная?
                        if (r > 0 && r < rows - 1 && (c == 0 || c == cols - 1))
                            AddObject(new Wall_vertical(c * cellSize, r * cellSize));
                        else if (c > 0 && c < cols - 1 && (r == 0 || r == rows - 1))
                            AddObject(new Wall_gorizont(c * cellSize, r * cellSize));
                        else
                            AddObject(new Wall(c * cellSize, r * cellSize, "wall_.png"));
                    }
                }
            }
        }

        public (double x, double y) GetStartPosition() => (startCell.y * cellSize + 8, startCell.x * cellSize + 8);
        public (double x, double y) GetPortalPosition() => (portalCell.y * cellSize + 8, portalCell.x * cellSize + 8);
    }
} 