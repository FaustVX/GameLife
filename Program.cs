using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace SFML
{
	internal static class Program
	{
		private static readonly Font Font;
		private static readonly RenderWindow Window;
		private static readonly GameLife GameLife;
		private static readonly Color RegularGrey;

		private static readonly Window.Vector2i Offset;
		private static readonly Dictionary<bool, Color> PauseColor;

		static Program()
		{
			RegularGrey = new Color(127, 127, 127);
			Font = new Font(@"Font\visitor.ttf");
			PauseColor = new Dictionary<bool, Color>()
				{
					{true, Color.Black},
					{false, Color.White}
				};

			Offset = new Window.Vector2i(100, 0);
			GameLife = new GameLife(60, 40);

			Window =
				new RenderWindow(
					new Window.VideoMode((uint)(GameLife.Width * Cell.Width + Offset.X), (uint)(GameLife.Height * Cell.Height + Offset.Y), 32),
					"Game of live")
					{
						Position = new Window.Vector2i(300, 100)
					};

			Window.SetFramerateLimit(10);
			Window.Closed += (s, e) => Window.Close();

			Window.KeyReleased += Window_KeyReleased;
			Window.MouseButtonReleased += window_MouseButtonReleased;
			Window.MouseMoved += Window_MouseMoved;
		}

		static void Window_KeyReleased(object sender, Window.KeyEventArgs e)
		{
			switch (e.Code)
			{
				case Keyboard.Key.Space:
					GameLife.Running = !GameLife.Running;
					break;
				case Keyboard.Key.Right:
					GameLife.OneFrame();
					break;
				case Keyboard.Key.Left:
					GameLife.Reset();
					break;
			}
		}

		private static void Window_MouseMoved(object sender, Window.MouseMoveEventArgs e)
		{
			e.X -= Offset.X;
			e.Y -= Offset.Y;
			if (e.X < 0 || e.Y < 0 || e.X >= GameLife.Width * Cell.Width || e.Y >= GameLife.Height * Cell.Height)
			{
				GameLife.SelectedCell = null;
				return;
			}

			int x = (int)(e.X / Cell.Width);
			int y = (int)(e.Y / Cell.Height);

			GameLife.SelectedCell = GameLife[x, y];
		}

		private static void window_MouseButtonReleased(object sender, Window.MouseButtonEventArgs e)
		{
			if (e.Button != SFML.Window.Mouse.Button.Left)
				return;

			if (GameLife.SelectedCell == null)
			{
				if (e.X <= Offset.X)
					GameLife.Running = !GameLife.Running;
				return;
			}

			GameLife.SelectedCell.Click();
		}

		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		private static void Main()
		{
			while (Window.IsOpen())
			{
				Window.DispatchEvents();
				Window.Clear(PauseColor[GameLife.Running]);

				Window.Draw(new Text("Gen:\n" + GameLife.Generation.ToString(), Font)
					{
						Color = RegularGrey,
						Style = Text.Styles.Underlined
					});

				foreach (RectangleShape shape in GameLife.Draw(Offset.X))
					Window.Draw(shape);

				Window.Display();

			}
		}
	}
}
