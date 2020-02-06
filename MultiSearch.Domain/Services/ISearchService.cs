using MultiSearch.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MultiSearch.Domain.Services
{
	public interface ISearchService
	{
		Task<IEnumerable<SearchResultItem>> Search(string query);
	}
}
