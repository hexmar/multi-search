using MultiSearch.Domain.Models;
using MultiSearch.Domain.Models.SearchEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiSearch.Domain.Services
{
	public class SearchService : ISearchService
	{
		private readonly ISearchResultRepository _repository;

		public SearchService(ISearchResultRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<SearchResultItem>> SearchAsync(string query)
		{
			var searches = new List<ISearchEngine>()
			{
				new GoogleSearch(),
				new YandexSearch(),
				new BingSearch(),
			};

			var responses = new List<Tuple<string, ISearchEngine>>(searches.Count);
			using var barrier = new Barrier(searches.Count);
			using var finalBarrier = new Barrier(2);
			{
				var threads = searches.Select(se => new
				{
					thread = new Thread(StartSearchEngine),
					args = new SearchTaskArgs
					{
						searchEngine = se,
						query = query,
						barrier = barrier,
						responses = responses,
						finalBarrier = finalBarrier,
					},
				});

				foreach (var thread in threads)
				{
					thread.thread.Start(thread.args);
				}

				// Don't know why but thread works and has state Unstarted so this isn't work
				/*foreach (var thread in threads)
				{
					thread.thread.Join();
				}*/

				finalBarrier.SignalAndWait();
			}

			if (responses[0] == null)
			{
				return new List<SearchResultItem>();
			}
			var result = responses[0].Item2.ParseData(responses[0].Item1);

			if (result.Count() > 10)
			{
				result = result.Take(10);
			}
			foreach (var searchResult in result)
			{
				searchResult.Request = query;
			}

			result = await _repository.InsertResultsAsync(result);

			return result;
		}

		public IEnumerable<SearchResultItem> SearchLocal(string query)
		{
			var results = _repository.GetAll().Where(result => result.Title.Contains(query)).ToList();
			var reducedResults = results.Aggregate(new List<SearchResultItem>(), (reduced, result) =>
			{
				if (reduced.All(r => r.Link != result.Link))
				{
					reduced.Add(result);
				}

				return reduced;
			}).OrderBy(r => r.Position).ToList();

			return reducedResults;
		}

		private static void StartSearchEngine(object args)
		{
			if (args is SearchTaskArgs searchTaskArgs)
			{
				searchTaskArgs.barrier.SignalAndWait();
				var task = searchTaskArgs.searchEngine.GetDataAsync(searchTaskArgs.query);
				task.Wait();
				var response = task.Result;

				lock (searchTaskArgs.barrier)
				{
					searchTaskArgs.responses.Add(Tuple.Create(response, searchTaskArgs.searchEngine));

					if (searchTaskArgs.responses.Count == 1)
					{
						searchTaskArgs.finalBarrier.SignalAndWait();
					}
				}
			}
		}
	}

	class SearchTaskArgs
	{
		public ISearchEngine searchEngine;
		public string query;
		public Barrier barrier;
		public Barrier finalBarrier;
		public List<Tuple<string, ISearchEngine>> responses;
	}
}
