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
            level = new Level(Level.Level3);
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
            else
            {
                int dx = 0, dy = 0;
                switch (e.KeyChar)
                {
                    case 'w': dy--; break;
                    case 'a': dx--; break;
                    case 's': dy++; break;
                    case 'd': dx++; break;
                }
                level.Move(dx, dy);
            }
            
            label1.Text = level.ToString();
            label2.Text = "" + level.Moves;
            Console.WriteLine(level.CurrentState.HeuristicCost());
        }
    }
}
