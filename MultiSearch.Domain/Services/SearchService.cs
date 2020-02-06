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
		/*
		 * Gets all ISearchEngine
		 * Creates threads for each
		 * Creates Barrier
		 * Starts them
		 * ???
		 * First return response result with ISearchEngine
		 * Parse response with ISearchEngine
		 * Result should be List<SearchResultItem>
		 * Put results to DB
		 * Return results
		 */

		public async Task<IEnumerable<SearchResultItem>> Search(string query)
		{
			var searches = new List<ISearchEngine>()
			{
				new GoogleSearch(),
				
			};

			var responses = new List<Tuple<string, ISearchEngine>>(searches.Count);
			using var barrier = new Barrier(searches.Count);
			using var fianlBarrier = new Barrier(2);
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
						finalBarrier = fianlBarrier,
					},
				});

				foreach (var thread in threads)
				{
					thread.thread.Start(thread.args);
				}

				fianlBarrier.SignalAndWait();

				// Don't know why but thread is working and has state Unstarted
				// so this isn't work
				/*foreach (var thread in threads)
				{
					thread.thread.Join();
				}*/
			}

			if (responses[0] == null)
			{
				return new List<SearchResultItem>();
			}
			var result = responses[0].Item2.ParseData(responses[0].Item1);

			// TODO: Push to DB

			return result;
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
