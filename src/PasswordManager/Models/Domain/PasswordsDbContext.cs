using Microsoft.EntityFrameworkCore;

namespace PasswordManager.Models.Domain
{
    public class PasswordsDbContext : DbContext
    {
        public PasswordsDbContext(DbContextOptions<PasswordsDbContext> options)
        : base(options)
        {

        }

        public DbSet<Password> PasswordItem { get; set; } = null;
    }
}