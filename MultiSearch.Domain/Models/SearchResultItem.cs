namespace MultiSearch.Domain.Models
{
	public class SearchResultItem
	{
		public int Id { get; set; }
		public string ServerLink { get; set; }
		public string Request { get; set; }
		public int Position { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
	}
}
