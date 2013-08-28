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

		private readonly int[] _live, _survive;
		private readonly int _cellWidth, _cellHeight, _gridWidth, _gridHeight;
		private readonly int _fps;
		private readonly Dictionary<Cell.LiveState, Color> _colors, _selectedColors;
		private readonly Dictionary<bool, Color> _pauseColor;
		private readonly string _font;

		static Config()
		{
			Configuration = Create(@"Config.json");
		}

		public Config(int[] live, int[] survive, int cellWidth, int cellHeight, int gridWidth, int gridHeight, int fps,
					Dictionary<Cell.LiveState, Color> colors, Dictionary<Cell.LiveState, Color> selectedColors,
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

		public Config(int[] live, int[] survive, int cellWidth, int cellHeight, int gridWidth, int gridHeight, int fps, Dictionary<Cell.LiveState, Color> colors, Dictionary<Cell.LiveState, Color> selectedColors, Dictionary<bool, Color> pauseColor, string font)
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

		public Dictionary<Cell.LiveState, Color> Colors
		{
			get { return _colors; }
		}

		public Dictionary<Cell.LiveState, Color> SelectedColors
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
				var c = new Dictionary<Cell.LiveState, Color>()
					{
						{Cell.LiveState.Emerging, Color.Green},
						{Cell.LiveState.Live, Color.Blue},
						{Cell.LiveState.Dying, new Color(255, 127, 0)},
						{Cell.LiveState.Dead, new Color(200, 200, 200)}
					};
				var c1 = new Dictionary<Cell.LiveState, Color>()
					{
						{Cell.LiveState.Emerging, Color.Cyan},
						{Cell.LiveState.Live, Color.White},
						{Cell.LiveState.Dying, Color.Red},
						{Cell.LiveState.Dead, Color.Black}
					};
				var c2 = new Dictionary<bool, Color>()
					{
						{true, Color.Black},
						{false, Color.White}
					};

				return new Config(new int[] { 3 }, new int[] { 2, 3 }, 10, 10, 90, 60, 25, c, c1, c2, @"Font\visitor.ttf", filepath);
			}
			else
			{
				StreamReader sr = new StreamReader(filepath);
				JObject parser = JObject.Parse(sr.ReadToEnd());
				Config model = Create(string.Empty);
				JToken r = parser.SelectToken("Live", false);

				int[] live = r == null ? model.Live : r.ToObject<int[]>();
				r = parser.SelectToken("Survive", false);
				int[] survive = r == null ? model.Survive : r.ToObject<int[]>();
				r = parser.SelectToken("CellHeight", false);
				int cellH = r == null ? model.CellHeight : r.ToObject<int>();
				r = parser.SelectToken("CellWidth", false);
				int cellW = r == null ? model.CellWidth : r.ToObject<int>();
				r = parser.SelectToken("GridHeight", false);
				int gridH = r == null ? model.GridHeight : r.ToObject<int>();
				r = parser.SelectToken("GridWidth", false);
				int gridW = r == null ? model.GridWidth : r.ToObject<int>();
				r = parser.SelectToken("FPS", false);
				int fps = r == null ? model.FPS : r.ToObject<int>();
				r = parser.SelectToken("Colors", false);
				var c = r == null ? model.Colors : r.ToObject<Dictionary<Cell.LiveState, Color>>();
				r = parser.SelectToken("SelectedColors", false);
				var c1 = r == null ? model.SelectedColors : r.ToObject<Dictionary<Cell.LiveState, Color>>();
				r = parser.SelectToken("PauseColor", false);
				var c2 = r == null ? model.PauseColor : r.ToObject<Dictionary<bool, Color>>();
				r = parser.SelectToken("Font", false);
				var font = r == null ? model.Font : r.ToObject<string>();

				sr.Close();

				return new Config(live, survive, cellW, cellH, gridW, gridH, fps, c, c1, c2, font, filepath);
			}
		}
	}
}