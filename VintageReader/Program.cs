using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

using VintageReader.Book;
using VintageReader.UI;
using VintageReader.Library;
using System.Reflection;
using System.Collections;
using System.Security.Cryptography;

namespace VintageReader
{

	class ProgramInfo
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string Description { get; set; }
		public string Copyright { get; set; }
	}

	class Program
	{

		public const string LICENSE = "Released under GNU General Public License";

		private static ProgramInfo GetAssemblyInfo()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = assembly.GetName();
			AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)assembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute));
			AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
			return new ProgramInfo
			{
				Name = assemblyName.Name,
				Version = assemblyName.Version.ToString(),
				Description = description.Description,
				Copyright = copyright.Copyright
			};
		}

		private static void ShowFile(string filename)
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
			ProgramInfo programInfo = GetAssemblyInfo();
			Console.WriteLine(string.Format("{0} v{1}", programInfo.Name, programInfo.Version));
			Console.WriteLine(programInfo.Description);
			Console.WriteLine(programInfo.Copyright);
			Console.WriteLine(LICENSE);
			Console.WriteLine();

			if (true)
				return;

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
