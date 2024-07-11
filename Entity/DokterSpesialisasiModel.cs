
namespace Entity
{
    public class DokterSpesialisasiModel
    {
        public int dokter_id {  get; set; }
        public DokterModel Dokter { get; set; }
        
        public int spesialisasi_id { get; set; }
        public SpesialisasiModel Spesialisasi { get; set; }


    }
}
