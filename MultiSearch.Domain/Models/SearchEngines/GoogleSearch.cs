using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace MultiSearch.Domain.Models.SearchEngines
{
	public class GoogleSearch : ISearchEngine
	{
		public async Task<string> GetDataAsync(string query)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");
			var response = await client.GetAsync("https://google.ru/search?q=" + HttpUtility.UrlEncode(query));

			var content = await response.Content.ReadAsStringAsync();
			return content;
		}

		public IEnumerable<SearchResultItem> ParseData(string data)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(data);
			var results = htmlDoc.DocumentNode.SelectNodes(".//*[@class=\"rc\"]").ToList();
			
			var mappedResults = results.Select((result, index) =>
			{
				var linkElement = result.SelectSingleNode(".//*[@class=\"r\"]/a");
				var link = linkElement.Attributes["href"].Value;

				var titleElement = linkElement.SelectSingleNode(".//h3");
				var title = titleElement.InnerText;

				var descriptionElement = result.SelectSingleNode(".//*[@class=\"st\"]");

				return new SearchResultItem
				{
					ServerLink = "https://google.ru/",
					Position = index,
					Title = title,
					Link = link,
				};
			}).ToList();

			return mappedResults;
		}
	}
}
