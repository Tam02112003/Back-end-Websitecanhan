using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Websitecanhan.Models;

namespace Websitecanhan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly IDynamoDBContext _context;

        public CertificatesController(IDynamoDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Certificate>>> GetCertificates()
        {
            var conditions = new List<ScanCondition>();
            var certificates = await _context.ScanAsync<Certificate>(conditions).GetRemainingAsync();
            return certificates;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Certificate>> GetCertificate(int id)
        {
            try
            {
                var certificate = await _context.LoadAsync<Certificate>(id);
                if (certificate == null) return NotFound();
                return certificate;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Certificate>> PostCertificate(Certificate certificate)
        {
            await _context.SaveAsync(certificate);
            return CreatedAtAction(nameof(GetCertificate), new { id = certificate.Id }, certificate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCertificate(int id, Certificate certificate)
        {
            if (id != certificate.Id)
            {
                return BadRequest();
            }

            var existingCertificate = await _context.LoadAsync<Certificate>(id);
            if (existingCertificate == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _context.SaveAsync(certificate);
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the certificate.");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var certificate = await _context.LoadAsync<Certificate>(id);
            if (certificate == null) return NotFound();

            await _context.DeleteAsync(certificate);
            return NoContent();
        }
    }
}