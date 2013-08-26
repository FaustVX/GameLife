using System.Collections.Generic;
using SFML.Graphics;

namespace SFML
{
	public class GameLife
		//: CSharpHelper.Grid<CSharpHelper.DiagonalCell>
	{
		private static readonly Dictionary<LiveState, Color> colors, selectedColors;
		private readonly int _width, _height;
		private readonly Cell[,] _cells;
		private bool _running;
		private int _generation;
		private Cell _selectedCell;


		static GameLife()
		{
			colors = new Dictionary<LiveState, Color>()
				{
					{LiveState.Emerging, Color.Green},
					{LiveState.Live, Color.Blue},
					{LiveState.Dying, new Color(255, 127, 0)},
					{LiveState.Dead, new Color(200, 200, 200)}
				};

			selectedColors = new Dictionary<LiveState, Color>()
				{
					{LiveState.Emerging, Color.Cyan},
					{LiveState.Live, Color.White},
					{LiveState.Dying, Color.Red},
					{LiveState.Dead, Color.Black}
				};
		}

		public GameLife(int width, int height)
			//: base(width, height)
		{
			_running = false;
			_generation = 1;
			_width = width;
			_height = height;

			_cells = new Cell[Width,Height];

			for (int i = 0; i < Width; i++)
				for (int j = 0; j < Height; j++)
					_cells[i, j] = new Cell(i, j, this);
		}

		public static Dictionary<LiveState, Color> Colors
		{
			get { return colors; }
		}

		public static Dictionary<LiveState, Color> SelectedColors
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
			get { return _running; }
			set { _running = value; }
		}

		public int Iteration
		{
			get { return _generation; }
		}

		public Cell SelectedCell
		{
			get { return _selectedCell; }
			set { _selectedCell = value; }
		}

		private IEnumerable<Drawable> Draw()
		{
			LiveState[,] newCells = new LiveState[_width, _height];

			for (int i = 0; i < _width; ++i)
			{
				for (int j = 0; j < _height; ++j)
				{
					Cell cell = _cells[i, j];
					int living = Arround(cell);
					switch (cell.State)
					{
						case LiveState.Emerging:
							if (living == 2 || living == 3)
								newCells[i, j] = LiveState.Live;
							else
								newCells[i, j] = LiveState.Dying;
							break;
						case LiveState.Live:
							if (living == 2 || living == 3)
								newCells[i, j] = cell.State;
							else
								newCells[i, j] = LiveState.Dying;
							break;
						case LiveState.Dying:
							if (living == 3)
								newCells[i, j] = LiveState.Emerging;
							else
								newCells[i, j] = LiveState.Dead;
							break;
						case LiveState.Dead:
							if (living == 3)
								newCells[i, j] = LiveState.Emerging;
							else
								newCells[i, j] = cell.State;
							break;
					}
				}
			}

			for (int x = 0; x < _width; ++x)
				for (int y = 0; y < _height; ++y)
				{
					_cells[x, y].State = newCells[x, y];
					yield return _cells[x, y].Shape;
				}
			_generation++;
		}

		private static int Arround(Cell cell)
		{
			int i = 0;

			if (cell.Up != null && (cell.Up.State == LiveState.Emerging || cell.Up.State == LiveState.Live))
				i++;
			if (cell.Down != null && (cell.Down.State == LiveState.Emerging || cell.Down.State == LiveState.Live))
				i++;
			if (cell.Left != null && (cell.Left.State == LiveState.Emerging || cell.Left.State == LiveState.Live))
				i++;
			if (cell.Right != null && (cell.Right.State == LiveState.Emerging || cell.Right.State == LiveState.Live))
				i++;
			if (cell.UpLeft != null && (cell.UpLeft.State == LiveState.Emerging || cell.UpLeft.State == LiveState.Live))
				i++;
			if (cell.UpRight != null && (cell.UpRight.State == LiveState.Emerging || cell.UpRight.State == LiveState.Live))
				i++;
			if (cell.DownLeft != null && (cell.DownLeft.State == LiveState.Emerging || cell.DownLeft.State == LiveState.Live))
				i++;
			if (cell.DownRight != null && (cell.DownRight.State == LiveState.Emerging || cell.DownRight.State == LiveState.Live))
				i++;

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

			return i;
		}
	}
}