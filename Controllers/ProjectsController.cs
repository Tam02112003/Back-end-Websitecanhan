using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Websitecanhan.Models;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        return await _context.Projects.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        try
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();
            return project;
           
        }
        catch (Exception ex)
        {
            // Ghi log lỗi
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Internal server error");
        }

    }

    [HttpPost]
    public async Task<ActionResult<Project>> PostProject(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpPut]
    public async Task<IActionResult> PutProject([FromHeader(Name = "X-Project-ID")] int id, [FromBody] Project project)
    {
        // Kiểm tra xem bản ghi có tồn tại không
        var existingProject = await _context.Projects.FindAsync(id);
        if (existingProject == null)
        {
            return NotFound(); // Trả về 404 nếu không tìm thấy bản ghi
        }

        // Kiểm tra xác thực mô hình
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Cập nhật các thuộc tính của project
        existingProject.Name = project.Name;
        existingProject.Description = project.Description;
        existingProject.StartDate = project.StartDate;
        existingProject.EndDate = project.EndDate;
        existingProject.Technologies = project.Technologies;
        existingProject.GitHubLink = project.GitHubLink;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, "An error occurred while updating the project."); // Trả về lỗi 500 nếu có lỗi
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}