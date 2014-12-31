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
    public class DisplayManager
    {
        public PictureBox DisplayBox;
        public Grid GameGrid;
        private int _cellSize;
        public int CellSize { get { return _cellSize; } }

        public DisplayManager(PictureBox display, int rows = 8, int cols = 8)
        {
            DisplayBox = display;
            GameGrid = new Grid(rows, cols);
            GameGrid.SetBorderToWall();
            _cellSize = Math.Min(display.Width / cols, display.Height / rows);
            DisplayBox.Paint += DrawGrid;
        }

        public DisplayManager(PictureBox display, Grid gameGrid)
        {
            DisplayBox = display;
            GameGrid = gameGrid;
            GameGrid.SetBorderToWall();
            _cellSize = Math.Min(display.Width / gameGrid.Columns, display.Height / gameGrid.Rows);
            DisplayBox.Paint += DrawGrid;
        }

        private void DrawGrid(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(SystemColors.Control);
            foreach (Cell gridCell in GameGrid.Cells)
            {
                DrawCell(g, gridCell);
            }
            DrawPorter(g);
            foreach (Box aBox in GameGrid.Boxes)
            {
                DrawBox(g, aBox);
            }
            
            //g.Dispose();
        }

        private void DrawBox(Graphics g, Box aBox)
        {
            Rectangle cell = new Rectangle(aBox.Position.Column * _cellSize, aBox.Position.Row * _cellSize, _cellSize, _cellSize);
            Pen border = new Pen(Color.Red);
            SolidBrush fill;
            if (aBox.IsAtHome)
            {
                fill = new SolidBrush(Color.Green);
            }
            else
            {
                fill = new SolidBrush(Color.Orange);
            }
            g.DrawRectangle(border, cell);
            cell.Inflate(-1, -1);
            g.FillRectangle(fill, cell);
        }

        private void DrawPorter(Graphics g)
        {
            if (GameGrid.ThePorter == null)
            {
                return;
            }
            Rectangle cell = new Rectangle(GameGrid.ThePorter.Position.Column * _cellSize, GameGrid.ThePorter.Position.Row * _cellSize, 
                _cellSize, _cellSize);
            SolidBrush fill = new SolidBrush(Color.Blue);
            g.FillEllipse(fill, cell);
        }

        private void DrawCell(Graphics g, Cell gridCell)
        {
            Rectangle cell = new Rectangle(gridCell.Position.Column * _cellSize, gridCell.Position.Row * _cellSize, _cellSize, _cellSize);
            Pen border = new Pen(Color.Black);
            SolidBrush fill;
            if (gridCell.IsWall)
            {
                fill = new SolidBrush(Color.Black);
            }
            else
            {
                if (gridCell.IsHome)
                {
                    fill = new SolidBrush(Color.Red);
                }
                else
                {
                    fill = new SolidBrush(Color.White);
                }
            }
            g.DrawRectangle(border, cell);
            cell.Inflate(-1, -1);
            g.FillRectangle(fill, cell);
        }

        public Node GetCellNode(int x, int y)
        {
            int row = y / _cellSize;
            int col = x / _cellSize;
            return new Node(row, col);
        }
    }
}