using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System.Threading.Tasks;

namespace Sidok.Controllers
{
    public class SearchController : Controller
    {
        private readonly IOpensearchDokterService _opensearchDokterService;

        public SearchController(IOpensearchDokterService opensearchDokterService)
        {
            _opensearchDokterService = opensearchDokterService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return View();
            }

            var searchResults = await _opensearchDokterService.SearchDokterAsync(searchTerm);
            return View(searchResults);
        }
    }
}
