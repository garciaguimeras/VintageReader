using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

using VintageReader.Book;
using VintageReader.UI;

namespace VintageReader
{
	class Program
	{

		public const string NAME = "VintageReader v0.1";

		public static void ShowFile(string filename)
		{
			ConsoleWindow wnd = new ConsoleWindow("Some title for the window");

			BookReader reader = new BookReader();
			BookInfo info = reader.Read(filename);
			info.LoadFromReader(wnd.BookContent.Metrics.Width, wnd.BookContent.Metrics.Height);
			// info.LoadFromReader(80, 25);
			wnd.Execute(info);
			wnd.ClearScreen();
		}

		public static void Main(string[] args)
		{
			Console.WriteLine(NAME);
			if (args.Length == 0)
			{
				Console.WriteLine("Missing argument: filename.");
				return;
			}

			string filename = args[0];
			if (!File.Exists(filename))
			{
				Console.WriteLine("File does not exist: {0}", filename);
				return;
			}	

			ShowFile(filename);
		}
	}
}
