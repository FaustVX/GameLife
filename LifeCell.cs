using System.Collections.Generic;
using System.Diagnostics;
using CSharpHelper;
using SFML.Graphics;
using SFML.Window;

namespace GameLife
{
	public class LifeCell : DiagonalCell<LifeCell>
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

		static LifeCell()
		{
			width = Config.Configuration.CellWidth;
			height = Config.Configuration.CellHeight;
		}

		public LifeCell(int x, int y, Grid<LifeCell> list)
			: base(x, y, list, true)
		{
			_state = LiveState.Dead;
			_shape = CreateShape();
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

		private static RectangleShape CreateShape()
		{
			return new RectangleShape(new Vector2f(width, height))
				{
					FillColor = GameLife.Colors[LiveState.Dead],
					OutlineColor = Color.Cyan
				};
		}

		public int Neighbor()
		{
			int i = 0;

			if (Up != null && (Up.State == LiveState.Emerging || Up.State == LiveState.Live))
				i++;
			if (Down != null && (Down.State == LiveState.Emerging || Down.State == LiveState.Live))
				i++;
			if (Left != null && (Left.State == LiveState.Emerging || Left.State == LiveState.Live))
				i++;
			if (Right != null && (Right.State == LiveState.Emerging || Right.State == LiveState.Live))
				i++;
			if (UpLeft != null && (UpLeft.State == LiveState.Emerging || UpLeft.State == LiveState.Live))
				i++;
			if (UpRight != null && (UpRight.State == LiveState.Emerging || UpRight.State == LiveState.Live))
				i++;
			if (DownLeft != null && (DownLeft.State == LiveState.Emerging || DownLeft.State == LiveState.Live))
				i++;
			if (DownRight != null && (DownRight.State == LiveState.Emerging || DownRight.State == LiveState.Live))
				i++;

			return i;
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