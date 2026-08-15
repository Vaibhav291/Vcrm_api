using Microsoft.EntityFrameworkCore;
using Vcrm.Models;

namespace Vcrm.Data;

public class VcrmDbContext : DbContext
{
    public VcrmDbContext(DbContextOptions<VcrmDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<User> Users => Set<User>();
}
