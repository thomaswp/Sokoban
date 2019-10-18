using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    public abstract class Generator
    {
        public readonly int width, height, nPlayers, nBoxes, nSwitches;

        public abstract LevelState Generate();

        protected Random rand = new Random(1234);

        public Generator(int width, int height, int nPlayers, int nBoxes)
        {
            this.width = width;
            this.height = height;
            this.nPlayers = nPlayers;
            this.nBoxes = nBoxes;
            nSwitches = nBoxes;
        }

        public int Score(LevelState state)
        {
            Solver.Solution solution = Solver.FindPath(state);
            return solution.Solved ? solution.Explored : 0;
        }

        public LevelState GenerateBest(int n)
        {
            int bestScore = 0;
            LevelState bestLevel = null;
            int p = 0;
            int solvable = 0;
            for (int i = 0; i < n; i++)
            {
                LevelState level = Generate();
                int score = Score(level);
                if (score > 0) solvable++;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestLevel = level;
                }
                while (p < i * 25 / n) { Console.WriteLine((p++ * 4) + ": " + (solvable * 100 / (i+1)));}
            }
            Console.WriteLine();
            Console.WriteLine(bestScore);
            return bestLevel;
        }
    }

    public class RandomGenerator : Generator
    {
        public RandomGenerator(int width, int height, int nPlayers, int nBoxes) : base(width, height, nPlayers, nBoxes) { }

        public override LevelState Generate()
        {
            int nWalls = (int) Math.Round((0.1 + rand.NextDouble() * 0.3) * width * height);

            int[,] state = new int[width, height];

            List<Point> open = new List<Point>();
            for (int i = 0; i < width; i++) for (int j = 0; j < height; j++) open.Add(new Point(i, j));

            // TODO: generate intelligently so it's always pathable, if not solvable
            Point[] playerOrder = AddRandomly(state, nPlayers, Actor.Player, open, true);
            AddRandomly(state, nBoxes, Actor.Box, open, true, true);
            AddRandomly(state, nSwitches, Actor.Switch, open, false);
            AddRandomly(state, nWalls, Actor.Wall, open, true);
            
            return new LevelState(state, playerOrder);
        }

        private Point[] AddRandomly(int[,] state, int n, Actor actor, List<Point> open, bool emptyOnly, bool noEdges = false)
        {
            Point[] points = new Point[n];
            for (int i = 0; i < n; i++)
            {
                if (emptyOnly && open.Count == 0) return points;
                Point p;
                if (!emptyOnly) p = new Point(rand.Next(width), rand.Next(height));
                else
                {
                    int index = rand.Next(open.Count);
                    p = open[index];
                    open.RemoveAt(index);
                }
                if (noEdges && p.X == 0 || p.Y == 0 || p.X == state.GetLength(0) - 1 || p.Y == state.GetLength(1) - 1)
                {
                    i--;
                    continue;
                }
                if (actor == Actor.Wall)
                {
                    if (!Connected(state, p))
                    {
                        i--;
                        continue;
                    }
                }
                points[i] = p;
                state[p.X, p.Y] |= actor.id;
            }
            return points;
        }

        private static bool Connected(int[,] state, Point without)
        {
            List<Point> nonWalls = new List<Point>();
            int width = state.GetLength(0), height = state.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!Actor.Wall.IsAt(state[i, j])) nonWalls.Add(new Point(i, j));
                }
            }
            nonWalls.Remove(without);
            List<Point> connected = new List<Point>();
            List<Point> toExplore = new List<Point>();
            toExplore.Add(nonWalls[0]);
            while (toExplore.Count > 0)
            {
                Point p = toExplore[0];
                connected.Add(p);
                toExplore.RemoveAt(0);
                int[] dxs = { 1, -1, 0, 0 };
                int[] dys = { 0, 0, 1, -1 };
                for (int i = 0; i < 4; i++)
                {
                    int x = p.X + dxs[i];
                    int y = p.Y + dys[i];
                    Point q = new Point(x, y);
                    if (q == without) continue;
                    if (x < 0 || y < 0 || x >= width || y >= height) continue;
                    if (Actor.Wall.IsAt(state[x, y]) || connected.Contains(q) || toExplore.Contains(q)) continue;
                    toExplore.Add(q);
                }
            }
            return nonWalls.All(p => connected.Contains(p));
        }
    }

    
    // TODO: Generator that creates the solution first, then fills in walls
}
