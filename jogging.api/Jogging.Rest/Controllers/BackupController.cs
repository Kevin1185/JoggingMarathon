using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jogging.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System.IO;
using Jogging.Rest.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jogging.Rest.Controllers {
    [ApiController]
    [Route("api/admin/backup")]
    //[Authorize(Roles = "Admin")]  
    // Normaal zou je hier de [Authorize]-attribuut actief laten,
    // zodat alleen gebruikers met rol "Admin" toegang krijgen:
    // [Authorize(Roles = "Admin")]
    public class BackupController : ControllerBase {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IBackupService backupService, ILogger<BackupController> logger) {
            _backupService = backupService;
            _logger = logger;
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadBackup() {
            try {
                var zipPath = await _backupService.CreateBackupZipAsync();
                if (!System.IO.File.Exists(zipPath))
                    return NotFound("Backup-bestand kon niet worden gemaakt of gevonden.");

                var zipBytes = await System.IO.File.ReadAllBytesAsync(zipPath);
                var fileName = Path.GetFileName(zipPath);
                return File(zipBytes, "application/zip", fileName);
            } catch (System.Exception ex) {
                _logger.LogError(ex, "Er is een fout opgetreden tijdens het maken van de backup.");
                return StatusCode(500, $"Backup mislukt: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        // Voor Swagger en correcte binding bij het uploaden van een bestand,
        // gebruiken we een DTO (BackupUploadDto) in plaats van direct IFormFile.
        // Normaal (in productie) ziet dit er zo uit:
        // public async Task<IActionResult> UploadBackup([FromForm] BackupUploadDto dto)
        // en in de method body: dto.File
        //
        // Voor lokale tests kun je eventueel tijdelijk rechtstreeks IFormFile blijven gebruiken,
        // maar dan moet Swagger de parameter goed inlezen. Daarom is de DTO-variant in dit voorbeeld geadviseerd.
        public async Task<IActionResult> UploadBackup([FromForm] BackupUploadDto dto) {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Er is geen zip-bestand gekozen of het bestand is leeg.");

            try {
                await _backupService.RestoreFromBackupZipAsync(dto.File);
                return Ok("Restore is succesvol voltooid.");
            } catch (System.Exception ex) {
                _logger.LogError(ex, "Er is een fout opgetreden tijdens het herstellen van de backup.");
                return StatusCode(500, $"Restore mislukt: {ex.Message}");
            }
        }
    }
}
