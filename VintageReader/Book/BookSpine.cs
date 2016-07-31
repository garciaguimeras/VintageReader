using System;
using System.Collections.Generic;

namespace VintageReader.Book
{

	class IndexItem
	{
		public string Title { get; set; }
		public BookPage Page { get; set; }
	}

	class BookSpine
	{

		public List<IndexItem> Index { get; protected set; }

		public BookSpine()
		{
			Index = new List<IndexItem>();
		}
	}

}

