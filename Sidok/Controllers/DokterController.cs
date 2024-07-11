using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Entity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Service;


namespace Sidok.Controllers
{
    public class DokterController : Controller
    {
        private readonly IDokterService _dokterService;
        private readonly ISpesialisasiService _spesialisasiService; // Add this field
        private readonly IPoliService _poliService; // Add this field

        public DokterController(IDokterService dokterService, ISpesialisasiService spesialisasiService, IPoliService poliService)
        {
            _dokterService = dokterService;
            _spesialisasiService = spesialisasiService;
            _poliService = poliService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateJadwal(int id)
        {
            // Mengambil data dokter berdasarkan ID
            var dokter = await _dokterService.GetByIdAsync(id);
            if (dokter == null)
            {
                return NotFound();
            }

            // Mengambil daftar poli
            var poliList = await _poliService.GetAllAsync();
            if (poliList == null)
            {
                return NotFound("Daftar poli tidak ditemukan.");
            }

            // Membuat ViewModel untuk mengirimkan data ke View
            var viewModel = new DokterJadwalViewModel
            {
                Dokter = dokter,
                PoliList = poliList
            };

            // Mengambil daftar jadwal dokter berdasarkan dokterId
            viewModel.JadwalDokter = await _dokterService.GetJadwalListAsync(id);

            // Membuat SelectList untuk daftar poli
            ViewBag.PoliList = new SelectList(viewModel.PoliList, "id", "nama_poli");

            return View(viewModel);
        }


        public async Task<IActionResult> Index()
        {
            // Retrieve all dokter asynchronously
            var dokterList = await _dokterService.GetAllAsync();

            // Pass the dokterList to the view
            return View(dokterList);
        }

        public IActionResult Create()
        {
            var spesialisasiListTask = _spesialisasiService.GetAllAsync();
            spesialisasiListTask.Wait();

            var spesialisasiList = spesialisasiListTask.Result;

            ViewBag.SpesialisasiList = new SelectList(spesialisasiList, "id", "nama"); 

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(DokterModel dokter)
        {
           
                await _dokterService.AddAsync(dokter);
                return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> Details(int id)
        {
            var dokter = await _dokterService.GetByIdAsync(id);
            if (dokter == null)
            {
                return NotFound();
            }
            return View(dokter);
        }

        public IActionResult Edit(int id)
        {
            var dokterTask = _dokterService.GetByIdAsync(id);
            var spesialisasiListTask = _spesialisasiService.GetAllAsync();
            var dokterSpesialisasiTask = _dokterService.GetSpesialisasiByDokterIdAsync(id);

            // Menunggu ketiga task selesai
            Task.WaitAll(dokterTask, spesialisasiListTask, dokterSpesialisasiTask);

            var dokter = dokterTask.Result;
            if (dokter == null)
            {
                return NotFound();
            }

            var spesialisasiList = spesialisasiListTask.Result;
            var dokterSpesialisasi = dokterSpesialisasiTask.Result.Select(ds => ds.id).ToList();

            // Menggunakan SelectList dengan selectedValue
            ViewBag.SpesialisasiList = new SelectList(spesialisasiList, "id", "nama", dokterSpesialisasi.FirstOrDefault());

            return View(dokter);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DokterModel dokter)
        {
            if (id != dokter.Id)
            {
                return NotFound();
            }

                try
                {
                    await _dokterService.UpdateAsync(dokter);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DokterExists(dokter.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));

            // Jika ModelState tidak valid, kembalikan View dengan data yang diperlukan
            var spesialisasiList = await _spesialisasiService.GetAllAsync();
            ViewBag.SpesialisasiList = new SelectList(spesialisasiList, "id", "nama");

            return View(dokter);
        }

        private async Task<bool> DokterExists(int id)
        {
            var dokter = await _dokterService.GetByIdAsync(id);
            return dokter != null;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dokter = await _dokterService.GetByIdAsync(id);
            if (dokter == null)
            {
                return NotFound();
            }

            return View(dokter);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dokterService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> CreateJadwal(DokterJadwalViewModel model)
        {
            
                try
                {
                    var bertugasDiModel = new BertugasDiModel
                    {
                        DokterId = model.Dokter.Id,
                        PoliId = model.SelectedPoliId,
                        Hari = model.SelectedHari
                    };

                    // Simpan ke repository bertugasDi
                    await _dokterService.AddJadwalAsync(bertugasDiModel);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Handle exception if needed
                    ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                }
            
            // Kembalikan view dengan model yang sama
            return View(model);
        }



    }
}
