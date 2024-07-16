using Dapper;
using dapveri.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace dapveri.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcController : ControllerBase
    {
        private readonly string conncetionString;
        public ProcController(IConfiguration configuration)
        {
            conncetionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet]
        public IActionResult KullaniciListeleme()
        {
            List<LiKullanici> kullanicis = new List<LiKullanici>();

            try
            {
                using (var connection = new SqlConnection(conncetionString))
                {
                    connection.Open();

                    string sql = "exec TumGetir";
                    var data = connection.Query<LiKullanici>(sql);
                    kullanicis = data.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bir Hata Oldu: \n" + ex.Message);
                return BadRequest();
            }
            return Ok(kullanicis);
        }

        [HttpGet("{id}")]
        public IActionResult KullaniciListeleme(int id)
        {
            List<LiKullanici> kullanicis = new List<LiKullanici>();
            try
            {
                using (var connection = new SqlConnection(conncetionString))
                {
                    connection.Open();

                    string sql = "exec GetirId @id";
                    var kullanici = connection.QuerySingle<LiKullanici>(sql, new { id = id });
                    if (kullanici != null)
                    {
                        return Ok(kullanici);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bir Hata Oldu: \n" + ex.Message);
                return BadRequest();

            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult Create(KullaniciDto kullaniciDto)
        {

            try
            {
                using (var connection = new SqlConnection(conncetionString))
                {
                    connection.Open();

                    string sql = "exec InsertKullanici  @kullanici_adi,@kullanici_soyad,@kullanici_tlf,@kullanici_sehir";

                    var kullanici = new LiKullanici()
                    {
                        kullanici_adi = kullaniciDto.kullanici_adi,
                        kullanici_soyad = kullaniciDto.kullanici_soyad,
                        kullanici_tlf = kullaniciDto.kullanici_tlf,
                        kullanici_sehir = kullaniciDto.kullanici_sehir,
                    };

                    
            int rowsAffected = connection.Execute(sql, kullanici);
            if (rowsAffected > 0)
            {
                return Ok();
            }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bir Hata Oldu: \n" + ex.Message);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public IActionResult Guncelle(int id, KullaniciDto kullaniciDto)
        {
            try
            {
                using (var connection = new SqlConnection(conncetionString))
                {
                    connection.Open();

                    string sql = "exec DuzenleTum @kullanici_id, @kullanici_adi, @kullanici_soyad, @kullanici_tlf, @kullanici_sehir";

                    var parameters = new
                    {
                        kullanici_id = id,
                        kullanici_adi = kullaniciDto.kullanici_adi,
                        kullanici_soyad = kullaniciDto.kullanici_soyad,
                        kullanici_tlf = kullaniciDto.kullanici_tlf,
                        kullanici_sehir = kullaniciDto.kullanici_sehir,
                    };

                    int count = connection.Execute(sql, parameters);
                    if (count < 1)
                    {
                        return NotFound();
                    }
                    var updatedUser = connection.QuerySingle<LiKullanici>("exec GetirId @id", new { id });
                    return Ok(updatedUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bir Hata Oldu: \n" + ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteKullanici(int id)
        {
            try
            {
                using (var connection = new SqlConnection(conncetionString))
                {
                    connection.Open();

                    string sql = "exec TumSil @id";
                    int count = connection.Execute(sql, new { id = id });
                    if (count < 1)
                    {
                        return NotFound();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Bir Hata Oldu: \n" + ex.Message);
                return BadRequest();
            }

            return Ok();
        }
    }
}
