using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace GameLife
{
	public class GameLife
	{
		private readonly int _tasksX;
		private readonly int _tasksY;
		private readonly Vector2i _offset;
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
		//private readonly Task<List<Drawable>>[,] _tasks;
		private readonly bool[,] _launch;
		//private readonly List<Drawable>[,] _lists;

		static GameLife()
		{
			Config config = Config.Configuration;

			colors = config.Colors;
			selectedColors = config.SelectedColors;
		}

		public GameLife(int fps, int tasksX, int tasksY, Vector2i offset)
		{
			_tasksX = tasksX;
			_tasksY = tasksY;
			_offset = offset;
			Config config = Config.Configuration;

			_running = false;
			_oneFrame = false;
			_generation = 1;
			_width = config.GridWidth;
			_height = config.GridHeight;
			_fps = fps / config.FPS;
			_live = config.Live;
			_survive = config.Survive;
			//_tasks = new Task<List<Drawable>>[tasksX, tasksY];
			_launch = new bool[tasksX,tasksY];
			//_lists = new List<Drawable>[tasksX,tasksY];
			_frame = -1;

			_cells = new Cell[Width,Height];

			int x = _width / _tasksX;
			int y = _height / _tasksY;
			for (int i = 0; i < tasksX; ++i)
			{
				for (int j = 0; j < tasksY; ++j)
				{
					_launch[i, j] = false;
					//_lists[i, j] = new List<Drawable>();

					//RectangleShape rect = new RectangleShape()
					//{
					//	Position = new Vector2f(i * x, j * y),
					//	Size = new Vector2f(x, y),
					//	Origin = new Vector2f(i, j)
					//};

					//_tasks[i, j] = new Task<List<Drawable>>(FirstCompute, rect);
					//_tasks[i, j].Start();
				}
			}

			for (int i = 0; i < Width; i++)
				for (int j = 0; j < Height; j++)
					_cells[i, j] = new Cell(i, j, this);
		}

		private Cell.LiveState[,] FirstCompute(object obj)
		{
			RectangleShape rect = (RectangleShape)obj;
			int startX = (int)rect.Position.X;
			int startY = (int)rect.Position.Y;
			int width = (int)rect.Size.X;
			int height = (int)rect.Size.Y;
			int posX = (int)rect.Origin.X;
			int posY = (int)rect.Origin.Y;

			//while (true)
			//{
			//if (!_launch[posX, posY])
			//	continue;
			Cell.LiveState[,] newCells = new Cell.LiveState[width, height];
			if (_frame % _fps == 0)
				if (_running || _oneFrame)
					for (int i = 0; i < width; ++i)
						for (int j = 0; j < height; ++j)
						{
							Cell cell = _cells[i + startX, j + startY];
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
			//else
			//	for (int i = 0; i < width; ++i)
			//		for (int j = 0; j < height; ++j)
			//			newCells[i, j] = _cells[i + startX, j + startY].State;
			return newCells;

			//_launch[posX, posY] = true;
			//bool finish;
			//do
			//{
			//	finish = true;
			//	for (int i = 0; i < _tasksX; ++i)
			//		for (int j = 0; j < _tasksY; ++j)
			//			if (!_launch[i, j])
			//				finish = false;
			//} while (!finish);

			////lock (_lists[posX, posY])
			////	_lists[posX, posY].Clear();
			//List<Drawable> result = new List<Drawable>(width * height);

			//for (int x = 0; x < width; ++x)
			//	for (int y = 0; y < height; ++y)
			//	{
			//		Cell cell = _cells[startX + x, startY + y];

			//		cell.Shape.FillColor = (cell == _selectedCell) ? selectedColors[cell.State] : colors[cell.State];
			//		if (_frame % _fps == 0)
			//		{
			//			if (_running || _oneFrame)
			//				cell.State = newCells[x, y];
			//			cell.Shape.Position = new Vector2f(_offset.X + (startX + x) * Cell.Width, _offset.Y + (startY + y) * Cell.Height);
			//		}
			//		//lock (_lists[posX, posY])
			//		result.Add(cell.Shape);
			//	}

			////_launch[posX, posY] = false;
			//return result;
			//}
		}

		private List<Drawable> AfterCompute(dynamic obj)
		{
			dynamic o = obj.Result;
			// new { Cells = r.Result, X = i1, Y = j1, Width = x, Height = y };

			Cell.LiveState[,] newCells = o.Cells;
			int posX = o.X;
			int posY = o.Y;
			int width = o.Width;
			int height = o.Height;
			int startX = posX * width;
			int startY = posY * height;

			//Cell.LiveState[,] newCells = obj.Result;
			//RectangleShape rect = (RectangleShape)obj;
			//int startX = (int)rect.Position.X;
			//int startY = (int)rect.Position.Y;
			//int width = (int)rect.Size.X;
			//int height = (int)rect.Size.Y;
			//int posX = (int)rect.Origin.X;
			//int posY = (int)rect.Origin.Y;

			//while (true)
			//{
			//if (!_launch[posX, posY])
			//	continue;
			//Cell.LiveState[,] newCells = null;
			//if (_frame % _fps == 0)
			//	if (_running || _oneFrame)
			//	{
			//		newCells = new Cell.LiveState[width, height];

			//		for (int i = 0; i < width; ++i)
			//			for (int j = 0; j < height; ++j)
			//			{
			//				Cell cell = _cells[i + startX, j + startY];
			//				int living = Arround(cell).Count();
			//				switch (cell.State)
			//				{
			//					case Cell.LiveState.Live:
			//					case Cell.LiveState.Emerging:
			//						{
			//							bool find = _survive.Any(val => val == living);

			//							if (find)
			//								newCells[i, j] = Cell.LiveState.Live;
			//							else
			//								newCells[i, j] = Cell.LiveState.Dying;
			//							break;
			//						}
			//					case Cell.LiveState.Dead:
			//					case Cell.LiveState.Dying:
			//						{
			//							bool find = _live.Any(val => val == living);
			//							if (find)
			//								newCells[i, j] = Cell.LiveState.Emerging;
			//							else
			//								newCells[i, j] = Cell.LiveState.Dead;
			//							break;
			//						}
			//				}
			//			}
			//	}

			//_launch[posX, posY] = true;
			//bool finish;
			//do
			//{
			//	finish = true;
			//	for (int i = 0; i < _tasksX; ++i)
			//		for (int j = 0; j < _tasksY; ++j)
			//			if (!_launch[i, j])
			//				finish = false;
			//} while (!finish);

			//lock (_lists[posX, posY])
			//	_lists[posX, posY].Clear();
			List<Drawable> result = new List<Drawable>(width * height);

			for (int x = 0; x < width; ++x)
				for (int y = 0; y < height; ++y)
				{
					Cell cell = _cells[startX + x, startY + y];

					cell.Shape.FillColor = (cell == _selectedCell) ? selectedColors[cell.State] : colors[cell.State];
					if (_frame % _fps == 0)
					{
						if (_running || _oneFrame)
							cell.State = newCells[x, y];
						cell.Shape.Position = new Vector2f(_offset.X + (startX + x) * Cell.Width, _offset.Y + (startY + y) * Cell.Height);
					}
					//lock (_lists[posX, posY])
					result.Add(cell.Shape);
				}

			_launch[posX, posY] = false;
			return result;
			//}
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

		public IEnumerable<Drawable> Draw()
		{
			_frame++;
			int x = _width / _tasksX;
			int y = _height / _tasksY;

			List<Drawable> result = new List<Drawable>(_width * _height);
			bool[,] finished = new bool[_tasksX,_tasksY];

			for (int i = 0; i < _tasksX; ++i)
				for (int j = 0; j < _tasksY; ++j)
				{
					_launch[i, j] = false;
					finished[i, j] = false;

					RectangleShape rect = new RectangleShape()
						{
							Position = new Vector2f(i * x, j * y),
							Size = new Vector2f(x, y),
							Origin = new Vector2f(i, j)
						};

					var task = new Task<Cell.LiveState[,]>(FirstCompute, rect);
					int i1 = i;
					int j1 = j;
					task.ContinueWith(r =>
						{
							_launch[i1, j1] = true;
							bool p;
							do
							{
								p = true;
								for (int u = 0; u < _tasksX; ++u)
									for (int v = 0; v < _tasksY; ++v)
										if (!_launch[u, v])
											p = false;
							} while (!p);
							return new {Cells = r.Result, X = i1, Y = j1, Width = x, Height = y};
						}, TaskContinuationOptions.ExecuteSynchronously)
						.ContinueWith<List<Drawable>>(AfterCompute).ContinueWith(r =>
						{
							lock (result)
								result.AddRange(r.Result);
							//foreach (Drawable drawable in r.Result)
							//	result.Add(drawable);
							finished[i1, j1] = true;
						}, TaskContinuationOptions.ExecuteSynchronously);
					task.Start();
				} //Cell.LiveState[,] newCells = null;


			if (_frame % _fps == 0)
				if (_running || _oneFrame)
				{
					//newCells = new Cell.LiveState[_width,_height];

					//for (int i = 0; i < _width; ++i)
					//	for (int j = 0; j < _height; ++j)
					//	{
					//		Cell cell = _cells[i, j];
					//		int living = Arround(cell).Count();
					//		switch (cell.State)
					//		{
					//			case Cell.LiveState.Live:
					//			case Cell.LiveState.Emerging:
					//				{
					//					bool find = _survive.Any(val => val == living);

					//					if (find)
					//						newCells[i, j] = Cell.LiveState.Live;
					//					else
					//						newCells[i, j] = Cell.LiveState.Dying;
					//					break;
					//				}
					//			case Cell.LiveState.Dead:
					//			case Cell.LiveState.Dying:
					//				{
					//					bool find = _live.Any(val => val == living);
					//					if (find)
					//						newCells[i, j] = Cell.LiveState.Emerging;
					//					else
					//						newCells[i, j] = Cell.LiveState.Dead;
					//					break;
					//				}
					//		}
					//	}
					_generation++;
				}

			//for (int x = 0; x < _width; ++x)
			//	for (int y = 0; y < _height; ++y)
			//	{
			//		Cell cell = _cells[x, y];

			//		cell.Shape.FillColor = (cell == _selectedCell) ? selectedColors[cell.State] : colors[cell.State];
			//		if (_frame % _fps == 0)
			//		{
			//			_frame = 0;
			//			if (_running || _oneFrame)
			//				cell.State = newCells[x, y];
			//			cell.Shape.Position = new Vector2f(_offset.X + x * Cell.Width, _offset.Y + y * Cell.Height);
			//		}
			//		yield return cell.Shape;
			//	}
			//for (int i = 0; i < _tasksX; ++i)
			//	for (int j = 0; j < _tasksY; ++j)
			//		_tasks[i, j].Start();

			bool finish;
			do
			{
				finish = true;
				for (int i = 0; i < _tasksX; ++i)
					for (int j = 0; j < _tasksY; ++j)
						if (!finished[i, j])
							finish = false;
				//	{
				//		if (!_tasks[i, j].IsCompleted)
				//			finish = false;
				//		else if(!_launch[i, j])
				//		{
				//			foreach (Drawable drawable in _tasks[i, j].Result)
				//				result.Add(drawable);
				//			_launch[i, j] = true;
				//		}
				//	}
			} while (!finish);

			if (_oneFrame)
				_oneFrame = false;
			//for (int i = 0; i < _tasksX; ++i)
			//	for (int j = 0; j < _tasksY; ++j)
			//		lock (_lists[i, j])
			//			result.AddRange(_lists[i, j]);
			return result;
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