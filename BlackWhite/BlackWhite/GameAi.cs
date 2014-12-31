using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackWhite
{
    public class GameAi
    {
        private Board _board;
        private Piece _color;

        public GameAi(Board board, Piece color)
        {
            _board = board;
            _color = color;
        }

        public Tuple<int, int> Step()
        {
            int maxChanged = -1;
            Tuple<int, int> maxChangedPos = null;

            for (int i = 0; i < _board.Size; i++)
            {
                for (int j = 0; j < _board.Size; j++)
                {
                    if (_board[i, j] == Piece.Empty)
                    {
                        var pos = Tuple.Create(i, j);
                        int changed = _board.StepVirtual(_color, Tuple.Create(i, j));
                        if (changed > maxChanged)
                        {
                            maxChanged = changed;
                            maxChangedPos = pos;
                        }
                    }
                }
            }

            if (maxChanged == 1)
            {
                Random rd = new Random();
                int i, j;
                while (true)
                {
                    i = rd.Next(0, _board.Size);
                    j = rd.Next(0, _board.Size);
                    if (_board[i, j] == Piece.Empty)
                    {
                        break;
                    }
                }
                maxChangedPos = Tuple.Create(i, j);
            }

            return maxChangedPos;
        }
    }
}
