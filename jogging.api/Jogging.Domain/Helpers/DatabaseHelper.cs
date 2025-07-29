using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Jogging.Domain.Helpers;


public static class DatabaseHelper
{
    private static string? _connectionString;
    private static string? _connectionwithoutDb;

    public static void SetConnectionString(string connectionstring, string connectionwithoutDb)
    {
        _connectionString = connectionstring;
        _connectionwithoutDb = connectionwithoutDb;
    }

#if BACKUP

    public static async Task<FileStream> Backup()
    {
        var fileLocation = Directory.GetCurrentDirectory() + "\\backup.sql";

        using (var conn = new MySqlConnection(_connectionString))
        {
            using (var cmd = conn.CreateCommand())
            {
                using (var mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = conn;
                    await conn.OpenAsync();
                    mb.ExportToFile(fileLocation);
                    await conn.CloseAsync();
                }
            }
        }
      return new FileStream(fileLocation, FileMode.Open);
    }

    public static async Task Import(IFormFile file)
    {
        
        //Make path to a file 
        var filepath = Path.Combine(Path.GetTempPath(), file.FileName);

        using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
        {
            await file.CopyToAsync(stream);
        }
        //Creates new db so an import could be made
        await CreateDatabaseIfDontExist("jogging");
        using (var conn = new MySqlConnection(_connectionString))
        {
            using (var cmd = conn.CreateCommand())
            {
                using (var mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = conn;
                    await conn.OpenAsync();
                    mb.ImportFromFile(filepath);
                }
            }
        }
        
        File.Delete(filepath);
    }
#endif 

    public static async Task CreateDatabaseIfDontExist(string dbName)
    {
        using (var connection = new MySqlConnection(_connectionwithoutDb))
        {
            await connection.OpenAsync();
            var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{dbName}`;", connection);
            cmd.ExecuteNonQuery();
            await connection.CloseAsync();
        }
    }
}