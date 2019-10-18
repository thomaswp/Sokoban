using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    public class LevelState
    {
        public readonly int width, height;
        public readonly int[,] state;
        public readonly Point[] playerOrder;

        public LevelState(int[,] state, Point[] playerOrder)
        {
            this.playerOrder = playerOrder;
            this.state = state;
            width = state.GetLength(0);
            height = state.GetLength(1);
        }

        public Actor GetActor(int x, int y)
        {
            if (!IsInBounds(x, y)) return null;
            return Actor.ActorForId(state[x, y]);
        }

        public bool IsInBounds(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= width || y >= height);
        }

        public bool Move(int dx, int dy)
        {
            Point player = playerOrder[0];
            bool success = Move(player.X, player.Y, dx, dy);
            if (success) Advance();
            return success;
        }

        public bool Pass()
        {
            if (playerOrder.Length <= 1) return false;
            Advance();
            return true;
        }

        private void Advance()
        {
            Point first = playerOrder[0];
            for (int i = 1; i < playerOrder.Length; i++)
            {
                playerOrder[i - 1] = playerOrder[i];
            }
            playerOrder[playerOrder.Length - 1] = first;
        }

        private bool Move(int x, int y, int dx, int dy)
        {
            if (dx == 0 && dy == 0) return false;
            int nx = x + dx, ny = y + dy;
            if (!IsInBounds(nx, ny)) return false;
            Actor here = GetActor(x, y);
            Actor there = GetActor(nx, ny);
            if (!there.passable)
            {
                if (!there.pushable) return false;
                if (!Move(nx, ny, dx, dy)) return false;
            }
            
            int playerIndex = Array.IndexOf(playerOrder, new Point(x, y));
            if (playerIndex >= 0)
            {
                playerOrder[playerIndex] = new Point(nx, ny);
            }
            state[x, y] = state[x, y] & ~here.id;
            state[nx, ny] = state[nx, ny] | here.id;

            return true;
        }

        public IEnumerable<LevelState> GetPossibleNextStates()
        {
            LevelState copy = Clone();
            int[] dxs = { 1, -1, 0, 0 };
            int[] dys = { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                if (copy.Move(dxs[i], dys[i]))
                {
                    yield return copy;
                    copy = Clone();
                }
            }
            if (copy.Pass()) yield return copy;
            // Copy if continuing
        }

        public int HeuristicCost()
        {
            List<Point> boxes = FindActors(Actor.Box);
            List<Point> players = FindActors(Actor.Player);
            List<Point> switches = FindActors(Actor.Switch);
            
            int cost = 0;
            foreach (Point box in boxes)
            {
                int value = state[box.X, box.Y];
                if (Actor.Switch.IsAt(value)) continue;
                cost += FindClosestActor(box, players, false);
            }
            foreach (Point @switch in switches)
            {
                cost += FindClosestActor(@switch, boxes, true);
            }
            return cost;
        }

        private int FindClosestActor(Point from, List<Point> actors, bool remove)
        {
            if (actors.Count == 0) return int.MaxValue;
            List<int> dists = actors.Select(box => Distance(box, from)).ToList();
            int index = dists.IndexOf(dists.Min());
            if (remove) actors.RemoveAt(index);
            return dists[index];
        }

        static int Distance(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public List<Point> FindActors(Actor actor)
        {
            List<Point> actors = new List<Point>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (actor.IsAt(state[i, j])) actors.Add(new Point(i, j));
                }
            }
            return actors;
        }

        public int CountActor(Actor actor)
        {
            return FindActors(actor).Count;
        }

        public LevelState Clone()
        {
            return new LevelState(state.Clone() as int[,], playerOrder.Clone() as Point[]);
        }

        public static LevelState FromString(string level)
        {
            level = level.Replace("\r", "");
            if (level.StartsWith("\n")) level = level.Substring(1);
            string[] lines = level.Split('\n');
            int width = lines[0].Length;
            int height = lines.Length;
            int[,] state = new int[width, height];
            List<Point> playerOrder = new List<Point>();
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    char icon = lines[i][j];
                    if (icon <= '9' && icon >= '0')
                    {
                        int player = int.Parse("" + icon);
                        while (playerOrder.Count < player) playerOrder.Add(new Point());
                        playerOrder[player - 1] = new Point(j, i);
                        state[j, i] = Actor.Player.id;
                        continue;
                    }
                    state[j, i] = Actor.ActorIDsForIcon(icon);
                }
            }
            return new LevelState(state, playerOrder.ToArray());
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int order = Array.IndexOf(playerOrder, new Point(j, i));
                    if (order >= 0)
                    {
                        s += (order + 1);
                        continue;
                    }
                    int id = state[j, i];
                    char icon = Actor.ActorIconForIDs(id);
                    s += icon;
                }
                s += "\n";
            }
            return s;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    hash = hash * 31 + state[i, j];
                }
            }
            foreach (Point p in playerOrder)
            {
                hash = hash * 31 + p.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object obj)
        {
            LevelState other = obj as LevelState;
            if (other == null || other.width != width || other.height != height) return false;
            if (!Enumerable.SequenceEqual(playerOrder, other.playerOrder)) return false;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (other.state[i, j] != state[i, j]) return false;
                }
            }
            return true;
        }
    }
}
