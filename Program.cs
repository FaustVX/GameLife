//#define WINDOWS
//#define QUICK

using System.Collections.Generic;
using CSharpHelper;
using SFML.Graphics;
using System.Linq;

namespace SFML
{
	internal static class Program
	{
		private enum LiveState
		{
			Emerging,
			Live,
			Dying,
			Dead
		}

		private static readonly RenderWindow Window;
#if WINDOWS
		private static readonly RenderWindow ZommedWindow;
#endif

		private static readonly KeyValuePair<LiveState, Drawable>[][] Cells;
		private static readonly Window.Vector2f CellSize;
		private static readonly Window.Vector2i NbCells, Offset;
		private static readonly Dictionary<LiveState, Color> Colors, SelectedColors;
		private static readonly Dictionary<bool, Color> PauseColor;

#if WINDOWS
		private static readonly Window.Vector2f ZoomedCellSize;
		private static readonly Window.Vector2i NbZoomedCells, ZoomedOffset;
		//private static readonly Dictionary<LiveState, Color> SelectedColors;
#endif

		private static Window.Vector2i selectedCell;
		private static readonly List<KeyValuePair<LiveState, Drawable>[][]> States;
		private static readonly int StateButtonSize;
#if QUICK
		private static readonly Dictionary<LiveState, IList<KeyValuePair<LiveState, Drawable>>> Quick;
#endif
		private static bool running;

		static Program()
		{
			running = false;

#if QUICK
			Quick = new Dictionary<LiveState, IList<KeyValuePair<LiveState, Drawable>>>()
				{
					{LiveState.Emerging, new List<KeyValuePair<LiveState, Drawable>>()},
					{LiveState.Live, new List<KeyValuePair<LiveState, Drawable>>()},
					{LiveState.Dying, new List<KeyValuePair<LiveState, Drawable>>()},
					{LiveState.Dead, new List<KeyValuePair<LiveState, Drawable>>()}
				};
#endif


			Colors = new Dictionary<LiveState, Color>()
				{
					{LiveState.Emerging, Color.Green},
					{LiveState.Live, Color.Blue},
					{LiveState.Dying, new Color(255, 127, 0)},
					{LiveState.Dead, new Color(200, 200, 200)}
				};

			PauseColor = new Dictionary<bool, Color>()
				{
					{true, Color.Black},
					{false, Color.White}
				};

			SelectedColors = new Dictionary<LiveState, Color>()
				{
					{LiveState.Emerging, Color.Cyan},
					{LiveState.Live, Color.White},
					{LiveState.Dying, Color.Red},
					{LiveState.Dead, Color.Black}
				};

			NbCells = new Window.Vector2i(60, 40);
			CellSize = new Window.Vector2f(15f, 15f);
			Offset = new Window.Vector2i(40, 0);
			StateButtonSize = 20;
#if WINDOWS
			NbZoomedCells = new Window.Vector2i(9, 9);

			ZoomedOffset = new Window.Vector2i(NbZoomedCells.X / 2, NbZoomedCells.Y / 2);
			ZoomedCellSize = new Window.Vector2f(CellSize.X * 2f, CellSize.Y * 2f);
#endif


			selectedCell = new Window.Vector2i(-1, -1);

			Cells = new KeyValuePair<LiveState, Drawable>[NbCells.X][];
			for (int i = 0; i < Cells.Length; ++i)
			{
				Cells[i] = new KeyValuePair<LiveState, Drawable>[NbCells.Y];
				for (int j = 0; j < Cells[i].Length; ++j)
				{
					Cells[i][j] = new KeyValuePair<LiveState, Drawable>(LiveState.Dead,
																		new RectangleShape(CellSize)
																			{
																				FillColor = Colors[LiveState.Dead],
																				OutlineColor = Color.Cyan
																			});
#if QUICK
					Quick[LiveState.Dead].Add(Cells[i][j]);
#endif

				}
			}

			States = new List<KeyValuePair<LiveState, Drawable>[][]>()
				{
					Cells
				};

			Window =
				new RenderWindow(
					new Window.VideoMode((uint)(NbCells.X * CellSize.X + Offset.X), (uint)(NbCells.Y * CellSize.Y + Offset.Y), 32),
					"Game of live")
					{
						Position = new Window.Vector2i(300, 100)
					};
#if WINDOWS
			ZommedWindow = new RenderWindow(
				new Window.VideoMode((uint)(NbZoomedCells.X * ZoomedCellSize.X), (uint)(NbZoomedCells.Y * ZoomedCellSize.Y), 32),
				"Zoom");
#endif


			Window.SetFramerateLimit(10);
			Window.Closed += (s, e) => Window.Close();
#if WINDOWS
			Window.Closed += (s, e) => ZommedWindow.Close();
#endif

			Window.MouseButtonReleased += window_MouseButtonReleased;
			Window.MouseMoved += Window_MouseMoved;
		}

