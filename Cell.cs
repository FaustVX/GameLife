using SFML.Graphics;
using SFML.Window;

namespace SFML
{
	public enum LiveState
	{
		Emerging,
		Live,
		Dying,
		Dead
	}

	public class Cell
	{
		private static readonly int width, height;

		private LiveState _state;
		private readonly Drawable _shape;
		//private readonly int _x, _y;
		private readonly Cell _up, _left;
		private Cell _down, _right;
		//private readonly GameLife _list;

		static Cell()
		{
			width = 15;
			height = 15;
		}

		public Cell(int x, int y, GameLife list)
		{
			_state = LiveState.Dead;
			_shape = CreateShape();


			if (x != 0)
			{
				_up = list[x - 1, y];
				_up._down = this;
			}

			if (y != 0)
			{
				_left = list[x, y - 1];
				_left._right = this;
			}
		}

		public LiveState State
		{
			get { return _state; }
			set { _state = value; }
		}

		public Drawable Shape
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

		private static Drawable CreateShape()
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
					State = LiveState.Dying;
					break;
				case LiveState.Live:
					State = LiveState.Dying;
					break;
				case LiveState.Dying:
					State = LiveState.Emerging;
					break;
				case LiveState.Dead:
					State = LiveState.Emerging;
					break;

			}
		}
	}
}