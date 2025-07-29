using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Infrastructure.Repositories.MysqlRepos;
using Jogging.Persistence.Models;
using Jogging.Rest.DTOs.RunningclubDtos;
using Jogging.Rest.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Jogging.Rest.Controllers;
[Controller]
[Route("api/[controller]/")]
public class RunningclubController:ControllerBaseExtension
{
    private IRunningclubRepo _repo;

    public RunningclubController(IRunningclubRepo repo)
    {
        _repo = repo;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<RunningclubResponse>>> GetAllAsync()
    {
        var clubs = await _repo.GetAllAsync();

        if (clubs == null)
        {
            return NotFound("Clubs not found");
        }
        return Ok(clubs.Select(c => c.Map()));
    }

    [HttpGet]
    [Route("get-with-data")]
    public async Task<ActionResult<List<RunningClubWithData>>> GetAllWithDataAsync()
    {
        var clubs = await _repo.GetAllWithDataAsync();

        if (clubs == null)
        {
            return NotFound("Clubs not found");
        }
        return Ok(clubs);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<RunningclubResponse>> GetByIdAsync(int id)
    {
        var club = await _repo.GetByIdAsync(id);

        if (club == null)
        {
            return NotFound("Club not found");
        }
        return Ok(club.Map());
    }
    [HttpGet]
    [Route("name/{name}")]
    public async Task<ActionResult<RunningclubResponse>> GetByNameAsync(string name)
    {
        var club = await _repo.GetByNameAsync(name);
        if (club == null)
        {
            return NotFound("Club not found");
        }
        return Ok(club.Map());
    }
    [HttpGet]
    [Route("user/{userId}")]
    public async Task<ActionResult<RunningclubResponse>> GetByUserId(int userId)
    {
        var club = await _repo.GetByUserIdAsync(userId);
        if (club == null)
        {
            return NotFound("Club not found");
        }
        return Ok(club.Map());
    }
    [HttpPost]
    public async Task<ActionResult> AddAsync(RunningclubRequest club)
    {
        if (club == null)
        {
            return BadRequest();
        }

        try
        {
            await _repo.AddAsync(club.Map());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok("Success");
        
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> RegisterPersonToClubAsync(int userId, int clubId)
    {
        if (userId == null || clubId == null)
        {
            return BadRequest();
        }

        try
        {
            await _repo.RegisterPersonToClubAsync(userId, clubId);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok("Success");
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> UpdateAsync(int id,RunningclubRequest club)
    {
        if (club == null )
        {
            return BadRequest();
        }

        if (id == null)
        {
            return BadRequest();
        }

        try
        {
            await _repo.UpdateAsync(id, club.Map());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok("Success");
    }
    
    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        if (id == null)
        {
            return BadRequest();
        }

        try
        {
            await _repo.DeleteAsync(id);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok("Success");
    }
    
}