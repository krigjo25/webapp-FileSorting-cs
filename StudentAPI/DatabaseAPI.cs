using Microsoft.EntityFrameworkCore;

namespace Webapp.sorting.cs;


public class MSSQL : DbContext
{
    public MSSQL(DbContextOptions<MSSQL> options) : base(options)
    {
    }
    public DbSet<Students> Students  => Set<Students>();
}
