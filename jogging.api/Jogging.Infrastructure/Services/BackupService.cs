using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using MySqlConnector;

namespace Jogging.Infrastructure.Services {
    /// <summary>
    /// Klasse die IBackupService implementeert.
    /// - CreateBackupZipAsync(): Database dump + assets kopiëren + ZIP maken
    /// - RestoreFromBackupZipAsync(): ZIP uitpakken + SQL importeren + assets terugzetten
    /// </summary>
    public class BackupService : IBackupService {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<BackupService> _logger;

        // Waarden uit appsettings.json → "BackupSettings"
        private readonly string _tempFolderName;   // Bijvoorbeeld: "BackupTemp"
        private readonly string _mysqldumpPath;    // Bijvoorbeeld: "mysqldump"
        private readonly string _mysqlPath;        // Bijvoorbeeld: "mysql"
        private readonly string _connectionString; // Bijvoorbeeld: "server=...;database=...;user=...;password=..."

        public BackupService(
            IConfiguration configuration,
            IWebHostEnvironment env,
            ILogger<BackupService> logger) {
            _configuration = configuration;
            _env = env;
            _logger = logger;

            // Lees BackupSettings uit appsettings.json
            var backupSection = _configuration.GetSection("BackupSettings");
            _tempFolderName = backupSection.GetValue<string>("TempFolder") ?? "BackupTemp";
            _mysqldumpPath = backupSection.GetValue<string>("MySqlDumpExePath") ?? "mysqldump";
            _mysqlPath = backupSection.GetValue<string>("MySqlExePath") ?? "mysql";

            // Haal de connection string (DefaultConnection) op
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// 1) Verwijder of maak de tijdelijke map aan
        /// 2) Maak een database dump via mysqldump
        /// 3) Kopieer asset-mappen uit wwwroot naar de temp-map
        /// 4) Maak een ZIP van de temp-map
        /// </summary>
        public async Task<string> CreateBackupZipAsync() {
            // 1) Bepaal het volledige pad van de tijdelijke map
            var tempRoot = Path.Combine(_env.ContentRootPath, _tempFolderName);
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
            Directory.CreateDirectory(tempRoot);

            // 2) Database dump maken (mysqldump)
            var csBuilder = new MySqlConnectionStringBuilder(_connectionString);
            var host = csBuilder.Server;
            var port = csBuilder.Port.ToString();
            var dbName = csBuilder.Database;
            var user = csBuilder.UserID;
            var pwd = csBuilder.Password;
            var timestamp = DateTime.Now.ToString("dd_MM_yyyy_HHmmss", CultureInfo.InvariantCulture);
            
            var dumpFileName = $"backup_{timestamp}.sql";
            var dumpFilePath = Path.Combine(tempRoot, dumpFileName);

            // mysqldump-commando: bijv. "mysqldump -hhost -Pport -uuser -ppwd dbName"
            var dumpArguments = $"-h{host} -P{port} -u{user} -p{pwd} {dbName}";
            var dumpInfo = new ProcessStartInfo {
                FileName = _mysqldumpPath,
                Arguments = dumpArguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = dumpInfo }) {
                _logger.LogInformation("mysqldump wordt gestart: {args}", dumpArguments);
                process.Start();

                // Schrijf de stdout-uitvoer naar het dumpbestand
                await using (var fs = new FileStream(dumpFilePath, FileMode.Create, FileAccess.Write)) {
                    await process.StandardOutput.BaseStream.CopyToAsync(fs);
                }

                var stderr = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();
                if (process.ExitCode != 0) {
                    _logger.LogError("mysqldump faalde (ExitCode={code}): {err}", process.ExitCode, stderr);
                    throw new Exception($"mysqldump is mislukt: {stderr}");
                }
                _logger.LogInformation("mysqldump succesvol, dumpbestand: {path}", dumpFilePath);
            }

            // 3) Kopieer asset-mappen (wwwroot/images, wwwroot/icons, wwwroot/uploads)
            var wwwroot = Path.Combine(_env.ContentRootPath, "wwwroot");
            var assetFolders = new[] { "images", "icons", "uploads" };
            foreach (var folderName in assetFolders) {
                var sourcePath = Path.Combine(wwwroot, folderName);
                if (!Directory.Exists(sourcePath)) {
                    _logger.LogWarning("Asset-map niet gevonden, sla over: {folder}", sourcePath);
                    continue;
                }
                var destPath = Path.Combine(tempRoot, folderName);
                CopyDirectoryRecursively(sourcePath, destPath);
                _logger.LogInformation("Asset gekopieerd: {src} → {dest}", sourcePath, destPath);
            }

            // 4) Maak van de temp-map een ZIP-bestand
            var zipFileName = $"backup_{timestamp}.zip";
            var zipFilePath = Path.Combine(_env.ContentRootPath, zipFileName);
            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);

