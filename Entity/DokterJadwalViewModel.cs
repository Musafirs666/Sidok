namespace Entity
{
    public class DokterJadwalViewModel
    {
        public DokterModel Dokter { get; set; }
        public IEnumerable<PoliModel> PoliList { get; set; }
        public int SelectedPoliId { get; set; }
        public string SelectedHari { get; set; }
        public List<BertugasDiModel> JadwalDokter { get; set; }
    }
}
