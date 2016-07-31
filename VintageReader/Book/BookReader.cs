﻿using System;
using System.Collections.Generic;

using eBdb.EpubReader;


namespace VintageReader.Book
{
	class BookReader
	{

		Epub epub;

		public BookInfo Read(string filename)
		{
			epub = new Epub(filename);
			return new BookInfo(this) { Title = epub.Title[0], Author = epub.Creator[0] };
		}

		private void ReadBookStructure(List<NavPoint> navPoints, BookInfo bookInfo)
		{
			if (navPoints == null || navPoints.Count == 0)
				return;

			foreach(NavPoint np in navPoints)
			{
				List<BookPage> pages = PaginateText(np.ContentData.GetContentAsPlainText(), bookInfo);

				IndexItem indexItem = new IndexItem 
				{
					Title = np.Title,
					Page = pages[0]
				};
					
				bookInfo.Pages.AddRange(pages);
				bookInfo.Spine.Index.Add(indexItem);

				ReadBookStructure(np.Children, bookInfo);
			}
		}

		private string CutLine(string text, int chrsPerLine)
		{
			int i = chrsPerLine;

			if (text.Length > chrsPerLine)
			{	
				while (i > 0 && text[i] != ' ')
					i--;
				if (i == 0)
					i = chrsPerLine;
			}
			else
				i = text.Length;

			string eol = "";
			int pos = text.IndexOf("\n");
			if (pos != -1 && pos < i)
			{
				i = pos;
				eol = "\n";
			}

			return text.Substring(0, i).Trim() + eol;
		}

		private List<BookPage> PaginateText(string text, BookInfo bookInfo)
		{
			List<BookPage> pages = new List<BookPage>();

			text = text.Replace("\r\n", "\n");
			text = text.Replace("\r", "\n");
				
			BookPage p = new BookPage();
			int lineNumber = 0;
			while (!string.IsNullOrEmpty(text))
			{
				string line = CutLine(text, bookInfo.CharactersPerLine);
				text = line.Length < text.Length ? text.Substring(line.Length + 1) : "";
				p.Lines.Add(line);

				lineNumber = lineNumber + 1;
				if (lineNumber == bookInfo.LinesPerPage)
				{
					pages.Add(p);
					lineNumber = 0;
					p = new BookPage();
				}
			}

			if (lineNumber > 0)
				pages.Add(p);

			return pages;
		}

		public void AddPages(BookInfo bookInfo)
		{
			ReadBookStructure(epub.TOC, bookInfo);
		}

	}
}