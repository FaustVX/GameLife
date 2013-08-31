using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFML.Graphics;

namespace GameLife
{
	public class Config
	{
		public static readonly Config Configuration;
		public static readonly Config Base;

		private readonly int[] _live, _survive;
		private readonly int _cellWidth, _cellHeight, _gridWidth, _gridHeight;
		private readonly int _fps;
		private readonly Dictionary<LifeCell.LiveState, Color> _colors, _selectedColors;
		private readonly Dictionary<bool, Color> _pauseColor;
		private readonly string _font;

		static Config()
		{
			var c = new Dictionary<LifeCell.LiveState, Color>()
				{
					{LifeCell.LiveState.Emerging, Color.Green},
					{LifeCell.LiveState.Live, Color.Blue},
					{LifeCell.LiveState.Dying, new Color(255, 127, 0)},
					{LifeCell.LiveState.Dead, new Color(200, 200, 200)}
				};
			var c1 = new Dictionary<LifeCell.LiveState, Color>()
				{
					{LifeCell.LiveState.Emerging, Color.Cyan},
					{LifeCell.LiveState.Live, Color.White},
					{LifeCell.LiveState.Dying, Color.Red},
					{LifeCell.LiveState.Dead, Color.Black}
				};
			var c2 = new Dictionary<bool, Color>()
				{
					{true, Color.Black},
					{false, Color.White}
				};
			Base = new Config(new int[] { 3 }, new int[] { 2, 3 }, 6, 6, 200, 100, 50, c, c1, c2, @"Font\Pokemon.ttf");
			Configuration = Create(@"Config.json");
		}

		public Config(int[] live, int[] survive, int cellWidth, int cellHeight, int gridWidth, int gridHeight, int fps,
					Dictionary<LifeCell.LiveState, Color> colors, Dictionary<LifeCell.LiveState, Color> selectedColors,
					Dictionary<bool, Color> pauseColor, string font, string filepath)
			: this(live, survive, cellWidth, cellHeight, gridWidth, gridHeight, fps, colors, selectedColors, pauseColor, font)
		{
			if (filepath == string.Empty)
				return;
			JsonSerializer serializer = new JsonSerializer
				{
					NullValueHandling = NullValueHandling.Include
				};

			using (StreamWriter sw = new StreamWriter(filepath))
			using (JsonWriter writer = new JsonTextWriter(sw) {Formatting = Formatting.Indented})
				serializer.Serialize(writer, this);
		}

		public Config(int[] live, int[] survive, int cellWidth, int cellHeight, int gridWidth, int gridHeight, int fps, Dictionary<LifeCell.LiveState, Color> colors, Dictionary<LifeCell.LiveState, Color> selectedColors, Dictionary<bool, Color> pauseColor, string font)
		{
			_live = live;
			_survive = survive;
			_cellWidth = cellWidth;
			_cellHeight = cellHeight;
			_gridWidth = gridWidth;
			_gridHeight = gridHeight;
			_fps = fps;
			_colors = colors;
			_selectedColors = selectedColors;
			_pauseColor = pauseColor;
			_font = font;
		}

		public int[] Live
		{
			get { return _live; }
		}

		public int[] Survive
		{
			get { return _survive; }
		}

		public int CellWidth
		{
			get { return _cellWidth; }
		}

		public int CellHeight
		{
			get { return _cellHeight; }
		}

		public int GridWidth
		{
			get { return _gridWidth; }
		}

		public int GridHeight
		{
			get { return _gridHeight; }
		}

		public int FPS
		{
			get { return _fps; }
		}

		public Dictionary<LifeCell.LiveState, Color> Colors
		{
			get { return _colors; }
		}

		public Dictionary<LifeCell.LiveState, Color> SelectedColors
		{
			get { return _selectedColors; }
		}

		public Dictionary<bool, Color> PauseColor
		{
			get { return _pauseColor; }
		}

		public string Font
		{
			get { return _font; }
		}

		private static Config Create(string filepath)
		{
			if (filepath == string.Empty || !File.Exists(filepath))
			{
				return new Config(Base.Live, Base.Survive, Base.CellWidth, Base.CellHeight, Base.GridWidth, Base.GridHeight, Base.FPS, Base.Colors, Base.SelectedColors, Base.PauseColor, Base.Font, filepath);
			}
			else
			{
				StreamReader sr = new StreamReader(filepath);
				JObject parser = JObject.Parse(sr.ReadToEnd());
				JToken r = parser.SelectToken("Live", false);

				int[] live = r == null ? Base.Live : r.ToObject<int[]>();
				r = parser.SelectToken("Survive", false);
				int[] survive = r == null ? Base.Survive : r.ToObject<int[]>();
				r = parser.SelectToken("CellHeight", false);
				int cellH = r == null ? Base.CellHeight : r.ToObject<int>();
				r = parser.SelectToken("CellWidth", false);
				int cellW = r == null ? Base.CellWidth : r.ToObject<int>();
				r = parser.SelectToken("GridHeight", false);
				int gridH = r == null ? Base.GridHeight : r.ToObject<int>();
				r = parser.SelectToken("GridWidth", false);
				int gridW = r == null ? Base.GridWidth : r.ToObject<int>();
				r = parser.SelectToken("FPS", false);
				int fps = r == null ? Base.FPS : r.ToObject<int>();
				r = parser.SelectToken("Colors", false);
				var c = r == null ? Base.Colors : r.ToObject<Dictionary<LifeCell.LiveState, Color>>();
				r = parser.SelectToken("SelectedColors", false);
				var c1 = r == null ? Base.SelectedColors : r.ToObject<Dictionary<LifeCell.LiveState, Color>>();
				r = parser.SelectToken("PauseColor", false);
				var c2 = r == null ? Base.PauseColor : r.ToObject<Dictionary<bool, Color>>();
				r = parser.SelectToken("Font", false);
				var font = r == null ? Base.Font : r.ToObject<string>();

				sr.Close();

				return new Config(live, survive, cellW, cellH, gridW, gridH, fps, c, c1, c2, font, filepath);
			}
		}
	}
}