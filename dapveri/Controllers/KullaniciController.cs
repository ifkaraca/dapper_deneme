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
    public class KullaniciController : ControllerBase
    {
        private readonly string conncetionString;
        public KullaniciController(IConfiguration configuration)
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

                    string sql = "select * from kullanici_tbl";
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

                    string sql = "select * from kullanici_tbl where id=@id";
                    var kullanici = connection.QuerySingle<LiKullanici>(sql, new {id = id});
                    if(kullanici !=null)
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

                    string sql = "insert into kullanici_tbl" +
                        "(kullanici_adi,kullanici_soyad,kullanici_tlf,kullanici_sehir)" +
                        "OUTPUT INSERTED.*" +
                        "values (@kullanici_adi,@kullanici_soyad,@kullanici_tlf,@kullanici_sehir)";

                    var kullanici = new LiKullanici()
                    {
                        kullanici_adi = kullaniciDto.kullanici_adi,
                        kullanici_soyad = kullaniciDto.kullanici_soyad,
                        kullanici_tlf = kullaniciDto.kullanici_tlf,
                        kullanici_sehir = kullaniciDto.kullanici_sehir,
                    };

                    var newliKullanici = connection.QuerySingleOrDefault<LiKullanici>(sql, kullanici);
                    if (newliKullanici != null)
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
        public IActionResult Guncelle(int id,KullaniciDto kullaniciDto)
        {
            try
            {
                using (var connection = new SqlConnection(conncetionString))
                {
                    connection.Open();

                    string sql = "Update kullanici_tbl set kullanici_adi=@kullanici_adi, kullanici_soyad=@kullanici_soyad, kullanici_tlf=@kullanici_tlf, " +
                        "kullanici_sehir=@kullanici_sehir where id=@id";

                    var kullanici = new LiKullanici()
                    {
                        id = id,
                        kullanici_adi = kullaniciDto.kullanici_adi,
                        kullanici_soyad = kullaniciDto.kullanici_soyad,
                        kullanici_tlf = kullaniciDto.kullanici_tlf,
                        kullanici_sehir = kullaniciDto.kullanici_sehir,
                    };

                    int count = connection.Execute(sql, kullanici);
                    if (count < 1)
                    {
                        return NotFound();
                    }
                    return KullaniciListeleme(id);
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

                    string sql = "delete from kullanici_tbl where id=@id";
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
