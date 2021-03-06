﻿using System;
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

        public LevelState CurrentState
        {
            get { return currentState.Clone(); }
        }

        public int Moves
        {
            get { return history.Count - 1; }
        }

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
            return Act(() => currentState.Move(dx, dy));
        }

        public bool Pass()
        {
            return Act(() => currentState.Pass());
        }

        private bool Act(Func<bool> action)
        {
            bool success = action();
            if (success)
            {
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

        public void Solve()
        {
            Solver.Solution solution = Solver.FindPath(currentState);
            Console.WriteLine(solution);
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
#*.*.#
#..###
####..");

        public static readonly LevelState Level2 = LevelState.FromString(@"
######
#....#
#.#1.#
#.*@.#
#.O@.#
#....#
######");

        public static readonly LevelState Level3 = LevelState.FromString(@"
########
###...##
#O1*..##
###.*O##
#O##*.##
#.#.O.##
#*.@**O#
#...O..#
########");


    }
}
