using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace GameLife
{
	public class GameLife
	{
		private static readonly Dictionary<Cell.LiveState, Color> colors, selectedColors;
		private readonly int _width, _height;
		private readonly int _fps;
		private readonly Cell[,] _cells;
		private bool _running, _oneFrame;
		private static bool _2States;
		private int _generation;
		private Cell _selectedCell;
		private int _frame;
		private readonly int[] _live, _survive;

		static GameLife()
		{
			Config config = Config.Configuration;

			colors = config.Colors;
			selectedColors = config.SelectedColors;
		}

		public GameLife(int fps)
		{
			Config config = Config.Configuration;

			_running = false;
			_oneFrame = false;
			_generation = 1;
			_width = config.GridWidth;
			_height = config.GridHeight;
			_fps = fps / config.FPS;
			_live = config.Live;
			_survive = config.Survive;
			_frame = -1;

			_cells = new Cell[Width,Height];

			for (int i = 0; i < Width; i++)
				for (int j = 0; j < Height; j++)
					_cells[i, j] = new Cell(i, j, this);
		}

		public static Dictionary<Cell.LiveState, Color> Colors
		{
			get { return colors; }
		}

		public Cell this[int x, int y]
		{
			get
			{
				if (x < 0 || y < 0 || x >= _width || y >= _height)
					return null;
				return _cells[x, y];
			}
		}

		public int Width
		{
			get { return _width; }
		}

		public int Height
		{
			get { return _height; }
		}

		public bool Running
		{
			get { return _running || _oneFrame; }
			set { _running = value; }
		}

		public int Generation
		{
			get { return _generation; }
		}

		public Cell SelectedCell
		{
			get { return _selectedCell; }
			set { _selectedCell = value; }
		}

		public static bool States
		{
			get { return _2States; }
			private set
			{
				_2States = value;
				if (_2States)
				{
					colors[Cell.LiveState.Emerging] = colors[Cell.LiveState.Live];
					colors[Cell.LiveState.Dying] = colors[Cell.LiveState.Dead];
				}
				else
				{
					colors[Cell.LiveState.Emerging] = Color.Green;
					colors[Cell.LiveState.Dying] = new Color(255, 127, 0);
				}
			}
		}

		public int FPS
		{
			get { return _fps; }
		}

		public void OneFrame()
		{
			if (!_running)
				_frame = -1;
			_oneFrame = true;
		}

		public IEnumerable<Drawable> Draw(int offset)
		{
			_frame++;
			Cell.LiveState[,] newCells = null;
			if(_frame%_fps==0)
				if (_running || _oneFrame)
				{
					newCells = new Cell.LiveState[_width,_height];

					for (int i = 0; i < _width; ++i)
						for (int j = 0; j < _height; ++j)
						{
							Cell cell = _cells[i, j];
							int living = Arround(cell).Count();
							switch (cell.State)
							{
								case Cell.LiveState.Live:
								case Cell.LiveState.Emerging:
									{
										bool find = _survive.Any(val => val == living);

										if (find)
											newCells[i, j] = Cell.LiveState.Live;
										else
											newCells[i, j] = Cell.LiveState.Dying;
										break;
									}
								case Cell.LiveState.Dead:
								case Cell.LiveState.Dying:
									{
										bool find = _live.Any(val => val == living);
										if (find)
											newCells[i, j] = Cell.LiveState.Emerging;
										else
											newCells[i, j] = Cell.LiveState.Dead;
										break;
									}
							}
						}
					_generation++;
				}

			for (int x = 0; x < _width; ++x)
				for (int y = 0; y < _height; ++y)
				{
					Cell cell = _cells[x, y];
					
					cell.Shape.FillColor = (cell == _selectedCell) ? selectedColors[cell.State] : colors[cell.State];
					if (_frame % _fps == 0)
					{
						_frame = 0;
						if (_running || _oneFrame)
							cell.State = newCells[x, y];
						cell.Shape.Position = new Vector2f(offset + x * Cell.Width, y * Cell.Height);
					}
					yield return cell.Shape;
				}
			if (_oneFrame)
				_oneFrame = false;
		}

		private static IEnumerable<Cell> Arround(Cell cell)
		{
			if (cell.Up != null && (cell.Up.State == Cell.LiveState.Emerging || cell.Up.State == Cell.LiveState.Live))
				yield return cell.Up;
			if (cell.Down != null && (cell.Down.State == Cell.LiveState.Emerging || cell.Down.State == Cell.LiveState.Live))
				yield return cell.Down;
			if (cell.Left != null && (cell.Left.State == Cell.LiveState.Emerging || cell.Left.State == Cell.LiveState.Live))
				yield return cell.Left;
			if (cell.Right != null && (cell.Right.State == Cell.LiveState.Emerging || cell.Right.State == Cell.LiveState.Live))
				yield return cell.Right;
			if (cell.UpLeft != null && (cell.UpLeft.State == Cell.LiveState.Emerging || cell.UpLeft.State == Cell.LiveState.Live))
				yield return cell.UpLeft;
			if (cell.UpRight != null &&
				(cell.UpRight.State == Cell.LiveState.Emerging || cell.UpRight.State == Cell.LiveState.Live))
				yield return cell.UpRight;
			if (cell.DownLeft != null &&
				(cell.DownLeft.State == Cell.LiveState.Emerging || cell.DownLeft.State == Cell.LiveState.Live))
				yield return cell.DownLeft;
			if (cell.DownRight != null &&
				(cell.DownRight.State == Cell.LiveState.Emerging || cell.DownRight.State == Cell.LiveState.Live))
				yield return cell.DownRight;
		}

		public void Pause()
		{
			_running = !_running;
		}

		public static void Bicolor()
		{
			States = true;
		}
		public static void Quadricolor()
		{
			States = false;
		}

		public void Reset()
		{
			foreach (Cell cell in _cells)
				cell.State = (cell.State != Cell.LiveState.Dead) ? Cell.LiveState.Dying : Cell.LiveState.Dead;

			_running = false;
			_generation = 0;
			_oneFrame = true;
		}
	}
}