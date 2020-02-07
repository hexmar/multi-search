using Microsoft.EntityFrameworkCore;
using MultiSearch.Domain.Models;

namespace MultiSearch.Repository
{
	public class MultiSearchContext : DbContext
	{
		public MultiSearchContext(DbContextOptions<MultiSearchContext> options) : base(options)
		{
		}

		public DbSet<SearchResultItem> SearchResults { get; set; }
	}
}
