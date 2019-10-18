using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    public class Solver
    {
        public class Node : IComparable<Node>
        {
            public readonly Node Parent;
            public readonly LevelState State;
            public readonly int EstimatedDistance;
            public readonly int Cost;
            public int F
            {
                get
                {
                    if (EstimatedDistance != -1 && Cost != -1)
                        return EstimatedDistance + Cost;
                    else
                        return -1;
                }
            }

            public Node(Node parent, LevelState state)
            {
                Parent = parent;
                State = state;
                EstimatedDistance = state.HeuristicCost();
                Cost = parent == null ? 0 : 1 + parent.Cost;
            }

            public int CompareTo(Node obj)
            {
                return F.CompareTo(obj.F);
            }
        }

        public struct Solution
        {
            public int Explored;
            public int Seen;
            public int MaxUnderestimation;
            public List<LevelState> Path;
            
            public bool Solved { get { return Path != null; } }

            public override string ToString()
            {
                string s = String.Format("Explored {0} nodes ({1} unique)\nMax Underestimation: {2}\n", Explored, Seen, MaxUnderestimation);
                if (Path == null) return s + "Unsolved";
                int i = 0;
                foreach (LevelState state in Path)
                {
                    s += (i++) + "\n" + state + "\n";
                }
                return s;
            }
        }

        public static Solution FindPath(LevelState start, int maxExplored = 4000)
        {
            Solution solution = new Solution();
            List<Node> OpenList = new List<Node>();
            HashSet<LevelState> Seen = new HashSet<LevelState>();
            Node current = new Node(null, start);

            // add start LevelState to Open List
            OpenList.Add(current);
            Seen.Add(current.State);

            bool done = false;

            while (OpenList.Count != 0)
            {
                current = OpenList[0];
                OpenList.Remove(current);
                IEnumerable<LevelState> adjacencies = current.State.GetPossibleNextStates();

                //Console.WriteLine(current.Cost);
                //Console.WriteLine(current.DistanceToTarget);
                //Console.WriteLine(current.State);
                //Console.WriteLine("-------------");

                foreach (LevelState state in adjacencies)
                {
                    solution.Explored++;
                    if (!Seen.Contains(state))
                    {
                        Node n = new Node(current, state);
                        Seen.Add(state);
                        OpenList.Insert(0, n);
                        if (n.EstimatedDistance == 0)
                        {
                            done = true;
                            break;
                        }
                    }
                }
                if (done || solution.Explored >= maxExplored) break;
                OpenList.Sort();
            }

            solution.Seen = Seen.Count;

            // construct path, if end was not closed return null
            if (!done) return solution;
            
            solution.Path = new List<LevelState>();

            // if all good, return path
            Node temp = OpenList[0];
            while (temp != null)
            {
                solution.MaxUnderestimation = Math.Max(solution.MaxUnderestimation, 
                    temp.EstimatedDistance - solution.Path.Count);
                solution.Path.Insert(0, temp.State);
                temp = temp.Parent;
            }


            return solution;
        }
    }
}
