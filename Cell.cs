using SFML.Graphics;
using SFML.Window;

namespace GameLife
{
	public class Cell
	{
		public enum LiveState
		{
			Emerging,
			Live,
			Dying,
			Dead
		}

		private static readonly int width, height;

		private LiveState _state;
		private readonly RectangleShape _shape;
		private readonly int _x, _y;
		private Cell _up, _left, _down, _right;

		static Cell()
		{
			width = Config.Configuration.CellWidth;
			height = Config.Configuration.CellHeight;
		}

		public Cell(int x, int y, GameLife list)
		{
			_x = x;
			_y = y;
			_state = LiveState.Dead;
			_shape = CreateShape();


			if (x != 0)
			{
				_left = list[x - 1, y];
				_left._right = this;
				if (_x == list.Width - 1)
				{
					_right = list[0, y];
					_right._left = this;
				}
			}

			if (y != 0)
			{
				_up = list[x, y - 1];
				_up._down = this;
				if (_y == list.Height - 1)
				{
					_down = list[x, 0];
					_down._up = this;
				}
			}
		}

		public LiveState State
		{
			get { return _state; }
			set { _state = value; }
		}

		public RectangleShape Shape
		{
			get { return _shape; }
		}

		public static int Width
		{
			get { return width; }
		}

		public static int Height
		{
			get { return height; }
		}

		public Cell Up
		{
			get { return _up; }
		}

		public Cell Down
		{
			get { return _down; }
		}

		public Cell Left
		{
			get { return _left; }
		}

		public Cell Right
		{
			get { return _right; }
		}

		public Cell UpLeft
		{
			get { return Up != null ? Up.Left : null; }
		}

		public Cell UpRight
		{
			get { return Up != null ? Up.Right : null; }
		}

		public Cell DownLeft
		{
			get { return Down != null ? Down.Left : null; }
		}

		public Cell DownRight
		{
			get { return Down != null ? Down.Right : null; }
		}

		public int X
		{
			get { return _x; }
		}

		public int Y
		{
			get { return _y; }
		}

		private static RectangleShape CreateShape()
		{
			return new RectangleShape(new Vector2f(width, height))
				{
					FillColor = GameLife.Colors[LiveState.Dead],
					OutlineColor = Color.Cyan
				};
		}

		public void Click()
		{
			switch (State)
			{
				case LiveState.Emerging:
				case LiveState.Live:
					State = LiveState.Dying;
					break;
				case LiveState.Dying:
				case LiveState.Dead:
					State = LiveState.Emerging;
					break;
			}
		}
	}
}