using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MultiSearch.Domain.Models.SearchEngines
{
	public class BingSearch : ISearchEngine
	{
		public async Task<string> GetDataAsync(string query)
		{
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add(
				"user-agent",
				"Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");
			// Bing gives search results only after setting cookies.
			// Because of this, two requests are executed.
			await client.GetAsync("https://www.bing.com/search?q=" + HttpUtility.UrlEncode(query));
			var response = await client.GetAsync(
				"https://www.bing.com/search?q=" + HttpUtility.UrlEncode(query));

			var content = await response.Content.ReadAsStringAsync();
			return content;
		}

		public IEnumerable<SearchResultItem> ParseData(string data)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(data);
			var nodes = htmlDoc.DocumentNode.SelectNodes(".//li[@class=\"b_algo\"]//h2/a");

			if (nodes == null)
			{
				return new List<SearchResultItem>();
			}

			var mappedResults = nodes.Select((node, index) =>
			{
				var link = node.Attributes["href"].Value;

				var title = HttpUtility.HtmlDecode(node.InnerText);

				return new SearchResultItem
				{
					ServerLink = "https://www.bing.com/",
					Position = index,
					Title = title,
					Link = link,
				};
			}).ToList();

			return mappedResults;
		}
	}
}
