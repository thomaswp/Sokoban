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
            public readonly int DistanceToTarget;
            public readonly int Cost;
            public int F
            {
                get
                {
                    if (DistanceToTarget != -1 && Cost != -1)
                        return DistanceToTarget + Cost;
                    else
                        return -1;
                }
            }

            public Node(Node parent, LevelState state)
            {
                Parent = parent;
                State = state;
                DistanceToTarget = state.HeuristicCost();
                Cost = parent == null ? 0 : 1 + parent.Cost;
            }

            public int CompareTo(Node obj)
            {
                return F.CompareTo(obj.F);
            }
        }

        public static Stack<LevelState> FindPath(LevelState start)
        {
            Stack<LevelState> Path = new Stack<LevelState>();
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

                Console.WriteLine(current.Cost);
                Console.WriteLine(current.DistanceToTarget);
                Console.WriteLine(current.State);
                Console.WriteLine("-------------");

                foreach (LevelState state in adjacencies)
                {
                    if (!Seen.Contains(state))
                    {
                        Node n = new Node(current, state);
                        Seen.Add(state);
                        OpenList.Insert(0, n);
                        if (n.DistanceToTarget == 0)
                        {
                            done = true;
                            break;
                        }
                    }
                }
                if (done) break;
                OpenList.Sort();
            }

            // construct path, if end was not closed return null
            if (!done) return null;

            // if all good, return path
            Node temp = OpenList[0];
            while (temp != null)
            {
                Path.Push(temp.State);
                temp = temp.Parent;
            }
            return Path;
        }
    }
}
