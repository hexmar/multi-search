using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MultiSearch.Domain.Models.SearchEngines
{
	public class YandexSearch : ISearchEngine
	{
		public async Task<string> GetDataAsync(string query)
		{
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add(
				"user-agent",
				"Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:73.0) Gecko/20100101 Firefox/73.0");
			var response = await client.GetAsync(
				"https://yandex.ru/search/?text=" + HttpUtility.UrlEncode(query));

			var content = await response.Content.ReadAsStringAsync();
			return content;
		}

		public IEnumerable<SearchResultItem> ParseData(string data)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(data);
			var results = htmlDoc.DocumentNode
				.SelectNodes(".//a[@class=\"link link_theme_normal organic__url link_cropped_no i-bem\"]");

			if (results == null)
			{
				return new List<SearchResultItem>();
			}

			var mappedResults = results.Select((result, index) =>
			{
				var link = result.Attributes["href"].Value;

				var titleElement = result.SelectSingleNode("./*[@class=\"organic__url-text\"]");
				var title = HttpUtility.HtmlDecode(titleElement.InnerText);

				return new SearchResultItem
				{
					ServerLink = "https://yandex.ru/",
					Position = index,
					Title = title,
					Link = link,
				};
			}).ToList();

			return mappedResults;
		}
	}
}
