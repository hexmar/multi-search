using System;
using System.Collections.Generic;
using System.Text;

namespace MultiSearch.Domain.Models
{
	public class GoogleSearch : ISearchEngine
	{
		public string GetData(string request)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SearchResultItem> ParseData(string data)
		{
			throw new NotImplementedException();
		}
	}
}
