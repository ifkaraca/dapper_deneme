using System.ComponentModel.DataAnnotations;

namespace dapveri.Models
{
    public class KullaniciDto
    {
        [Required, MaxLength(100)]
        public string kullanici_adi { get; set; } = "";
        [Required, MaxLength(100)]
        public string kullanici_soyad { get; set; } = "";
        [Required, MaxLength(100)]
        public string kullanici_tlf { get; set; } = "";
        [Required, MaxLength(100)]
        public string kullanici_sehir { get; set; } = "";
    }
}
