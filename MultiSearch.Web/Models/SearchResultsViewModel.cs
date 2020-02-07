using MultiSearch.Domain.Models;
using System.Collections.Generic;

namespace MultiSearch.Web.Models
{
	public class SearchResultsViewModel
	{
		public IEnumerable<SearchResultItem> SearchResults { get; set; }
		public string Query { get; set; }
		public string SearchEngine { get; set; }
		public bool LocalSearch { get; set; }
	}
}