		private static void Window_MouseMoved(object sender, Window.MouseMoveEventArgs e)
		{
			e.X -= Offset.X;
			e.Y -= Offset.Y;
			if (e.X < 0 || e.Y < 0 || e.X >= NbCells.X * CellSize.X || e.Y >= NbCells.Y * CellSize.Y)
			{
				selectedCell.X = -1;
				selectedCell.Y = -1;
				return;
			}

			int x = (int)(e.X / CellSize.X);
			int y = (int)(e.Y / CellSize.Y);

			selectedCell.X = x;
			selectedCell.Y = y;
		}

		private static KeyValuePair<LiveState, Drawable> Switch(KeyValuePair<LiveState, Drawable> item, LiveState newState)
		{
			if (item.Key == newState)
				return item;

			var newItem = new KeyValuePair<LiveState, Drawable>(newState, item.Value);
#if QUICK
			Quick[item.Key].Remove(item);
			Quick[newState].Add(newItem);
#endif

			return newItem;
		}

		private static void window_MouseButtonReleased(object sender, Window.MouseButtonEventArgs e)
		{
			if (e.Button != SFML.Window.Mouse.Button.Left)
				return;

			if (selectedCell.X == -1 || selectedCell.Y == -1)
			{
				if (e.X <= Offset.X)
					running = !running;
				return;
			}

			int x = selectedCell.X;
			int y = selectedCell.Y;

			switch (Cells[x][y].Key)
			{
				case LiveState.Emerging:
					Cells[x][y] = Switch(Cells[x][y], LiveState.Live);
					break;
				case LiveState.Live:
					Cells[x][y] = Switch(Cells[x][y], LiveState.Dying);
					break;
				case LiveState.Dying:
					Cells[x][y] = Switch(Cells[x][y], LiveState.Dead);
					break;
				case LiveState.Dead:
					Cells[x][y] = Switch(Cells[x][y], LiveState.Emerging);
					break;

			}
		}

		private static int Arround(
			this IList<IList<KeyValuePair<LiveState, Drawable>>> list, int x, int y)
		{
			int i = 0;

			if (x > 0 && (list[x - 1][y].Key == LiveState.Emerging || list[x - 1][y].Key == LiveState.Live))
				i++;
			if (x > 0 && y > 0 && (list[x - 1][y - 1].Key == LiveState.Emerging || list[x - 1][y - 1].Key == LiveState.Live))
				i++;
			if (y > 0 && (list[x][y - 1].Key == LiveState.Emerging || list[x][y - 1].Key == LiveState.Live))
				i++;
			if (x < NbCells.X - 1 && y > 0 && (list[x + 1][y - 1].Key == LiveState.Emerging || list[x + 1][y - 1].Key == LiveState.Live))
				i++;
			if (x < NbCells.X - 1 && (list[x + 1][y].Key == LiveState.Emerging || list[x + 1][y].Key == LiveState.Live))
				i++;
			if (x < NbCells.X - 1 && y < NbCells.Y - 1 &&
				(list[x + 1][y + 1].Key == LiveState.Emerging || list[x + 1][y + 1].Key == LiveState.Live))
				i++;
			if (y < NbCells.Y - 1 && (list[x][y + 1].Key == LiveState.Emerging || list[x][y + 1].Key == LiveState.Live))
				i++;
			if (x > 0 && y < NbCells.Y - 1 && (list[x - 1][y + 1].Key == LiveState.Emerging || list[x - 1][y + 1].Key == LiveState.Live))
				i++;

			return i;
		}

#if WINDOWS
		private static IntRect ToIntRect(int x, int y, int width, int height, int offsetX = 0, int offsetY = 0)
		{
			return new IntRect(x - offsetX, y - offsetY, width, height);
		}

