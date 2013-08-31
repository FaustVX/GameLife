using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace GameLife
{
	internal static class Program
	{
		private static readonly Font Font;
		private static readonly RenderWindow Window;
		private static readonly GameLife GameLife;
		private static readonly Color RegularGrey;

		private static readonly Vector2i Offset;
		private static readonly Dictionary<bool, Color> PauseColor;

		static Program()
		{
			RegularGrey = new Color(127, 127, 127);
			Font = new Font(Config.Configuration.Font);
			PauseColor = Config.Configuration.PauseColor;

			Offset = new Vector2i(0, 50);
			GameLife = new GameLife(100, Offset);

			Window =
				new RenderWindow(
					new VideoMode((uint)(GameLife.Width * LifeCell.Width + Offset.X), (uint)(GameLife.Height * LifeCell.Height + Offset.Y), 32),
					"Game of life")
					{
						Position = new Vector2i(0, 10)
					};

			Window.SetFramerateLimit(100);
			Window.Closed += (s, e) => Window.Close();

			Window.KeyReleased += Window_KeyReleased;
			Window.MouseButtonReleased += window_MouseButtonReleased;
			Window.MouseMoved += Window_MouseMoved;
		}

		static void Window_KeyReleased(object sender, KeyEventArgs e)
		{
			switch (e.Code)
			{
				case Keyboard.Key.Space:
					GameLife.Pause();
					break;
				case Keyboard.Key.Right:
					GameLife.OneFrame();
					break;
				case Keyboard.Key.Left:
					GameLife.Reset();
					break;
				case Keyboard.Key.LShift:
				case Keyboard.Key.RShift:
					if (GameLife.States)
						GameLife.Quadricolor();
					else
						GameLife.Bicolor();
					break;
			}
		}

		private static void Window_MouseMoved(object sender, MouseMoveEventArgs e)
		{
			e.X -= Offset.X;
			e.Y -= Offset.Y;
			if (e.X < 0 || e.Y < 0 || e.X >= GameLife.Width * LifeCell.Width || e.Y >= GameLife.Height * LifeCell.Height)
			{
				GameLife.SelectedCell = null;
				return;
			}

			int x = (int)(e.X / LifeCell.Width);
			int y = (int)(e.Y / LifeCell.Height);

			GameLife.SelectedCell = GameLife[x, y];
		}

		private static void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
		{
			if (GameLife.Running)
				return;
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

				Window.Draw(
					new Text(
						"Gen:" + GameLife.Generation + "\tCurent Cell: " +
						(GameLife.SelectedCell == null
							? "-1, -1 ()"
							: ((GameLife.SelectedCell.X + 1) + ", " + (GameLife.SelectedCell.Y + 1) + " (" + GameLife.SelectedCell.State +
								")")), Font)
						{
							Position = new Vector2f(15, 3),
							Color = RegularGrey
						});



				//Window.Draw(new Text("Gen:\n" + GameLife.Generation, Font)
				//	{
				//		Color = RegularGrey,
				//		Style = Text.Styles.Underlined
				//	});

				foreach (RectangleShape shape in GameLife.Draw())
					Window.Draw(shape);

				Window.Display();
			}
		}
	}
}
