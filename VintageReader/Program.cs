using System;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

using VintageReader.Book;
using VintageReader.UI;
using VintageReader.Library;

namespace VintageReader
{
	class Program
	{

		public const string NAME = "VintageReader v0.1";
		public const string SUBTITLE = "An old-style reader for your new books";

		public const string COPYRIGHT = "(c) 2016, Noel Garcia Guimeras <garcia.guimeras@gmail.com>";
		public const string LICENSE = "Released under GNU General Public License";

		public static void ShowFile(string filename)
		{
			ConsoleWindow wnd = new ConsoleWindow("");

			LibraryManager libraryManager = new LibraryManager();
			libraryManager.Read();

			Console.WriteLine("Reading epub...");
			BookReader bookReader = new BookReader();
			BookInfo bookInfo = bookReader.Read(filename);
			if (bookInfo == null)
			{
				Console.WriteLine("File does not seems to have epub format: {0}", filename);
				return;
			}	

			Console.WriteLine("Loading library...");
			LibraryInfo libraryInfo = libraryManager.Find(bookInfo);
			if (libraryInfo == null)
				libraryInfo = libraryManager.CreateNew(bookInfo);

			bookInfo.LoadFromReader(wnd.BookContent.Metrics.Width, wnd.BookContent.Metrics.Height);
			wnd.Show(bookInfo, libraryInfo);
			wnd.ClearScreen();

			libraryManager.Write();
		}

		public static void Main(string[] args)
		{
			Console.WriteLine(NAME);
			Console.WriteLine(SUBTITLE);
			Console.WriteLine(LICENSE);
			Console.WriteLine();

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
