using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Story> Stories { get; set; }
    }
}