using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Algorithm;

namespace UITest
{
    public partial class Form1 : Form
    {
        private DisplayManager DisplayHost;

        public Form1()
        {
            InitializeComponent();
            DisplayHost = new DisplayManager(pictureBox1);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (cbCreateCell.Checked)
            {
                Node cellPosition = DisplayHost.GetCellNode(e.X, e.Y);
                if (rbWall.Checked)
                {
                    if (!DisplayHost.GameGrid[cellPosition].HasBoxOrPorter)
                    {
                        DisplayHost.GameGrid.SetCell(cellPosition.Row, cellPosition.Column, CellType.Wall);
                    }
                }
                else if (rbSpace.Checked)
                {
                    DisplayHost.GameGrid.SetCell(cellPosition.Row, cellPosition.Column, CellType.Space);
                }
                else if (rbHomeSpace.Checked)
                {
                    DisplayHost.GameGrid.SetCell(cellPosition.Row, cellPosition.Column, CellType.HomeSpace);
                }
                else if (rbPorter.Checked)
                {
                    if (DisplayHost.GameGrid[cellPosition].IsEmpty)
                    {
                        DisplayHost.GameGrid.CreatePorter(cellPosition.Row, cellPosition.Column);
                    }
                }
                else if (rbBox.Checked)
                {
                    if (DisplayHost.GameGrid[cellPosition].IsEmpty)
                    {
                        DisplayHost.GameGrid.CreateBox(cellPosition.Row, cellPosition.Column);
                    }
                }
                else
                {
                    DisplayHost.GameGrid.ClearMovable(cellPosition.Row, cellPosition.Column);
                }
                pictureBox1.Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    DisplayHost.GameGrid.MovePorter(Direction.Up);
                    break;
                case Keys.S:
                    DisplayHost.GameGrid.MovePorter(Direction.Down);
                    break;
                case Keys.A:
                    DisplayHost.GameGrid.MovePorter(Direction.Left);
                    break;
                case Keys.D:
                    DisplayHost.GameGrid.MovePorter(Direction.Right);
                    break;
                default:
                    break;
            }
            pictureBox1.Invalidate();
        }

        private void 退出QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGameDialog ngd = new NewGameDialog();
            if (ngd.ShowDialog() == DialogResult.OK)
            {
                int rows = Convert.ToInt32(ngd.numericUpDown1.Value);
                int cols = Convert.ToInt32(ngd.numericUpDown2.Value);
                DisplayHost = new DisplayManager(pictureBox1, rows, cols);
                pictureBox1.Invalidate();
            }
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DisplayHost.GameGrid.Serialize(saveFileDialog1.FileName);
            }
        }

        private void 选择关卡ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Grid gameGrid = Grid.Deserialize(openFileDialog1.FileName);
                DisplayHost = new DisplayManager(pictureBox1, gameGrid);
                pictureBox1.Invalidate();
            }
        }
    }
}
