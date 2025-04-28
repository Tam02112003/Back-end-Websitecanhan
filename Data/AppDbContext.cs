using Microsoft.EntityFrameworkCore;
using Websitecanhan.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Certificate> Certificates { get; set; }
}