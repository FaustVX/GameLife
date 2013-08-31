using System.Collections.Generic;
using System.Linq;
using CSharpHelper;
using SFML.Graphics;
using SFML.Window;

namespace GameLife
{
	public class GameLife : Grid<LifeCell>
	{
		private static readonly Dictionary<LifeCell.LiveState, Color> colors, selectedColors;
		private readonly int _fps;
		private bool _running, _oneFrame;
		private static bool _2States;
		private int _generation;
		private LifeCell _selectedCell;
		private readonly Vector2i _offset;
		private int _frame;
		private readonly int[] _live, _survive;

		static GameLife()
		{
			Config config = Config.Configuration;

			colors = config.Colors;
			selectedColors = config.SelectedColors;
		}

		public GameLife(int fps, Vector2i offset)
			: base(Config.Configuration.GridWidth, Config.Configuration.GridHeight, (i, j, cells) => new LifeCell(i, j, cells))
		{
			_offset = offset;
			Config config = Config.Configuration;

			_running = false;
			_oneFrame = false;
			_generation = 1;
			_fps = fps / config.FPS;
			_live = config.Live;
			_survive = config.Survive;
			_frame = -1;
		}

		public static Dictionary<LifeCell.LiveState, Color> Colors
		{
			get { return colors; }
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

		public LifeCell SelectedCell
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
					colors[LifeCell.LiveState.Emerging] = colors[LifeCell.LiveState.Live];
					colors[LifeCell.LiveState.Dying] = colors[LifeCell.LiveState.Dead];
				}
				else
				{
					colors[LifeCell.LiveState.Emerging] = Color.Green;
					colors[LifeCell.LiveState.Dying] = new Color(255, 127, 0);
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
			LifeCell.LiveState[,] newCells = null;
			if(_frame%_fps==0)
				if (_running || _oneFrame)
				{
					newCells = new LifeCell.LiveState[Width,Height];

					for (int i = 0; i < Width; ++i)
						for (int j = 0; j < Height; ++j)
						{
							LifeCell cell = this[i, j];
							int living = cell.Neighbor();
							switch (cell.State)
							{
								case LifeCell.LiveState.Live:
								case LifeCell.LiveState.Emerging:
									{
										bool find = _survive.Any(val => val == living);

										if (find)
											newCells[i, j] = LifeCell.LiveState.Live;
										else
											newCells[i, j] = LifeCell.LiveState.Dying;
										break;
									}
								case LifeCell.LiveState.Dead:
								case LifeCell.LiveState.Dying:
									{
										bool find = _live.Any(val => val == living);
										if (find)
											newCells[i, j] = LifeCell.LiveState.Emerging;
										else
											newCells[i, j] = LifeCell.LiveState.Dead;
										break;
									}
							}
						}
					_generation++;
				}

			for (int x = 0; x < Width; ++x)
				for (int y = 0; y < Height; ++y)
				{
					LifeCell cell = this[x, y];
					
					cell.Shape.FillColor = (cell == _selectedCell) ? selectedColors[cell.State] : colors[cell.State];
					if (_frame % _fps == 0)
					{
						_frame = 0;
						if (_running || _oneFrame)
							cell.State = newCells[x, y];
						cell.Shape.Position = new Vector2f(_offset.X + x * LifeCell.Width, _offset.Y + y * LifeCell.Height);
					}
					yield return cell.Shape;
				}
			if (_oneFrame)
				_oneFrame = false;
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
			foreach (LifeCell cell in Cells)
				cell.State = (cell.State != LifeCell.LiveState.Dead) ? LifeCell.LiveState.Dying : LifeCell.LiveState.Dead;

			_running = false;
			_generation = 0;
			_oneFrame = true;
		}
	}
}