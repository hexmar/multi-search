using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiSearch.Domain.Models;
using MultiSearch.Domain.Services;

namespace MultiSearch.Repository
{
	public class SearchResultRepository : ISearchResultRepository
	{
		private readonly MultiSearchContext _context;

		public SearchResultRepository(MultiSearchContext context)
		{
			_context = context;
		}

		public IQueryable<SearchResultItem> GetAll()
		{
			return _context.SearchResults.AsNoTracking();
		}

		public async Task<IEnumerable<SearchResultItem>> InsertResultsAsync(IEnumerable<SearchResultItem> results)
		{
			await _context.SearchResults.AddRangeAsync(results);
			await _context.SaveChangesAsync();

			return results;
		}
	}
}