		private static IEnumerable<IEnumerable<KeyValuePair<LiveState, Drawable>>> Square(
			this IEnumerable<IList<KeyValuePair<LiveState, Drawable>>> list, IntRect square)
		{
			int x = square.Left;
			int y = square.Top;
			int width = square.Width;
			int height = square.Height;

			if (x < -ZoomedOffset.X || y < -ZoomedOffset.Y)
				return new List<IList<KeyValuePair<LiveState, Drawable>>>();
			return list.Take(x + width).Skip(x).Select(item => item.Take(y + height).Skip(y));
		}
#endif


		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		private static void Main()
		{
			//int nbGeneration = 0;
			while (Window.IsOpen())
			{
				//for (int i = 1; i <= States.Count; i++)
				//{
				//	var rect = new RectangleShape(new Window.Vector2f(Offset.X, StateButtonSize))
				//		{
				//			Position = new Window.Vector2f(0, i * StateButtonSize)
				//		};
				//	Window.Draw(rect);
				//}

				if (running)
				{
					LiveState[][] newCells = new LiveState[NbCells.X][];
					for (int i = 0; i < NbCells.X; i++)
						newCells[i] = new LiveState[NbCells.Y];

					Cells.ForEach((i, j, cell) =>
						{
							int living = Cells.Arround(i, j);
							switch (cell.Key)
							{
								case LiveState.Emerging:
									if (living == 2 || living == 3)
										newCells[i][j] = LiveState.Live;
									else
										newCells[i][j] = LiveState.Dying;
									break;
								case LiveState.Live:
									if (living == 2 || living == 3)
										newCells[i][j] = cell.Key;
									else
										newCells[i][j] = LiveState.Dying;
									break;
								case LiveState.Dying:
									if (living == 3)
										newCells[i][j] = LiveState.Emerging;
									else
										newCells[i][j] = LiveState.Dead;
									break;
								case LiveState.Dead:
									if (living == 3)
										newCells[i][j] = LiveState.Emerging;
									else
										newCells[i][j] = cell.Key;
									break;
							}
						});
					newCells.ForEach((x, y, state) =>
						{
							Cells[x][y] = Switch(Cells[x][y], state);
						});
				}

				Window.DispatchEvents();
				Window.Clear(PauseColor[running]);
#if WINDOWS
				ZommedWindow.DispatchEvents();
				ZommedWindow.Clear();
#endif

					for (int i = 0; i < Cells.Length; ++i)
						for (int j = 0; j < Cells[i].Length; ++j)
						{
							var cell = Cells[i][j];
							RectangleShape rect = cell.Value as RectangleShape;
							if (rect == null)
								return;
							if (selectedCell.X >= 0 && selectedCell.Y >= 0 && cell.Value.Equals(Cells[selectedCell.X][selectedCell.Y].Value))
								rect.FillColor = SelectedColors[cell.Key];
							else
								rect.FillColor = Colors[cell.Key];
							rect.Position = new Window.Vector2f(i * CellSize.X + Offset.X, j * CellSize.Y + Offset.Y);
							rect.OutlineThickness = 0f;
							Window.Draw(rect);
						}

#if WINDOWS
				Cells.Square(ToIntRect(selectedCell.X, selectedCell.Y, NbZoomedCells.X, NbZoomedCells.Y, ZoomedOffset.X,
										ZoomedOffset.Y))
					.ForEach((
						x, y, item) =>
						{
							RectangleShape rect = item.Value as RectangleShape;
							if (rect == null)
								return;
							if (selectedCell.X >= 0 && selectedCell.Y >= 0 && item.Value.Equals(Cells[selectedCell.X][selectedCell.Y].Value))
								rect.FillColor = SelectedColors[item.Key];
							else
								rect.FillColor = Colors[item.Key];
							rect.Size = ZoomedCellSize;
							rect.Position = new Window.Vector2f(x * ZoomedCellSize.X, y * ZoomedCellSize.Y);
							rect.OutlineThickness = 1f;
							ZommedWindow.Draw(rect);
						});
#endif



				//if (running)
				//	if (nbGeneration++ >= 111)
				//		continue;
				Window.Display();
#if WINDOWS
				ZommedWindow.Display();
#endif

			}
		}
	}
}
