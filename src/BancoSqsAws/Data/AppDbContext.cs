using Microsoft.EntityFrameworkCore;
using BancoSqsAws.Models;

namespace BancoSqsAws.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MessageEntity> Messages { get; set; }
}
