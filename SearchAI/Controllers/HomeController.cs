using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.Search;
using SearchAI.Models;
using System.Diagnostics;

namespace SearchAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string SearchServiceName = "mozart-search";
        public string SearchServiceAdminAPIKey = "IMnLFq6QSrpbgoVSIcQ8bfrN9NwunX8sFwqOnNAcGGAzSeBHUQvB";
        public string indexName = "azureblob-index"; // Pegar em indexes na azure

        public IActionResult Search(string searchData)
        {
            if (String.IsNullOrEmpty(searchData))
                searchData = "*";

            SearchServiceClient serviceClient = new SearchServiceClient(SearchServiceName, new SearchCredentials(SearchServiceAdminAPIKey));

            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient(indexName);

            SearchParameters searchParameters = new SearchParameters();

            searchParameters.HighlightFields = new List<string> { "merged_content" };
            searchParameters.HighlightPreTag = "<b>";
            searchParameters.HighlightPostTag = "</b>";

            var results = indexClient.Documents.SearchAsync(searchData, searchParameters).Result;

            List<SearchResultViewModel> searchResults = new List<SearchResultViewModel>();

            foreach (var data in results.Results)
            {
                SearchResultViewModel currentData = new SearchResultViewModel();
                currentData.fileName = data.Document["metadata_storage_name"].ToString();

                var path = data.Document["metadata_storage_path"].ToString();
                path = path.Substring(0, path.Length - 1);

                var byteData = WebEncoders.Base64UrlDecode(path);
                currentData.filePath = System.Text.ASCIIEncoding.ASCII.GetString(byteData);

                currentData.fileText = data.Document["merged_content"].ToString();

                if (data.Highlights != null)
                {
                    foreach (var high in data.Highlights["merged_content"].ToList())
                    {
                        currentData.highlightedText += high;
                    }
                }

                searchResults.Add(currentData);
            }

            ViewBag.SearchData = searchResults;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
