using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiSearch.Domain.Models
{
	public interface ISearchEngine
	{
		Task<string> GetDataAsync(string query);
		IEnumerable<SearchResultItem> ParseData(string data);
	}
}
