using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2.DataModel;
using Websitecanhan.Models;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Websitecanhan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IDynamoDBContext _context;
    private readonly IConfiguration _config;

    public ProjectsController(IDynamoDBContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
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

    [HttpPost("{id}/upload-image")]
    public async Task<IActionResult> UploadProjectImage(int id, IFormFile file)
    {
        var project = await _context.LoadAsync<Project>(id);
        if (project == null) return NotFound();

        // Gọi ImagesController để upload
        var imagesController = new ImagesController(
            new Cloudinary(new Account(
                _config["Cloudinary:CloudName"],
                _config["Cloudinary:ApiKey"],
                _config["Cloudinary:ApiSecret"]
            ))
        );

        var result = await imagesController.UploadImage(file) as OkObjectResult;
        if (result == null) return BadRequest("Upload failed");

        // Lưu metadata vào Project
        project.ImageUrl = result.Value.GetType().GetProperty("Url").GetValue(result.Value).ToString();
        project.ImagePublicId = result.Value.GetType().GetProperty("PublicId").GetValue(result.Value).ToString();

        await _context.SaveAsync(project);
        return Ok(project);
    }

    [HttpDelete("{id}/delete-image")]
    public async Task<IActionResult> DeleteProjectImage(int id)
    {
        var project = await _context.LoadAsync<Project>(id);
        if (project == null || string.IsNullOrEmpty(project.ImagePublicId))
            return NotFound();

        // Xóa ảnh từ Cloudinary
        var cloudinary = new Cloudinary(new Account(
            _config["Cloudinary:CloudName"],
            _config["Cloudinary:ApiKey"],
            _config["Cloudinary:ApiSecret"]
        ));

        var deleteParams = new DeletionParams(project.ImagePublicId);
        await cloudinary.DestroyAsync(deleteParams);

        // Xóa metadata trong Project
        project.ImageUrl = null;
        project.ImagePublicId = null;
        await _context.SaveAsync(project);

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