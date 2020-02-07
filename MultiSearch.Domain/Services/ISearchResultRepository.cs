using MultiSearch.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiSearch.Domain.Services
{
	public interface ISearchResultRepository
	{
		IQueryable<SearchResultItem> GetAll();
		Task<IEnumerable<SearchResultItem>> InsertResultsAsync(IEnumerable<SearchResultItem> results);
	}
}
