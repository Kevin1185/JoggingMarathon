#if BACKUP
using Jogging.Domain.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Jogging.Rest.Controllers;
[Controller]
public class BackupImportController: ControllerBase
{
    [HttpGet]
    [Route("Backup")]
    public async Task<IActionResult> Backup()
    {
        var file = await DatabaseHelper.Backup();
        Response.Headers.Add("Content-Disposition", "attachment; filename=backup.sql");

        Response.OnCompleted(async () =>
        {
            await file.DisposeAsync();
            System.IO.File.Delete(file.Name);
        });

        return File(file, "application/sql");
    }

    [HttpPost]
    [Route("Import")]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file selected");
        }
        DatabaseHelper.Import(file);
        
        return Ok();
    }
    
}
#endif