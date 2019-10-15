﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban
{
    public class Actor
    {
        private static readonly List<Actor> actors = new List<Actor>();
        private static int nextId = 0;

        public readonly char icon;
        public readonly int id;
        public readonly bool passable, pushable;

        public static Actor Empty = new Actor('.', true, false);
        public static Actor Wall = new Actor('#', false, false);
        public static Actor Switch = new Actor('O', true, false);
        public static Actor Box = new Actor('*', false, true);
        public static Actor Player = new Actor('P', false, true);

        private Actor(char icon, bool passable, bool pushable)
        {
            this.icon = icon;
            this.passable = passable;
            this.pushable = pushable;
            id = nextId;
            if (nextId == 0) nextId++;
            else nextId *= 2;
            actors.Add(this);
        }

        public static Actor ActorForId(int id)
        {
            int index = id == 0 ? 0 : (int)(Math.Log(id) / Math.Log(2)) + 1;
            if (index >= actors.Count)
                return null;
            return actors[index];
        }

        public static Actor ActorForIcon(char icon)
        {
            return actors.Where(a => a.icon == icon).First();
        }

        public virtual bool CanMove(int x, int y, int dx, int dy, LevelState state)
        {
            return state.IsInBounds(x + dx, y + dy);
        }
    }

    //public class Wall : Actor
    //{
    //    public override bool CanMove(int x, int y, int dx, int dy, LevelState state)
    //    {
    //        return false;
    //    }
    //}

    //public class Player : Actor
    //{
    //    public override bool CanMove(int x, int y, int dx, int dy, LevelState state)
    //    {
    //        Actor actor = state.GetActor(x + dx, y + dy);
    //        return actor == null || actor.CanMove(x + dx, y + dy, dx, dy, state);
    //    }

    //    public override bool IsPushable()
    //    {
    //        return true;
    //    }
    //}

    //public class Box : Actor
    //{
    //    public override bool CanMove(int x, int y, int dx, int dy, LevelState state)
    //    {
    //        Actor actor = state.GetActor(x + dx, y + dy);
    //        return actor == null;
    //    }

    //    public override bool IsPushable()
    //    {
    //        return true;
    //    }
    //}

    //public class Switch : Actor
    //{
    //    public override bool CanMove(int x, int y, int dx, int dy, LevelState state)
    //    {
    //        return false;
    //    }

    //    public override bool IsPassable()
    //    {
    //        return true;
    //    }
    //}
}