            ZipFile.CreateFromDirectory(tempRoot, zipFilePath, CompressionLevel.Optimal, includeBaseDirectory: false);
            _logger.LogInformation("ZIP bestand aangemaakt: {zip}", zipFilePath);

            return zipFilePath;
        }

        /// <summary>
        /// 1) Sla het geüploade ZIP-bestand op en pak het uit
        /// 2) Zoek het .sql-bestand, drop/create de database
        /// 3) Importeer de SQL
        /// 4) Vervang asset-mappen met die uit de ZIP
        /// 5) Verwijder de tijdelijke map
        /// </summary>
        public async Task RestoreFromBackupZipAsync(IFormFile uploadedZip) {
            if (uploadedZip == null || uploadedZip.Length == 0)
                throw new ArgumentException("ZIP-bestand mag niet null of leeg zijn.");

            // 1) Maak of maak de temp-map schoon
            var tempRoot = Path.Combine(_env.ContentRootPath, _tempFolderName);
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
            Directory.CreateDirectory(tempRoot);

            // 2) Sla de geüploade ZIP op in de temp-map
            var tempZipPath = Path.Combine(tempRoot, "uploaded_backup.zip");
            await using (var fs = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write)) {
                await uploadedZip.CopyToAsync(fs);
            }
            _logger.LogInformation("Geüploade ZIP opgeslagen: {zip}", tempZipPath);

            // 3) Pak de ZIP uit
            ZipFile.ExtractToDirectory(tempZipPath, tempRoot);
            _logger.LogInformation("ZIP uitgepakt: {zip} → {dir}", tempZipPath, tempRoot);

            // 4) Zoek het .sql-bestand
            var sqlFiles = Directory.GetFiles(tempRoot, "*.sql", SearchOption.TopDirectoryOnly);
            if (sqlFiles.Length == 0)
                throw new Exception("Er is geen .sql-bestand gevonden in de ZIP.");
            var sqlFilePath = sqlFiles[0];
            _logger.LogInformation("SQL-bestand gevonden voor restore: {sql}", sqlFilePath);

            // 5) Drop & create de database
            var csBuilder = new MySqlConnectionStringBuilder(_connectionString);
            var host = csBuilder.Server;
            var port = csBuilder.Port.ToString();
            var dbName = csBuilder.Database;
            var user = csBuilder.UserID;
            var pwd = csBuilder.Password;

            var dropCreateArgs =
                $"-h{host} -P{port} -u{user} -p{pwd} -e " +
                $"\"DROP DATABASE IF EXISTS `{dbName}`; CREATE DATABASE `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;\"";
            var dropCreateInfo = new ProcessStartInfo {
                FileName = _mysqlPath,
                Arguments = dropCreateArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var proc = new Process { StartInfo = dropCreateInfo }) {
                _logger.LogInformation("Database wordt opnieuw aangemaakt: {args}", dropCreateArgs);
                proc.Start();
                var err = await proc.StandardError.ReadToEndAsync();
                proc.WaitForExit();
                if (proc.ExitCode != 0) {
                    _logger.LogError("Fout bij drop/create database: {err}", err);
                    throw new Exception($"Database reset mislukt: {err}");
                }
                _logger.LogInformation("Database `{db}` is succesvol opnieuw aangemaakt.", dbName);
            }

