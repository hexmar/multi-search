using System.Collections.Generic;

namespace MultiSearch.Domain.Models
{
	public interface ISearchEngine
	{
		string GetData(string request);
		IEnumerable<SearchResultItem> ParseData(string data);
	}
}
