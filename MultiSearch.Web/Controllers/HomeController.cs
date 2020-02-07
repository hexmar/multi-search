using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiSearch.Domain.Services;
using MultiSearch.Web.Models;

namespace MultiSearch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISearchService _searchService;

        public HomeController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new IndexViewModel());
            }

            var result = await _searchService.SearchAsync(query);
            var model = new SearchResultsViewModel
            {
                SearchResults = result,
                Query = query,
                SearchEngine = result.FirstOrDefault()?.ServerLink,
            };

            return View("~/Views/Home/SearchResults.cshtml", model);
        }

        public async Task<IActionResult> Local(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("~/Views/Home/Index.cshtml", new IndexViewModel
                {
                    LocalSearch = true,
                });
            }

            var result = _searchService.SearchLocal(query);
            var model = new SearchResultsViewModel
            {
                SearchResults = result,
                Query = query,
                LocalSearch = true,
            };

            return View("~/Views/Home/SearchResults.cshtml", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
