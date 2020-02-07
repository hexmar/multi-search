using MultiSearch.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiSearch.Domain.Services
{
	public interface ISearchService
	{
		Task<IEnumerable<SearchResultItem>> SearchAsync(string query);
		IEnumerable<SearchResultItem> SearchLocal(string query);
	}
}
