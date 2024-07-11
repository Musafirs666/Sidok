using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Service;

namespace Sidok.Controllers
{
    public class DokterPoliController : Controller
    {
        private readonly IDokterService _dokterService;
        private readonly IPoliService _poliService;
        private readonly IDokterPoliService _dokterPoliService;

        public DokterPoliController(IDokterService dokterService, IDokterPoliService dokterPoliService, IPoliService poliService)
        {
            _dokterService = dokterService;
            _dokterPoliService = dokterPoliService;
            _poliService = poliService;
        }

        public async Task<IActionResult> Index()
        {
            var poliList = await _poliService.GetAllAsync();
            ViewBag.PoliList = new SelectList(poliList, "id", "nama_poli");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDokterByPoliId(int poliId)
        {
            var dokterList = await _dokterPoliService.GetDokterByPoliAsync(poliId);
            if (dokterList == null)
            {
                return NotFound();
            }

            ViewBag.DokterList = dokterList;
            var poliList = await _poliService.GetAllAsync();
            ViewBag.PoliList = new SelectList(poliList, "id", "nama_poli");

            return View("Index", new Entity.DokterByPoliViewModel { PoliId = poliId, PoliList = poliList });
        }
    }
}
