using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace MiniTodo.data
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class AppDbContext: DbContext
    {
        public DbSet<Todo>? Todos { get; set; }     

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("DataSource=app.db;Cache=Shared");

        private string GetDebuggerDisplay() => ToString();
    }
}