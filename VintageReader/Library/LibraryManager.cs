using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using VintageReader.Book;


namespace VintageReader.Library
{
	class LibraryInfo
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public double ReadingPosition { get; set; }
		public DateTime LastReading { get; set; }
	}

	class LibraryManager
	{

		const string NAME = "VintageReaderLibrary.xml";

		string filename;
		List<LibraryInfo> bookList;

		public LibraryManager()
		{
			string dir = Environment.CommandLine.Split(' ')[0];
			dir = Path.GetDirectoryName(dir);
			filename = dir + Path.DirectorySeparatorChar + NAME;
			bookList = new List<LibraryInfo>();
		}

		public void Read()
		{
			bookList = new List<LibraryInfo>();

			if (!File.Exists(filename))
				return;

			XDocument xDoc = XDocument.Load(filename);
			IEnumerable<XElement> elements = xDoc.Elements("library");
			if (elements.Count() != 1)
				return;

			XElement root = elements.First();
			IEnumerable<XElement> books = root.Elements().Where(e => e.Name.ToString().Equals("book"));
			foreach (XElement book in books)
			{
				double pos = 0f;
				if (book.Element("position") != null)
					double.TryParse(book.Element("position").Value, out pos);

				LibraryInfo info = new LibraryInfo 
				{
					Title = book.Element("title") != null ? book.Element("title").Value : "",
					Author = book.Element("author") != null ? book.Element("author").Value : "",
					ReadingPosition = pos,
					LastReading = book.Element("last-reading") != null ? DateTime.Parse(book.Element("last-reading").Value) : DateTime.Now
				};
				bookList.Add(info);
			}
		}

		public void Write()
		{
			XElement library = new XElement("library");
			foreach (LibraryInfo info in bookList)
			{
				XElement book = new XElement("book");
				XElement title = new XElement("title") { Value = info.Title };
				XElement author = new XElement("author") { Value = info.Author };
				XElement position = new XElement("position") { Value = info.ReadingPosition.ToString() };
				XElement lastReading = new XElement("last-reading") { Value = info.LastReading.ToString() };
				book.Add(title);
				book.Add(author);
				book.Add(position);
				book.Add(lastReading);
				library.Add(book);
			}
			XDocument xDoc = new XDocument();
			xDoc.Add(library);

			using (FileStream fs = File.OpenWrite(filename))
			{
				xDoc.Save(fs);
			}
		}

		public LibraryInfo Find(BookInfo bookInfo)
		{
			foreach (LibraryInfo libInfo in bookList)
			{
				if (libInfo.Title.Equals(bookInfo.Title) && libInfo.Author.Equals(bookInfo.Author))
					return libInfo;
			}
			return null;
		}

		public LibraryInfo CreateNew(BookInfo bookInfo)
		{
			LibraryInfo libInfo = new LibraryInfo 
			{
				Title = bookInfo.Title,
				Author = bookInfo.Author,
				ReadingPosition = 0f,
				LastReading = DateTime.Now
			};
			bookList.Add(libInfo);
			return libInfo;
		}

	}
}

