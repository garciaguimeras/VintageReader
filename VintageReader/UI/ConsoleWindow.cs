using System;
using System.Collections.Generic;

using VintageReader.Book;

namespace VintageReader.UI
{

	class WindowMetrics
	{
		public int Top { get; set; }
		public int Left { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}

	enum TextAlign
	{
		LEFT, 
		RIGHT,
		CENTER,
		JUSTIFY
	}

	abstract class ConsoleElement
	{
		public ConsoleColor BackgroundColor { get; set; }
		public ConsoleColor TextColor { get; set; }
		public WindowMetrics Metrics { get; set; }

		public abstract void Draw();
	
		protected ConsoleElement(WindowMetrics metrics)
		{
			Metrics = metrics;
		}

		private string JustifyText(string text, int width)
		{
			int diff = width - text.Length;
			if (diff > 0)
			{
				List<int> spacePositions = new List<int>();
				for (int i = 0; i < text.Length; i++)
					if (text[i] == ' ')
						spacePositions.Add(i);
				if (spacePositions.Count == 0)
					return text;

				int rest = 0;
				int patchSize = Math.DivRem(diff, spacePositions.Count, out rest);

				string patch = "";
				for (int i = 0; i < patchSize; i++)
					patch = patch + " ";

				for (int i = spacePositions.Count - 1; i >= 0; i--)
				{
					int pos = spacePositions[i];
					if (rest > 0)
					{
						text = text.Substring(0, pos) + patch + " " + text.Substring(pos);
						rest = rest - 1;
					}
					else
						text = text.Substring(0, pos) + patch + text.Substring(pos);
				}
			}
			return text;
		}

		public void WriteText(string text, WindowMetrics metrics, TextAlign align)
		{
			Console.ForegroundColor = TextColor;
			Console.BackgroundColor = BackgroundColor;

			// Fix end of line
			// TODO: Improve this: Is paragraph => left... Is title => CENTER
			if (text.EndsWith("\n") && align == TextAlign.JUSTIFY)
				align = TextAlign.LEFT;				

			int top = metrics.Top;
			int left = 0;
			switch (align)
			{
				case TextAlign.LEFT:
					left = metrics.Left;
					break;
				case TextAlign.RIGHT:
					int len = text.Length == 0 ? 1 : text.Length;
					left = metrics.Left + metrics.Width - len;
					break;
				case TextAlign.CENTER:
					left = metrics.Left + (metrics.Width / 2 - text.Length / 2);
					break;
				case TextAlign.JUSTIFY:
					left = metrics.Left;
					text = JustifyText(text, metrics.Width);
					break;
			}

			Console.CursorTop = top;
			Console.CursorLeft = left;
			Console.Write(text);
		}

	}

	class StatusBar : ConsoleElement
	{

		public StatusBar(WindowMetrics metrics) : base(metrics)
		{}

		public override void Draw()
		{
			Console.ForegroundColor = TextColor;
			Console.BackgroundColor = BackgroundColor;
			for (int i = Metrics.Left; i < Metrics.Width; i++)
			{
				Console.CursorLeft = i;
				Console.CursorTop = Metrics.Top;
				Console.Write(" ");
			}
		}

	}

	class TopBar : StatusBar
	{
		public string Title { get; set; }

		public TopBar(int width, string title) : base(new WindowMetrics { Top = 0, Left = 0, Width = width, Height = 1 })
		{
			BackgroundColor = ConsoleColor.DarkBlue;
			TextColor = ConsoleColor.Gray;
			Title = title;
		}

		public override void Draw()
		{
			base.Draw();
			WriteText(Title, Metrics, TextAlign.CENTER);
		}

	}

	class BottomBar : StatusBar
	{
		public string LeftText { get; set; }
		public string RightText { get; set; }

		public BottomBar(int width, int height) : base(new WindowMetrics { Top = height - 1, Left = 0, Width = width, Height = 1 })
		{
			BackgroundColor = ConsoleColor.DarkBlue;
			TextColor = ConsoleColor.Gray;
			LeftText = "";
			RightText = "";
		}

		public override void Draw()
		{
			base.Draw();
			WriteText(LeftText, Metrics, TextAlign.LEFT);
			WriteText(RightText, Metrics, TextAlign.RIGHT);
		}

	}

	class HelpBar : StatusBar
	{
		public string Text { get; set; }

