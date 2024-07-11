namespace Entity
{
    public class DokterModel
    {
        public int Id { get; set; }
        public string Nip { get; set; }
        public string Nik { get; set; }
        public string Nama { get; set; }
        public DateTime tanggal_lahir { get; set; }
        public string jenis_kelamin { get; set; }
        public string tempat_lahir { get; set; }
        public List<int> Spesialisasi { get; set; }

        public List<string> SpesialisasiData { get; set; }

        public string JenisKelaminDeskripsi => jenis_kelamin == "L" ? "Laki-Laki" : "Perempuan";

    }
}
