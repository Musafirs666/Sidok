using Microsoft.AspNetCore.Mvc;
using Entity;
using Service.Interfaces;
using System.Threading.Tasks;
using Service;

namespace Sidok.Controllers
{
    public class PoliController : Controller
    {
        private readonly IPoliService _poliService;

        public PoliController(IPoliService poliService)
        {
            _poliService = poliService;
        }

        public async Task<IActionResult> Index()
        {
            var poliList = await _poliService.GetAllAsync();
            return View(poliList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PoliModel poli)
        {
            if (ModelState.IsValid)
            {
                await _poliService.CreateAsync(poli);
                return RedirectToAction(nameof(Index));
            }
            return View(poli);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var poli = await _poliService.GetByIdAsync(id);
            if (poli == null)
            {
                return NotFound();
            }
            return View(poli);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PoliModel poli)
        {
            if (id != poli.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _poliService.UpdateAsync(poli);
                return RedirectToAction(nameof(Index));
            }
            return View(poli);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var poli = await _poliService.GetByIdAsync(id);
            if (poli == null)
            {
                return NotFound();
            }
            return View(poli);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _poliService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var poli = await _poliService.GetByIdAsync(id);
            if (poli == null)
            {
                return NotFound();
            }
            return View(poli);
        }
    }
}