            // 6) Importeer de SQL
            if (OperatingSystem.IsWindows()) {
                var windowsArgs = $"/C \"{_mysqlPath} -h{host} -P{port} -u{user} -p{pwd} {dbName} < \"{sqlFilePath}\"\"";
                var winInfo = new ProcessStartInfo {
                    FileName = "cmd.exe",
                    Arguments = windowsArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var proc = new Process { StartInfo = winInfo }) {
                    _logger.LogInformation("SQL import (Windows) commando: cmd.exe {args}", windowsArgs);
                    proc.Start();
                    var err = await proc.StandardError.ReadToEndAsync();
                    proc.WaitForExit();
                    if (proc.ExitCode != 0) {
                        _logger.LogError("SQL import fout (Windows): {err}", err);
                        throw new Exception($"SQL import (Windows) mislukt: {err}");
                    }
                    _logger.LogInformation("SQL import (Windows) gelukt: {sql}", sqlFilePath);
                }
            } else {
                var linuxArgs = $"-c \"{_mysqlPath} -h{host} -P{port} -u{user} -p{pwd} {dbName} < '{sqlFilePath}'\"";
                var linInfo = new ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = linuxArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var proc = new Process { StartInfo = linInfo }) {
                    _logger.LogInformation("SQL import (Linux) commando: bash {args}", linuxArgs);
                    proc.Start();
                    var err = await proc.StandardError.ReadToEndAsync();
                    proc.WaitForExit();
                    if (proc.ExitCode != 0) {
                        _logger.LogError("SQL import fout (Linux): {err}", err);
                        throw new Exception($"SQL import (Linux) mislukt: {err}");
                    }
                    _logger.LogInformation("SQL import (Linux) gelukt: {sql}", sqlFilePath);
                }
            }

            // 7) Herstel asset-mappen (onder wwwroot)
            var wwwroot = Path.Combine(_env.ContentRootPath, "wwwroot");
            var assetFolders = new[] { "images", "icons", "uploads" };
            foreach (var folderName in assetFolders) {
                var targetFolder = Path.Combine(wwwroot, folderName);
                if (Directory.Exists(targetFolder))
                    Directory.Delete(targetFolder, recursive: true);

                var sourceFolder = Path.Combine(tempRoot, folderName);
                if (Directory.Exists(sourceFolder)) {
                    CopyDirectoryRecursively(sourceFolder, targetFolder);
                    _logger.LogInformation("Asset hersteld: {src} → {dest}", sourceFolder, targetFolder);
                } else {
                    _logger.LogWarning("Map '{folder}' niet gevonden in ZIP; overslaan: {target}", folderName, targetFolder);
                }
            }

            _logger.LogInformation("Restore voltooid. Database en assets bijgewerkt.");

            // 8) Verwijder de tijdelijke map
            Directory.Delete(tempRoot, recursive: true);
        }

        #region Hulpmethoden

        /// <summary>
        /// Kopieert de bestanden en submappen uit sourceDir naar destinationDir,
        /// inclusief alle subfolders in dezelfde hiërarchie.
        /// </summary>
        private static void CopyDirectoryRecursively(string sourceDir, string destinationDir) {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists) return;

            Directory.CreateDirectory(destinationDir);

            foreach (var file in dir.GetFiles()) {
                var targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, overwrite: true);
            }

            foreach (var subDir in dir.GetDirectories()) {
                var newDestSubDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectoryRecursively(subDir.FullName, newDestSubDir);
            }
        }

        #endregion
    }
}
