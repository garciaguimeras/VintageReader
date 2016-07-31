using System;
using System.Collections.Generic;

namespace VintageReader.Book
{
	class BookInfo
	{

		private BookReader reader;

		public string Title { get; set; }
		public string Author { get; set; }
		public BookSpine Spine { get; set; }
		public List<BookPage> Pages { get; set; }
		public int LinesPerPage { get; set; }
		public int CharactersPerLine { get; set; }
		public int PageCount { get { return Pages.Count; } }

		public BookInfo(BookReader reader)
		{
			this.reader = reader;

			Title = "";
			Author = "";
			Pages = new List<BookPage>();
			Spine = new BookSpine();
		}

		public void LoadFromReader(int width, int height)
		{
			CharactersPerLine = width;
			LinesPerPage = height;
			reader.AddPages(this);
		}

	}
}

