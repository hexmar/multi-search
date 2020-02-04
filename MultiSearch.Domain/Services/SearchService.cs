using MultiSearch.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiSearch.Domain.Services
{
	public class SearchService
	{
		public async Task<IEnumerable<SearchResultItem>> Search(string query)
		{
			var searches = new List<ISearchEngine>()
			{
				new GoogleSearch(),
			};

			using var barrier = new Barrier(searches.Count);
			var responses = new List<Tuple<string, ISearchEngine>>(searches.Count);
			var tasks = searches.Select(se => new Task(
				StartSearchEngine,
				new SearchTaskArgs
				{
					searchEngine = se,
					query = query,
					barrier = barrier,
					responses = responses,
				}));
			await Task.WhenAll(tasks);

			// TODO: Push to DB

			if (responses[0] == null)
			{
				return new List<SearchResultItem>();
			}
			var result = responses[0].Item2.ParseData(responses[0].Item1);
			return result;
		}

		private static void StartSearchEngine(object args)
		{
			if (args is SearchTaskArgs searchTaskArgs)
			{
				searchTaskArgs.barrier.SignalAndWait();
				var response = searchTaskArgs.searchEngine.GetData(searchTaskArgs.query);

				lock (searchTaskArgs.barrier)
				{
					searchTaskArgs.responses.Add(Tuple.Create(response, searchTaskArgs.searchEngine));
				}
			}
		}
	}

	class SearchTaskArgs
	{
		public ISearchEngine searchEngine;
		public string query;
		public Barrier barrier;
		public List<Tuple<string, ISearchEngine>> responses;
	}
}
