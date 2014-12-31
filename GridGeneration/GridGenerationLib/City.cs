using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace GridGenerationLib
{
    public class CityGrid
    {
        public const double CellSize = 800;
        public const int InitialRCellCount = 5;
        public const double UpgradeProbability = 0.05;
        public const double MigrationProbability = 0.3;
        public const double RationalChoiceProbability = 0.5;

        public int ColCount;
        public int RowCount;
        public CityCell[,] Cells;
        public List<Person> Persons;

        private NodeGrid _AStarGrid;
        private Random _rand = new Random();

        public CityGrid(int nCol, int nRow)
        {
            ColCount = nCol;
            RowCount = nRow;
            Cells = new CityCell[nCol, nRow];
            Persons = new List<Person>();
            _AStarGrid = new NodeGrid(nCol, nRow);
        }

        public void InitializeCells()
        {
            for (int i = 0; i < ColCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    Cells[i, j] = new CityCell(i, j);
                }
            }
            Random rand = new Random();
            for (int i = 0; i < InitialRCellCount; i++)
            {
                int x = rand.Next(0, ColCount);
                int y = rand.Next(0, RowCount);
                CityCell cell = Cells[x, y];
                this.MakeResidential(cell);
                this.Accommodate(10, cell);
            }
        }

        public void MakeResidential(CityCell cell)
        {
            cell.Type = CellType.Residential;
            cell.RCapacity = 10;
            cell.JCapacity = 10;
            cell.Score = 1;
        }

        public void Accommodate(int n, CityCell cell)
        {
            cell.RCount += n;
            for (int i = 0; i < n; i++)
            {
                Persons.Add(new Person(cell));
            }
        }

        public void MovePerson(Person p, CityCell to)
        {
            CityCell from = p.Current;
            from.RCount--;
            to.RCount++;
            p.Current = to;

            AStar astar = new AStar(_AStarGrid, from.Col, from.Row, to.Col, to.Row);
            astar.search();
            foreach (var step in astar.path)
            {
                Cells[step.x, step.y].Score++;
            }
            //var pathSides = astar.getPathSides();
            //foreach (var node in pathSides)
            //{
            //    Cells[node.x, node.y].Score++;
            //}
        }

        // A whole day
        public void Update()
        {
            // Go out
            foreach (var p in Persons)
            {
                CityCell destination = ChooseDestination(p);
                MovePerson(p, destination);
            }
            // Go home
            foreach (var p in Persons)
            {
                MovePerson(p, p.Home);
            }
            // Update cells
            foreach (var cell in Cells)
            {
                //cell.Type = GetTypeByScore(cell.Score);
                UpgradeCell(cell);
            }
        }

        public void UpgradeCell(CityCell cell)
        {
            CellType realType = cell.Type;
            CellType qualifiedType = GetTypeByScore(cell.Score);
            if (qualifiedType != realType)
            {
                if (_rand.NextDouble() < UpgradeProbability)
                {
                    cell.Type = qualifiedType;
                    cell.JCapacity = qualifiedType == CellType.Hotel ? 20 : qualifiedType == CellType.Commercial ? 30 : qualifiedType == CellType.Industrial ? 40 : 10;
                    if (qualifiedType == CellType.Residential)
                    {
                        if (_rand.NextDouble() < MigrationProbability)
                        {
                            TearDownOneRCellAndMovePeopleTo(cell);
                        }
                    }
                }
            }
        }

        public void TearDownOneRCellAndMovePeopleTo(CityCell newCell)
        {
            var rCells = Cells.Cast<CityCell>().Where(x => x.Type == CellType.Residential).ToList();
            int maxDist = rCells.Max(x => GetDistance(x, newCell));
            var cellToTearDown = rCells.First(x => GetDistance(x, newCell) == maxDist);
            cellToTearDown.Score = 0;
            cellToTearDown.Type = CellType.Default;
            foreach (var person in Persons)
            {
                if (person.Home == cellToTearDown)
                {
                    person.Home = newCell;
                }
            }
        }

        public void Run(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Update();
            }
        }

        public CityCell ChooseDestination(Person p)
        {
            var destinations = GetDestinations().Where(d => d != p.Home).ToList();
            if (_rand.NextDouble() < RationalChoiceProbability)
            {
                double maxAttraction = destinations.Max(x => (double)x.JCapacity / GetDistance(x, p.Home));
                return destinations.First(x => (double)x.JCapacity / GetDistance(x, p.Home) == maxAttraction);
            }
            else
            {
                int i = _rand.Next(0, destinations.Count);
                return destinations[i];
            }
        }

        public List<CityCell> GetDestinations()
        {
            return Cells.Cast<CityCell>().Where(x => x.Type == CellType.Residential || x.Type == CellType.Hotel || x.Type == CellType.Commercial || x.Type == CellType.Industrial).ToList();
        }

        public int GetDistance(CityCell a, CityCell b)
        {
            return Math.Abs(a.Col - b.Col) + Math.Abs(a.Row - b.Row);
        }

        public static Color GetColorOfType(CellType type)
        {
            Dictionary<CellType, Color> dict = new Dictionary<CellType, Color>
            {
                { CellType.Default, Colors.LightGreen },
                { CellType.Residential, Colors.Yellow },
                { CellType.Hotel, Colors.Orange },
                { CellType.Commercial, Colors.Red },
                { CellType.Industrial, Colors.Purple },
                { CellType.Green, Colors.LightGreen },
                { CellType.Square, Colors.LightGray },
                { CellType.Road, Colors.Silver },
                { CellType.BusStation, Colors.DeepPink },
                { CellType.Solar, Colors.LightBlue }
            };
            return dict[type];
        }

        public static CellType GetTypeByScore(int score)
        {
            if (score == 0)
            {
                return CellType.Default;
            }
            else if (score > 0)
            {
                if (score < 100)
                {
                    return CellType.Residential;
                }
                else if (score < 200)
                {
                    return CellType.Hotel;
                }
                else if (score < 300)
                {
                    return CellType.Commercial;
                }
                else
                {
                    return CellType.Industrial;
                }
            }
            else // score < 0
            {
                if (score > -100)
                {
                    return CellType.Green;
                }
                else if (score > -200)
                {
                    return CellType.Square;
                }
                else if (score > -500)
                {
                    return CellType.Road;
                }
                else
                {
                    return CellType.BusStation;
                }
            }
        }
    }

    public class CityCell
    {
        public CellType Type;
        public int Col;
        public int Row;
        public int RCapacity;
        public int JCapacity;
        public int Score;
        public int RCount;
        public int JCount;

        public CityCell(int col, int row)
        {
            Type = CellType.Default;
            Col = col;
            Row = row;
            RCapacity = 10;
            JCapacity = 10;
            Score = 0;
        }
    }

    public class Person
    {
        public int ID;
        public CityCell Home;
        public CityCell Work;
        public CityCell Current;

        private static int _stamp = 1;

        public Person(CityCell home)
        {
            ID = _stamp;
            _stamp++;
            Home = home;
            Current = home;
        }
    }

    public enum CellType
    {
        Default,
        Residential,
        Hotel,
        Commercial,
        Industrial,
        Green,
        Square,
        Road,
        BusStation,
        Solar
    }
}
