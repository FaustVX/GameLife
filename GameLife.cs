using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;

namespace GameLife
{
	public class GameLife
		//: CSharpHelper.Grid<CSharpHelper.DiagonalCell>
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

		static GameLife()
		{
			colors = new Dictionary<Cell.LiveState, Color>()
				{
					{Cell.LiveState.Emerging,Color.Green},
					{Cell.LiveState.Live, Color.Blue},
					{Cell.LiveState.Dying, new Color(255, 127, 0)},
					{Cell.LiveState.Dead, new Color(200, 200, 200)}
				};

			selectedColors = new Dictionary<Cell.LiveState, Color>()
				{
					{Cell.LiveState.Emerging, Color.Cyan},
					{Cell.LiveState.Live, Color.White},
					{Cell.LiveState.Dying, Color.Red},
					{Cell.LiveState.Dead, Color.Black}
				};
		}

		public GameLife(int width, int height, int fps)
			//: base(width, height)
		{
			_running = false;
			_oneFrame = false;
			_generation = 1;
			_width = width;
			_height = height;
			_fps = fps;
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

		public static Dictionary<Cell.LiveState, Color> SelectedColors
		{
			get { return selectedColors; }
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
			set
			{
				_selectedCell = value;
				SFML.Audio.Listener.Direction = new SFML.Audio.Vector3f(0, 0, -1);
				SFML.Audio.Listener.Position = _selectedCell != null
											? new SFML.Audio.Vector3f(_selectedCell.X, 0, _selectedCell.Y)
											: new SFML.Audio.Vector3f(0, 0, 0);
			}
		}

		public static bool States
		{
			get { return _2States; }
			set
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
							newCells[i, j] = Cell.LiveState.Dead;

					//for (int index = 0; index < Cell.LivingCells.Count; index++)
					//	foreach (Cell cell in Arround(Cell.LivingCells[index]))


					for (int i = 0; i < _width; ++i)
						for (int j = 0; j < _height; ++j)
						{
							Cell cell = _cells[i, j];
							int living = Arround(cell).Count();
							//int i = cell.X;
							//int j = cell.Y;
							switch (cell.State)
							{
								case Cell.LiveState.Emerging:
									if (living == 2 || living == 3)
										newCells[i, j] = Cell.LiveState.Live;
									else
									{
										//Cell.RemoveCell(cell);
										newCells[i, j] = Cell.LiveState.Dying;
									}
									break;
								case Cell.LiveState.Live:
									if (living == 2 || living == 3)
										newCells[i, j] = cell.State;
									else
									{
										//Cell.RemoveCell(cell);
										newCells[i, j] = Cell.LiveState.Dying;
									}
									break;
								case Cell.LiveState.Dying:
									if (living == 3)
									{
										//Cell.AddCell(cell);
										newCells[i, j] = Cell.LiveState.Emerging;
									}
									else
										newCells[i, j] = Cell.LiveState.Dead;
									break;
								case Cell.LiveState.Dead:
									if (living == 3)
									{
										//Cell.AddCell(cell);
										newCells[i, j] = Cell.LiveState.Emerging;
									}
									else
										newCells[i, j] = cell.State;
									break;
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

			//if (x > 0 && (_cells[x - 1, y].State == LiveState.Emerging || _cells[x - 1, y].State == LiveState.Live))
			//	i++;
			//if (x > 0 && y > 0 && (_cells[x - 1, y - 1].State == LiveState.Emerging || _cells[x - 1, y - 1].State == LiveState.Live))
			//	i++;
			//if (y > 0 && (_cells[x, y - 1].State == LiveState.Emerging || _cells[x, y - 1].State == LiveState.Live))
			//	i++;
			//if (x < _width - 1 && y > 0 && (_cells[x + 1, y - 1].State == LiveState.Emerging || _cells[x + 1, y - 1].State == LiveState.Live))
			//	i++;
			//if (x < _width - 1 && (_cells[x + 1, y].State == LiveState.Emerging || _cells[x + 1, y].State == LiveState.Live))
			//	i++;
			//if (x < _width - 1 && y < _height - 1 &&
			//	(_cells[x + 1, y + 1].State == LiveState.Emerging || _cells[x + 1, y + 1].State == LiveState.Live))
			//	i++;
			//if (y < _height - 1 && (_cells[x, y + 1].State == LiveState.Emerging || _cells[x, y + 1].State == LiveState.Live))
			//	i++;
			//if (x > 0 && y < _height - 1 && (_cells[x - 1, y + 1].State == LiveState.Emerging || _cells[x - 1, y + 1].State == LiveState.Live))
			//	i++;
		}

		public void Pause()
		{
			_running = !_running;
		}

		public void Bicolor()
		{
			States = true;
		}
		public void Quadricolor()
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