		public HelpBar(int width, int height) : base(new WindowMetrics { Top = height - 2, Left = 0, Width = width, Height = 1 })
		{
			BackgroundColor = ConsoleColor.Gray;
			TextColor = ConsoleColor.DarkGray;
			Text = "Some help!";
		}

		public override void Draw()
		{
			base.Draw();
			WriteText(Text, Metrics, TextAlign.LEFT);
		}

	}

	class TextView : ConsoleElement
	{

		List<string> lines;

		public TextView(WindowMetrics metrics) : base(metrics)
		{
			lines = new List<string>();
		}

		public void SetLines(List<string> lines)
		{
			this.lines = lines;
		}

		public override void Draw()
		{
			BackgroundColor = ConsoleColor.Black;
			TextColor = ConsoleColor.Gray;

			WindowMetrics textMetrics = new WindowMetrics 
			{
				Top = Metrics.Top,
				Left = Metrics.Left,
				Width = Metrics.Width,
				Height = 1
			};

			int total = 0;
			foreach (string line in lines)
			{
				if (total < Metrics.Height)
				{
					WriteText(line, textMetrics, TextAlign.JUSTIFY);
					textMetrics.Top++;
					total++;
				}
			}
		}

	}

	enum BookReaderAction
	{
		NONE,
		QUIT, 
		NEXT_PAGE,
		PREV_PAGE,
		RELOAD
	}

	class ConsoleWindow
	{

		public int Width { get { return Console.WindowWidth; } }
		public int Height { get { return Console.WindowHeight; } }
		public TextView BookContent { get { return textView; } }

		protected TopBar topBar;
		protected BottomBar bottomBar;
		protected HelpBar helpBar;
		protected TextView textView;

		public ConsoleWindow(string title)
		{
			topBar = new TopBar(Width, title);
			bottomBar = new BottomBar(Width, Height);
			helpBar = new HelpBar(Width, Height);
			textView = new TextView(new WindowMetrics
			{ 
				Left = 1,
				Top = 2,
				Width = Width - 2,
				Height = Height - 5
			});
		}

		public void ClearScreen()
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Clear();
			Console.CursorTop = 0;
			Console.CursorLeft = 0;
		}

		public void Draw()
		{
			ClearScreen();
			topBar.Draw();
			textView.Draw();
			helpBar.Draw();
			bottomBar.Draw();
		}

		private BookReaderAction GetNextAction()
		{
			ConsoleKeyInfo keyInfo = Console.ReadKey();

			switch (keyInfo.KeyChar)
			{
				case 'q':
				case 'Q':
					return BookReaderAction.QUIT;
				
				case 'n':
				case 'N':
					return BookReaderAction.NEXT_PAGE;

				case 'p':
				case 'P':
					return BookReaderAction.PREV_PAGE;

				case 'r':
				case 'R':
					return BookReaderAction.RELOAD;
			}

			return BookReaderAction.NONE;
		}

		public void Execute(BookInfo bookInfo)
		{
			topBar.Title = string.Format("{0} - {1}", bookInfo.Author, bookInfo.Title);
			helpBar.Text = "q:Quit n:Next p:Prev r:Reload";

			int pageNumber = 0;
			BookReaderAction nextAction = BookReaderAction.NONE;
			while (nextAction != BookReaderAction.QUIT)
			{
				int percent = (int)((pageNumber + 1) * 100 / bookInfo.Pages.Count);

				bottomBar.LeftText = Program.NAME;
				bottomBar.RightText = string.Format("Page {0} of {1} ({2}%)", pageNumber + 1, bookInfo.Pages.Count, percent);
				textView.SetLines(bookInfo.Pages[pageNumber].Lines);

				if (bookInfo.Pages[pageNumber].IsBlank())
					bottomBar.LeftText = "This page was left blank.";

				Draw();

				nextAction = GetNextAction();
				switch (nextAction)
				{
					case BookReaderAction.NEXT_PAGE:
						if (pageNumber < bookInfo.Pages.Count - 1)
							pageNumber++;
						break;

					case BookReaderAction.PREV_PAGE:
						if (pageNumber > 0)
							pageNumber--;
						break;

					case BookReaderAction.RELOAD:
						bookInfo.LoadFromReader(Width, Height);
						pageNumber = (int)(bookInfo.Pages.Count * percent / 100);
						break;
				}
			}
		}

	}
}

