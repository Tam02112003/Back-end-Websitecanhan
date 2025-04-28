using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Websitecanhan.Models;

namespace Websitecanhan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CertificatesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        {
            return await _context.Certificates.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {
            try
            {
                var certificate = await _context.Certificates.FindAsync(id);
                if (certificate == null) return NotFound();
                return certificate;

            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        public async Task<ActionResult<Certificate>> PostCertificate(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCertificates), new { id = certificate.Id }, certificate);
        }

        [HttpPut]
        public async Task<IActionResult> PutCertificate([FromHeader(Name = "X-Project-ID")] int id, [FromBody] Certificate certificate)
        {
            // Kiểm tra xem bản ghi có tồn tại không
            var existingCertificate = await _context.Certificates.FindAsync(id);
            if (existingCertificate == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy bản ghi
            }

            // Kiểm tra xác thực mô hình
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Cập nhật các thuộc tính của certificate
            existingCertificate.Name = certificate.Name;
            existingCertificate.Year = certificate.Year;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "An error occurred while updating the certificate."); // Trả về lỗi 500 nếu có lỗi
            }

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
