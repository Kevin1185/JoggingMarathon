using Microsoft.AspNetCore.Http;

namespace Jogging.Rest.DTOs {
    /// <summary>
    /// DTO gebruikt voor binding met de UploadBackup-methode
    /// om een ZIP-bestand te ontvangen.
    /// </summary>
    public class BackupUploadDto {
        /// <summary>
        /// IFormFile-type dat wordt verzonden als "file" in form-data.
        /// </summary>
        public IFormFile File { get; set; } = default!;
    }
}
