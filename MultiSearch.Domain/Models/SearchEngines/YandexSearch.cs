using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiSearch.Domain.Models.SearchEngines
{
	public class YandexSearch : ISearchEngine
	{
		public Task<string> GetDataAsync(string request)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<SearchResultItem> ParseData(string data)
		{
			throw new NotImplementedException();
		}
	}
}
