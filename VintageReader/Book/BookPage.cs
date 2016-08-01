using System;
using System.Collections.Generic;

namespace VintageReader.Book
{
	class BookPage
	{

		public string Chapter { get; set; }
		public List<string> Lines { get; set; }

		public BookPage()
		{
			Lines = new List<string>();
		}

		public bool IsBlank()
		{
			foreach (string line in Lines)
			{
				if (line.Trim().Length > 0)
					return false;
			}
			return true;
		}

	}
}

