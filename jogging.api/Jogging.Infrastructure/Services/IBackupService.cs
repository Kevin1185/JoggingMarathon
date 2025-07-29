using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;


namespace Jogging.Infrastructure.Services {
    /// <summary>
    /// Veritabanı ve asset dosyalarını yedekleme ve geri yükleme işlevlerini tanımlar.
    /// </summary>
    public interface IBackupService {
        /// <summary>
        /// Veritabanını .sql dosyasına dump eder,
        /// wwwroot altındaki asset klasörlerini geçici klasöre kopyalar,
        /// ardından geçici klasörü ZIP olarak paketler ve ZIP dosyasının sunucudaki yolunu döner.
        /// </summary>
        /// <returns>Sunucuda oluşturulan ZIP dosyasının tam yolu.</returns>
        Task<string> CreateBackupZipAsync();

        /// <summary>
        /// Gelen ZIP dosyasını geçici klasöre açar,
        /// içindeki SQL dump’ı veritabanına import eder (drop & create),
        /// wwwroot altındaki asset klasörlerini silip yedekten geri yükler.
        /// </summary>
        /// <param name="uploadedZip">IFormFile olarak gelen ZIP dosyası.</param>
        Task RestoreFromBackupZipAsync(IFormFile uploadedZip);
    }
}
