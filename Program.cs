﻿using System;
using System.Collections.Generic;

namespace SimpleMazeGame
{
    class Program
    {
        static int playerX = 1;
        static int playerY = 1;

        static int exitX = 0;
        static int exitY = 0;
        
        const int width = 21;
        const int height = 21;
        
        const char player = '■';
        const char exit = 'E';
        const char wall = '#';
        const char path = ' ';
        const char border = '|';
        
        static void Main(string[] args)
        {
            // seed is current time in milliseconds
            int seed = (int)DateTime.Now.Ticks / 10000;
            bool[,] walls = GenerateMaze(width, height, seed);

            bool gameOver = false;

            while (!gameOver)
            {
                Console.Clear();

                // Print the maze
                DrawMaze(walls);

                // Get the player's input
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                ConsoleKey key = keyInfo.Key;

                // Update the player's position
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (playerY > 0 && !walls[playerY - 1, playerX])
                        {
                            playerY--;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (playerY < height - 1 && !walls[playerY + 1, playerX])
                        {
                            playerY++;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (playerX > 0 && !walls[playerY, playerX - 1])
                        {
                            playerX--;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (playerX < width - 1 && !walls[playerY, playerX + 1])
                        {
                            playerX++;
                        }
                        break; 
                    case ConsoleKey.R:
                        // regenerate the maze
                        seed = (int)DateTime.Now.Ticks / 10000;
                        walls = GenerateMaze(width, height, seed);
                        playerX = 1;
                        playerY = 1;
                        break;
                    
                }

                // Check if the player has reached the exit
                if (playerX == exitX && playerY == exitY)
                {
                    Console.WriteLine("Congratulations, you have reached the exit!");
                    gameOver = true;
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void DrawMaze(bool[,] walls)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == playerX && y == playerY)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(player);
                        Console.ResetColor();
                    }
                    else if (x == exitX && y == exitY)
                    {
                        Console.Write(exit);
                    }
                    else if (walls[y, x])
                    {
                        Console.Write(wall);
                    }
                    else
                    {
                        Console.Write(path);
                    }

                    Console.Write(border);
                }

                Console.WriteLine();
            }
        }

        public static bool[,] GenerateMaze(int width, int height, int seed)
        {
            bool[,] maze = new bool[width, height];

            // Initialize all cells as walls
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    maze[x, y] = false;
                }
            }

            // make walls around the maze
            for (int x = 0; x < width; x++)
            {
                maze[x, 0] = true;
                maze[x, height - 1] = true;
            }
            for (int y = 0; y < height; y++)
            {
                maze[0, y] = true;
                maze[width - 1, y] = true;
            }

            // initialize the random number generator with the given seed value
            Random rand = new Random(seed);

            // level generation using recursive backtracking
            int startX = rand.Next(1, width - 2);
            int startY = rand.Next(1, height - 2);
            RecursiveBacktracking(maze, startX, startY, rand);
            
            // add random points to the maze with open paths
            for (int i = 0; i < width / 3; i++)
            {
                int x = rand.Next(1, width - 2);
                int y = rand.Next(1, height - 2);
                maze[x, y] = false;
            }
            
            // place finish at random location
            int finishX = rand.Next(1, width - 2);
            int finishY = rand.Next(1, height - 2);
            maze[finishX, finishY] = false;
            exitX = finishX;
            exitY = finishY;

            return maze;
        }

        private static void RecursiveBacktracking(bool[,] maze, int x, int y, Random rand)
        {
            maze[x, y] = true;

            List<int[]> directions = new List<int[]>
            {
                new int[] { -1, 0 }, // left
                new int[] { 1, 0 }, // right
                new int[] { 0, -1 }, // up
                new int[] { 0, 1 } // down
            };

            // shuffle the directions randomly
            for (int i = 0; i < directions.Count; i++)
            {
                int r = rand.Next(i, directions.Count);
                (directions[r], directions[i]) = (directions[i], directions[r]);
            }

            // recursive backtracking
            foreach (int[] direction in directions)
            {
                int dx = direction[0];
                int dy = direction[1];

                int newX = x + dx * 2;
                int newY = y + dy * 2;

                if (newX >= 1 && newX < maze.GetLength(0) - 1 &&
                    newY >= 1 && newY < maze.GetLength(1) - 1 &&
                    !maze[newX, newY])
                {
                    maze[x + dx, y + dy] = true;
                    RecursiveBacktracking(maze, newX, newY, rand);
                }
            }
        }
    }
}