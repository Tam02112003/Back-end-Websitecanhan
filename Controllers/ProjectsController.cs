using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using Websitecanhan.Models;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IDynamoDBContext _context;

    public ProjectsController(IDynamoDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var conditions = new List<ScanCondition>();
        var projects = await _context.ScanAsync<Project>(conditions).GetRemainingAsync();
        return projects;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        try
        {
            var project = await _context.LoadAsync<Project>(id);
            if (project == null) return NotFound();
            return project;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Project>> PostProject(Project project)
    {
        await _context.SaveAsync(project);
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProject(int id, Project project)
    {
        if (id != project.Id)
        {
            return BadRequest();
        }

        var existingProject = await _context.LoadAsync<Project>(id);
        if (existingProject == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _context.SaveAsync(project);
        }
        catch
        {
            return StatusCode(500, "An error occurred while updating the project.");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.LoadAsync<Project>(id);
        if (project == null) return NotFound();

        await _context.DeleteAsync(project);
        return NoContent();
    }
}