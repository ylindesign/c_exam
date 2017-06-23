using Microsoft.EntityFrameworkCore;
 
namespace exam.Models {
    public class ExamContext : DbContext {
        public ExamContext(DbContextOptions<ExamContext> options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Activity> activities { get; set; }
        public DbSet<Part> part { get; set; }
    }
}