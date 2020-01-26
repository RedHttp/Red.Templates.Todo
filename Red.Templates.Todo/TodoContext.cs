using Microsoft.EntityFrameworkCore;

namespace Red.Templates.Todo
{
    public class TodoContext : DbContext
    {
        private readonly int _userId;

        public TodoContext(int userId)
        {
            _userId = userId;
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Todo>()
                .HasOne(todo => todo.User).WithMany()
                .HasForeignKey(todo => todo.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Todo>().HasQueryFilter(todo => todo.UserId == _userId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql($"Host=localhost:5432;Database=TodoDb;Username=postgres;Password=todotodo");
        }
    }
}