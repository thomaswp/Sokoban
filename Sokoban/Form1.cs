using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sokoban
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Level level;

        private void Form1_Load(object sender, EventArgs e)
        {
            var gen = new RandomGenerator(6, 6, 1, 3);
            //level = new Level(gen.Generate());
            level = new Level(gen.GenerateBest(1000));
            label1.Text = level.ToString();
            label2.Text = "0";

            level.Solve();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'z')
            {
                level.Undo();
            }
            else if (e.KeyChar == 'p')
            {
                level.Pass();
            }
            {
                int dx = 0, dy = 0;
                switch (e.KeyChar)
                {
                    case 'w': case (char)Keys.Up: dy--; break;
                    case 'a': case (char)Keys.Left: dx--; break;
                    case 's': case (char)Keys.Down: dy++; break;
                    case 'd': case (char)Keys.Right: dx++; break;
                }
                level.Move(dx, dy);
            }
            
            label1.Text = level.ToString();
            label2.Text = "" + level.Moves;
            Console.WriteLine(level.CurrentState.HeuristicCost());
        }
    }
}
