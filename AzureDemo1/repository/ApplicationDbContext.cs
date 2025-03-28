using AzureDemo1.Model;
using Microsoft.EntityFrameworkCore;

namespace AzureDemo1.repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }
        public DbSet<TestItem> TestItems { get; set; }
    }
}
