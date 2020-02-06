using MultiSearch.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiSearch.Web.Models
{
	public class SearchResultsViewModel
	{
		public IEnumerable<SearchResultItem> SearchResults { get; set; }
		public string Query { get; set; }
	}
}
