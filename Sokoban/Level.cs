using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    class Level
    {
        public readonly int players;
        public readonly int width, height;
        private readonly LevelState initialState;
        private LevelState currentState;
        private List<LevelState> history = new List<LevelState>();

        public Level(LevelState initialState)
        {
            this.initialState = currentState = initialState;
            width = initialState.width;
            height = initialState.height;
            players = initialState.CountActor(Actor.Player);
            history.Add(currentState.Clone());
            Console.WriteLine(initialState);
        }

        public bool Move(int dx, int dy)
        {
            bool success = currentState.Move(dx, dy);
            if (success)
            {
                currentState.Advance();
                history.Add(currentState.Clone());
            }
            return success;
        }

        public bool Undo()
        {
            if (history.Count <= 1) return false;
            history.RemoveAt(history.Count - 1);
            currentState = history[history.Count - 1].Clone();
            return true;
        }

        public override string ToString()
        {
            return currentState.ToString();
        }

        public static readonly LevelState Level1 = LevelState.FromString(@"
####..
#.O#..
#..###
#O1..#
#..*.#
#2.###
####..");
    }
}
